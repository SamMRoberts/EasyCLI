namespace EasyCLI
{
    /// <summary>
    /// Represents a summary of errors grouped by category.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ErrorSummary"/> class.
    /// </remarks>
    /// <param name="category">The error category.</param>
    /// <param name="errors">The errors in this category.</param>
        /// <summary>
        /// Gets the error category.
        /// </summary>
        public BatchErrorCategory Category { get; } = category;

        /// <summary>
        /// Gets the errors in this category.
        /// </summary>
        public IReadOnlyList<CollectedError> Errors { get; } = errors ?? throw new ArgumentNullException(nameof(errors));

        /// <summary>
        /// Gets the count of errors in this category.
        /// </summary>
        public int Count { get; } = errors.Count;

        /// <summary>
        /// Gets a display name for the category.
        /// </summary>
        public string CategoryDisplayName => Category switch
        {
            BatchErrorCategory.FileSystem => "File System",
            BatchErrorCategory.ExternalService => "External Service",
            BatchErrorCategory.InvalidArgument => "Invalid Argument",
            BatchErrorCategory.General => "General",
            BatchErrorCategory.Network => "Network",
            BatchErrorCategory.Validation => "Validation",
            BatchErrorCategory.Security => "Security",
            BatchErrorCategory.Configuration => "Configuration",
            _ => Category.ToString(),
        };
    }
}
