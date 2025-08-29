namespace EasyCLI.Telemetry
{
    /// <summary>
    /// Interface for telemetry services. Implementations can collect usage data
    /// and diagnostic information with explicit user consent.
    /// </summary>
    public interface ITelemetryService
    {
        /// <summary>
        /// Gets a value indicating whether telemetry collection is enabled.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether the user has provided explicit consent for telemetry.
        /// </summary>
        bool HasUserConsent { get; }

        /// <summary>
        /// Tracks a command execution event with optional metadata.
        /// </summary>
        /// <param name="commandName">The name of the command executed.</param>
        /// <param name="success">Whether the command executed successfully.</param>
        /// <param name="metadata">Optional metadata about the execution.</param>
        void TrackCommand(string commandName, bool success, IDictionary<string, object>? metadata = null);

        /// <summary>
        /// Tracks an error event with exception details.
        /// </summary>
        /// <param name="error">The error message or exception type.</param>
        /// <param name="metadata">Optional metadata about the error context.</param>
        void TrackError(string error, IDictionary<string, object>? metadata = null);

        /// <summary>
        /// Flushes any pending telemetry data.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task FlushAsync(CancellationToken cancellationToken = default);
    }
}
