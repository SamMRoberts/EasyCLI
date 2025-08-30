namespace EasyCLI.Shell.SignalHandling
{
    /// <summary>
    /// Default implementation of <see cref="ICleanupManager"/>.
    /// </summary>
    public class CleanupManager : ICleanupManager
    {
        private readonly List<CleanupRegistration> _cleanupActions = [];
        private readonly Lock _lock = new();
        private volatile bool _disposed;

        /// <inheritdoc />
        public int RegisteredCleanupCount
        {
            get
            {
                lock (_lock)
                {
                    return _cleanupActions.Count;
                }
            }
        }

        /// <inheritdoc />
        public ICleanupHandle RegisterCleanup(Func<CancellationToken, Task> cleanup, string? name = null)
        {
            ThrowIfDisposed();
            ArgumentNullException.ThrowIfNull(cleanup);

            Guid id = Guid.NewGuid();
            CleanupRegistration registration = new(id, cleanup, name, this);

            lock (_lock)
            {
                _cleanupActions.Add(registration);
            }

            return new CleanupHandle(registration);
        }

        /// <inheritdoc />
        public ICleanupHandle RegisterCleanup(Action cleanup, string? name = null)
        {
            ArgumentNullException.ThrowIfNull(cleanup);

            // Wrap synchronous action in async wrapper
            return RegisterCleanup(
                _ =>
                {
                    cleanup();
                    return Task.CompletedTask;
                },
                name);
        }

        /// <inheritdoc />
        public async Task<bool> ExecuteCleanupAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            // Get all registrations in reverse order (LIFO - Last In, First Out)
            CleanupRegistration[] registrations;
            lock (_lock)
            {
                registrations = [.. _cleanupActions.AsEnumerable().Reverse()];
            }

            if (registrations.Length == 0)
            {
                return true;
            }

            using CancellationTokenSource timeoutCts = new(timeout);
            using CancellationTokenSource combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            bool allSucceeded = true;

            foreach (CleanupRegistration? registration in registrations)
            {
                try
                {
                    await registration.Cleanup(combinedCts.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (combinedCts.Token.IsCancellationRequested)
                {
                    // Timeout or external cancellation - stop processing further cleanup actions
                    allSucceeded = false;
                    break;
                }
                catch
                {
                    // Individual cleanup failed - continue with remaining actions
                    allSucceeded = false;
                }
            }

            return allSucceeded;
        }

        /// <summary>
        /// Internal method to unregister a cleanup action by ID.
        /// </summary>
        /// <param name="id">The ID of the cleanup action to unregister.</param>
        internal void UnregisterCleanup(Guid id)
        {
            lock (_lock)
            {
                _ = _cleanupActions.RemoveAll(r => r.Id == id);
            }
        }

        private void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(CleanupManager));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            // Don't execute cleanup actions during dispose - they should be executed
            // explicitly via ExecuteCleanupAsync during shutdown
            lock (_lock)
            {
                _cleanupActions.Clear();
            }
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Handle implementation for cleanup registrations.
        /// </summary>
        internal sealed class CleanupHandle(CleanupRegistration registration) : ICleanupHandle
        {
            private readonly CleanupRegistration _registration = registration;
            private volatile bool _disposed;

            /// <inheritdoc />
            public string? Name => _registration.Name;

            /// <inheritdoc />
            public bool IsRegistered
            {
                get
                {
                    if (_disposed)
                    {
                        return false;
                    }

                    lock (_registration.Manager._lock)
                    {
                        return _registration.Manager._cleanupActions.Any(r => r.Id == _registration.Id);
                    }
                }
            }

            /// <inheritdoc />
            public void Unregister()
            {
                if (!_disposed)
                {
                    _registration.Manager.UnregisterCleanup(_registration.Id);
                    _disposed = true;
                }
            }

            /// <inheritdoc />
            public void Dispose()
            {
                Unregister();
            }
        }

        /// <summary>
        /// Internal registration data for cleanup actions.
        /// </summary>
        internal sealed record CleanupRegistration(
            Guid Id,
            Func<CancellationToken, Task> Cleanup,
            string? Name,
            CleanupManager Manager);
    }
}
