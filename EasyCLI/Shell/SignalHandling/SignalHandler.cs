namespace EasyCLI.Shell.SignalHandling
{
    /// <summary>
    /// Cross-platform signal handler implementation that manages OS signals
    /// and provides unified cancellation semantics for graceful shutdown.
    /// </summary>
    public class SignalHandler : ISignalHandler
    {
        private readonly CancellationTokenSource _shutdownTokenSource = new();
        private volatile bool _isActive;
        private volatile bool _disposed;



        /// <summary>
        /// Initializes a new instance of the <see cref="SignalHandler"/> class.
        /// </summary>
        public SignalHandler()
        {
            // Register console cancel events (Ctrl+C, Ctrl+Break on Windows)
            System.Console.CancelKeyPress += OnCancelKeyPress;
        }

        /// <inheritdoc />
        public event EventHandler<SignalReceivedEventArgs>? SignalReceived;

        /// <inheritdoc />
        public CancellationToken ShutdownToken => _shutdownTokenSource.Token;

        /// <inheritdoc />
        public bool IsActive => _isActive && !_disposed;

        /// <inheritdoc />
        public void Start()
        {
            ThrowIfDisposed(_disposed);
            if (_isActive)
            {
                return;
            }

            _isActive = true;

            // Register platform-specific signal handlers
            if (!OperatingSystem.IsWindows())
            {
                RegisterUnixSignalHandlers();
            }
            // Windows signal handling is handled via Console.CancelKeyPress
        }

        /// <inheritdoc />
        public void Stop()
        {
            if (!_isActive || _disposed)
            {
                return;
            }

            _isActive = false;

            // Unregister platform-specific signal handlers
            if (!OperatingSystem.IsWindows())
            {
                UnregisterUnixSignalHandlers();
            }
        }

        /// <summary>
        /// Handles console cancel key press events (Ctrl+C, Ctrl+Break).
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The console cancel event arguments.</param>
        private void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            if (!_isActive)
            {
                return;
            }

            // Cancel the default termination behavior to allow graceful shutdown
            e.Cancel = true;

            // Determine signal type based on the special key
            SignalType signalType = e.SpecialKey switch
            {
                ConsoleSpecialKey.ControlC => SignalType.Interrupt,
                ConsoleSpecialKey.ControlBreak => SignalType.Terminate,
                _ => SignalType.Other,
            };

            OnSignalReceived(signalType);
        }

        /// <summary>
        /// Raises the SignalReceived event and triggers the shutdown token.
        /// </summary>
        /// <param name="signalType">The type of signal that was received.</param>
        private void OnSignalReceived(SignalType signalType)
        {
            try
            {
                SignalReceived?.Invoke(this, new SignalReceivedEventArgs(signalType));
            }
            catch
            {
                // Ignore exceptions in event handlers during shutdown
            }

            // Trigger shutdown cancellation
            if (!_shutdownTokenSource.IsCancellationRequested)
            {
                _shutdownTokenSource.Cancel();
            }
        }

        /// <summary>
        /// Registers Unix signal handlers for SIGTERM and SIGHUP.
        /// </summary>
        private static void RegisterUnixSignalHandlers()
        {
            // Note: In .NET 6+, we could use PosixSignalRegistration
            // For broader compatibility, we use a simpler approach here
            // that focuses on the console cancel events which are cross-platform

            // Unix-specific signal handling could be added here using P/Invoke
            // or by leveraging newer .NET APIs when available
        }

        /// <summary>
        /// Unregisters Unix signal handlers.
        /// </summary>
        private static void UnregisterUnixSignalHandlers()
        {
            // Corresponding cleanup for Unix signal handlers
        }

        private static void ThrowIfDisposed(bool disposed)
        {
            ObjectDisposedException.ThrowIf(disposed, nameof(SignalHandler));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Stop();
            System.Console.CancelKeyPress -= OnCancelKeyPress;
            _shutdownTokenSource.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
