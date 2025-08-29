namespace EasyCLI
{
    /// <summary>
    /// Represents a category for error classification in batch operations.
    /// </summary>
    public enum BatchErrorCategory
    {
        /// <summary>General application errors.</summary>
        General,

        /// <summary>File system related errors.</summary>
        FileSystem,

        /// <summary>Network and connectivity errors.</summary>
        Network,

        /// <summary>Input validation errors.</summary>
        Validation,

        /// <summary>Invalid argument errors.</summary>
        InvalidArgument,

        /// <summary>Permission and security errors.</summary>
        Security,

        /// <summary>Configuration related errors.</summary>
        Configuration,

        /// <summary>External service errors.</summary>
        ExternalService,
    }
}
