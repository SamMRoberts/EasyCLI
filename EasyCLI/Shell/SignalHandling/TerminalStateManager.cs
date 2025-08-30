namespace EasyCLI.Shell.SignalHandling
{
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
                try
                {
#pragma warning disable CA1416 // Validate platform compatibility
                    _cursorVisible = System.Console.CursorVisible;
#pragma warning restore CA1416 // Validate platform compatibility
                }
                catch (PlatformNotSupportedException)
                {
                    _cursorVisible = true; // Default value if not supported
                }
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
                try
                {
                    System.Console.CursorVisible = _cursorVisible;
                }
                catch (PlatformNotSupportedException)
                {
                    // Cursor visibility not supported on this platform
                }

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
                    RestoreStateAsync().GetAwaiter().GetResult();
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
                    try
                    {
#pragma warning disable CA1416 // Validate platform compatibility
                        _originalCursorVisible = System.Console.CursorVisible;
#pragma warning restore CA1416 // Validate platform compatibility
                    }
                    catch (PlatformNotSupportedException)
                    {
                        _originalCursorVisible = true; // Default value if not supported
                    }

                    // Apply the modification
                    switch (modification)
                    {
                        case TerminalModification.HideCursor:
                            try
                            {
                                System.Console.CursorVisible = false;
                            }
                            catch (PlatformNotSupportedException)
                            {
                                // Cursor visibility not supported on this platform
                            }
                            break;
                        case TerminalModification.ShowCursor:
                            try
                            {
                                System.Console.CursorVisible = true;
                            }
                            catch (PlatformNotSupportedException)
                            {
                                // Cursor visibility not supported on this platform
                            }
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
                            try
                            {
                                System.Console.CursorVisible = _originalCursorVisible;
                            }
                            catch (PlatformNotSupportedException)
                            {
                                // Cursor visibility not supported on this platform
                            }
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
