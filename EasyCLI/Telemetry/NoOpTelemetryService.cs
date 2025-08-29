namespace EasyCLI.Telemetry
{
    /// <summary>
    /// Default telemetry service implementation that performs no operations.
    /// This is the default implementation ensuring no data is collected unless
    /// explicitly configured and consented to by the user.
    /// </summary>
    public class NoOpTelemetryService : ITelemetryService
    {
        /// <summary>
        /// Gets a value indicating whether telemetry collection is enabled.
        /// Always returns false for the no-op implementation.
        /// </summary>
        public bool IsEnabled => false;

        /// <summary>
        /// Gets a value indicating whether the user has provided explicit consent for telemetry.
        /// Always returns false for the no-op implementation.
        /// </summary>
        public bool HasUserConsent => false;

        /// <summary>
        /// Tracks a command execution event. No operation is performed.
        /// </summary>
        /// <param name="commandName">The name of the command executed.</param>
        /// <param name="success">Whether the command executed successfully.</param>
        /// <param name="metadata">Optional metadata about the execution.</param>
        public void TrackCommand(string commandName, bool success, IDictionary<string, object>? metadata = null)
        {
            // No operation - telemetry disabled by default
        }

        /// <summary>
        /// Tracks an error event. No operation is performed.
        /// </summary>
        /// <param name="error">The error message or exception type.</param>
        /// <param name="metadata">Optional metadata about the error context.</param>
        public void TrackError(string error, IDictionary<string, object>? metadata = null)
        {
            // No operation - telemetry disabled by default
        }

        /// <summary>
        /// Flushes any pending telemetry data. No operation is performed.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A completed task.</returns>
        public Task FlushAsync(CancellationToken cancellationToken = default)
        {
            // No operation - telemetry disabled by default
            return Task.CompletedTask;
        }
    }
}
