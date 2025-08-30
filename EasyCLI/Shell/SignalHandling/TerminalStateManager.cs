namespace EasyCLI.Shell.SignalHandling
{
    /// <summary>
    /// Manages terminal state and provides restoration capabilities during shutdown.
    /// Handles cursor visibility, terminal modes, and other console state that may
    /// need to be restored when a CLI application is interrupted.
    /// </summary>
    public interface ITerminalStateManager : IDisposable
    {
        /// <summary>
        /// Captures the current terminal state for later restoration.
        /// </summary>
        void CaptureState();

        /// <summary>
        /// Restores the terminal to its previously captured state.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for the restore operation.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task RestoreStateAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a value indicating whether terminal state has been captured.
        /// </summary>
        bool HasCapturedState { get; }

        /// <summary>
        /// Temporarily modifies terminal state (e.g., hide cursor) with automatic restoration.
        /// </summary>
        /// <param name="modification">The modification to apply.</param>
        /// <returns>A disposable that will restore the state when disposed.</returns>
        IDisposable TemporaryModification(TerminalModification modification);
    }

    /// <summary>
    /// Represents types of terminal modifications that can be applied.
    /// </summary>
    public enum TerminalModification
    {
        /// <summary>
        /// Hide the cursor.
        /// </summary>
        HideCursor,

        /// <summary>
        /// Show the cursor.
        /// </summary>
        ShowCursor,

        /// <summary>
        /// Enter raw mode (disable line buffering and echo).
        /// </summary>
        RawMode,

        /// <summary>
        /// Enter normal mode (enable line buffering and echo).
        /// </summary>
        NormalMode,
    }

    /// <summary>
    /// Default implementation of <see cref="ITerminalStateManager"/> that manages
    /// basic console state like cursor visibility and position.
    /// </summary>
    public class TerminalStateManager : ITerminalStateManager
    {
        private bool _cursorVisible = true;
        private int _cursorLeft;
        private int _cursorTop;
        private bool _hasCapturedState;
        private volatile bool _disposed;

        /// <inheritdoc />
        public bool HasCapturedState => _hasCapturedState && !_disposed;

        /// <inheritdoc />
        public void CaptureState()
        {
            ThrowIfDisposed();

            try
            {
                // Capture current console state
                _cursorVisible = System.Console.CursorVisible;
                _cursorLeft = System.Console.CursorLeft;
                _cursorTop = System.Console.CursorTop;
                _hasCapturedState = true;
            }
            catch
            {
                // If we can't access console properties (e.g., redirected output),
                // we'll just use defaults and mark state as captured
                _cursorVisible = true;
                _cursorLeft = 0;
                _cursorTop = 0;
                _hasCapturedState = true;
            }
        }

        /// <inheritdoc />
        public Task RestoreStateAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (!_hasCapturedState)
            {
                return Task.CompletedTask;
            }

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Restore cursor visibility
                System.Console.CursorVisible = _cursorVisible;

                // Only restore cursor position if we can determine current buffer state
                try
                {
                    if (_cursorTop < System.Console.BufferHeight && _cursorLeft < System.Console.BufferWidth)
                    {
                        System.Console.SetCursorPosition(_cursorLeft, _cursorTop);
                    }
                }
                catch
                {
                    // Ignore cursor position restoration failures (e.g., buffer size changed)
                }

                // Ensure any pending output is flushed
                System.Console.Out.Flush();
                System.Console.Error.Flush();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                // Ignore restoration errors during shutdown
                // (console may not be available or output may be redirected)
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public IDisposable TemporaryModification(TerminalModification modification)
        {
            ThrowIfDisposed();
            return new TemporaryTerminalModification(modification);
        }

        private void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(TerminalStateManager));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            // Attempt to restore state on dispose if we had captured it
            if (_hasCapturedState)
            {
                try
                {
                    _ = RestoreStateAsync().Wait(TimeSpan.FromSeconds(1));
                }
                catch
                {
                    // Ignore errors during dispose
                }
            }

            _disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implements temporary terminal modifications with automatic restoration.
        /// </summary>
        private sealed class TemporaryTerminalModification : IDisposable
        {
            private readonly TerminalModification _modification;
            private readonly bool _originalCursorVisible;
            private volatile bool _disposed;

            public TemporaryTerminalModification(TerminalModification modification)
            {
                _modification = modification;

                try
                {
                    _originalCursorVisible = System.Console.CursorVisible;

                    // Apply the modification
                    switch (modification)
                    {
                        case TerminalModification.HideCursor:
                            System.Console.CursorVisible = false;
                            break;
                        case TerminalModification.ShowCursor:
                            System.Console.CursorVisible = true;
                            break;
                        case TerminalModification.RawMode:
                        case TerminalModification.NormalMode:
                            // Raw/Normal mode changes would require platform-specific implementation
                            // For now, we focus on cursor visibility which is cross-platform
                            break;
                        default:
                            break;
                    }
                }
                catch
                {
                    // If modification fails, remember the original state for restoration
                    _originalCursorVisible = true;
                }
            }

            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                try
                {
                    // Restore original state
                    switch (_modification)
                    {
                        case TerminalModification.HideCursor:
                        case TerminalModification.ShowCursor:
                            System.Console.CursorVisible = _originalCursorVisible;
                            break;
                        case TerminalModification.RawMode:
                        case TerminalModification.NormalMode:
                            // Restore from raw/normal mode
                            break;
                        default:
                            break;
                    }
                }
                catch
                {
                    // Ignore restoration errors
                }

                _disposed = true;
            }
        }
    }
}
