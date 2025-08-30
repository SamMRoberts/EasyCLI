namespace EasyCLI.Shell.SignalHandling
{
    /// <summary>
    /// Provides cross-platform signal handling capabilities for CLI applications.
    /// Manages OS signals like SIGINT and SIGTERM and coordinates graceful shutdown.
    /// </summary>
    public interface ISignalHandler : IDisposable
    {
        /// <summary>
        /// Event raised when a cancellation signal (SIGINT, SIGTERM) is received.
        /// </summary>
        event EventHandler<SignalReceivedEventArgs>? SignalReceived;

        /// <summary>
        /// Gets a cancellation token that is triggered when a shutdown signal is received.
        /// </summary>
        CancellationToken ShutdownToken { get; }

        /// <summary>
        /// Enables signal handling and begins monitoring for shutdown signals.
        /// </summary>
        void Start();

        /// <summary>
        /// Disables signal handling and stops monitoring for signals.
        /// </summary>
        void Stop();

        /// <summary>
        /// Gets a value indicating whether gets whether signal handling is currently active.
        /// </summary>
        bool IsActive { get; }
    }

    /// <summary>
    /// Event arguments for signal received events.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SignalReceivedEventArgs"/> class.
    /// </remarks>
    /// <param name="signalType">The type of signal that was received.</param>
    public class SignalReceivedEventArgs(SignalType signalType) : EventArgs
    {

        /// <summary>
        /// Gets the type of signal that was received.
        /// </summary>
        public SignalType SignalType { get; } = signalType;
    }

    /// <summary>
    /// Represents the types of signals that can be handled.
    /// </summary>
    public enum SignalType
    {
        /// <summary>
        /// Interrupt signal (Ctrl+C on most platforms).
        /// </summary>
        Interrupt,

        /// <summary>
        /// Termination signal (usually SIGTERM on Unix systems).
        /// </summary>
        Terminate,

        /// <summary>
        /// Hangup signal (usually SIGHUP on Unix systems).
        /// </summary>
        Hangup,

        /// <summary>
        /// Application-specific or unknown signal.
        /// </summary>
        Other,
    }
}
