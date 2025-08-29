namespace EasyCLI
{
    /// <summary>
    /// Represents a summary of errors grouped by category.
    /// </summary>
    public class ErrorSummary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorSummary"/> class.
        /// </summary>
        /// <param name="category">The error category.</param>
        /// <param name="errors">The errors in this category.</param>
        public ErrorSummary(BatchErrorCategory category, IReadOnlyList<CollectedError> errors)
        {
            Category = category;
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
            Count = errors.Count;
        }

        /// <summary>
        /// Gets the error category.
        /// </summary>
        public BatchErrorCategory Category { get; }

        /// <summary>
        /// Gets the errors in this category.
        /// </summary>
        public IReadOnlyList<CollectedError> Errors { get; }

        /// <summary>
        /// Gets the count of errors in this category.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets a display name for the category.
        /// </summary>
        public string CategoryDisplayName => Category switch
        {
            BatchErrorCategory.FileSystem => "File System",
            BatchErrorCategory.ExternalService => "External Service",
            BatchErrorCategory.InvalidArgument => "Invalid Argument",
            _ => Category.ToString(),
        };
    }
}
