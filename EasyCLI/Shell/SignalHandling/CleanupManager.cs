using System.Collections.Concurrent;

namespace EasyCLI.Shell.SignalHandling
{
    /// <summary>
    /// Manages cleanup actions that should be executed during graceful shutdown.
    /// Provides a mechanism for commands and services to register cleanup hooks.
    /// </summary>
    public interface ICleanupManager : IDisposable
    {
        /// <summary>
        /// Registers a cleanup action to be executed during shutdown.
        /// </summary>
        /// <param name="cleanup">The cleanup action to register. Should be safe to call multiple times.</param>
        /// <param name="name">Optional name for the cleanup action (for debugging/logging).</param>
        /// <returns>A handle that can be used to unregister the cleanup action.</returns>
        ICleanupHandle RegisterCleanup(Func<CancellationToken, Task> cleanup, string? name = null);

        /// <summary>
        /// Registers a synchronous cleanup action to be executed during shutdown.
        /// </summary>
        /// <param name="cleanup">The cleanup action to register. Should be safe to call multiple times.</param>
        /// <param name="name">Optional name for the cleanup action (for debugging/logging).</param>
        /// <returns>A handle that can be used to unregister the cleanup action.</returns>
        ICleanupHandle RegisterCleanup(Action cleanup, string? name = null);

        /// <summary>
        /// Executes all registered cleanup actions in reverse order of registration.
        /// This method is typically called during graceful shutdown.
        /// </summary>
        /// <param name="timeout">Maximum time to wait for all cleanup actions to complete.</param>
        /// <param name="cancellationToken">Cancellation token for the cleanup operation.</param>
        /// <returns>True if all cleanup actions completed successfully, false if some failed or timed out.</returns>
        Task<bool> ExecuteCleanupAsync(TimeSpan timeout, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the number of currently registered cleanup actions.
        /// </summary>
        int RegisteredCleanupCount { get; }
    }

    /// <summary>
    /// Handle for a registered cleanup action that allows unregistration.
    /// </summary>
    public interface ICleanupHandle : IDisposable
    {
        /// <summary>
        /// Gets the name of the cleanup action (if provided during registration).
        /// </summary>
        string? Name { get; }

        /// <summary>
        /// Gets a value indicating whether gets whether this cleanup action is still registered.
        /// </summary>
        bool IsRegistered { get; }

        /// <summary>
        /// Unregisters the cleanup action. After calling this, the cleanup will not be executed.
        /// </summary>
        void Unregister();
    }

    /// <summary>
    /// Default implementation of <see cref="ICleanupManager"/>.
    /// </summary>
    public class CleanupManager : ICleanupManager
    {
        private readonly ConcurrentDictionary<Guid, CleanupRegistration> _cleanupActions = new();
        private volatile bool _disposed;

        /// <inheritdoc />
        public int RegisteredCleanupCount => _cleanupActions.Count;

        /// <inheritdoc />
        public ICleanupHandle RegisterCleanup(Func<CancellationToken, Task> cleanup, string? name = null)
        {
            ThrowIfDisposed();
            ArgumentNullException.ThrowIfNull(cleanup);

            Guid id = Guid.NewGuid();
            CleanupRegistration registration = new(id, cleanup, name, this);
            _cleanupActions[id] = registration;

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
            }, name);
        }

        /// <inheritdoc />
        public async Task<bool> ExecuteCleanupAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            // Get all registrations in reverse order (LIFO - Last In, First Out)
            CleanupRegistration[] registrations = _cleanupActions.Values.ToArray().Reverse().ToArray();

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
            _ = _cleanupActions.TryRemove(id, out _);
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
            _cleanupActions.Clear();
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
            public bool IsRegistered => !_disposed && _registration.Manager._cleanupActions.ContainsKey(_registration.Id);

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
