namespace EasyCLI
{
    /// <summary>
    /// Represents a collected error with metadata.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="CollectedError"/> class.
    /// </remarks>
    /// <param name="category">The error category.</param>
    /// <param name="message">The error message.</param>
    /// <param name="source">The source or context where the error occurred.</param>
    /// <param name="details">Additional error details.</param>
    public class CollectedError(BatchErrorCategory category, string message, string? source = null, string? details = null)
    {

        /// <summary>
        /// Gets the error category.
        /// </summary>
        public BatchErrorCategory Category { get; } = category;

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string Message { get; } = message ?? throw new ArgumentNullException(nameof(message));

        /// <summary>
        /// Gets the source or context where the error occurred.
        /// </summary>
        public string? Source { get; } = source;

        /// <summary>
        /// Gets additional error details.
        /// </summary>
        public string? Details { get; } = details;

        /// <summary>
        /// Gets the timestamp when the error was collected.
        /// </summary>
        public DateTime Timestamp { get; } = DateTime.UtcNow;

        /// <inheritdoc />
        public override string ToString()
        {
            List<string> parts = new List<string> { Message };
            if (!string.IsNullOrEmpty(Source))
            {
                parts.Add($"Source: {Source}");
            }

            if (!string.IsNullOrEmpty(Details))
            {
                parts.Add($"Details: {Details}");
            }

            return string.Join(" | ", parts);
        }
    }
}
