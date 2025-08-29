namespace EasyCLI
{
    /// <summary>
    /// Represents a collected error with metadata.
    /// </summary>
    public class CollectedError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectedError"/> class.
        /// </summary>
        /// <param name="category">The error category.</param>
        /// <param name="message">The error message.</param>
        /// <param name="source">The source or context where the error occurred.</param>
        /// <param name="details">Additional error details.</param>
        public CollectedError(BatchErrorCategory category, string message, string? source = null, string? details = null)
        {
            Category = category;
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Source = source;
            Details = details;
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the error category.
        /// </summary>
        public BatchErrorCategory Category { get; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the source or context where the error occurred.
        /// </summary>
        public string? Source { get; }

        /// <summary>
        /// Gets additional error details.
        /// </summary>
        public string? Details { get; }

        /// <summary>
        /// Gets the timestamp when the error was collected.
        /// </summary>
        public DateTime Timestamp { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            var parts = new List<string> { Message };
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
