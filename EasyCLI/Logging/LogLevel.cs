namespace EasyCLI.Logging
{
    /// <summary>
    /// Defines logging levels for CLI applications.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// No output (only errors).
        /// </summary>
        Quiet = 0,

        /// <summary>
        /// Normal output level.
        /// </summary>
        Normal = 1,

        /// <summary>
        /// Verbose output level.
        /// </summary>
        Verbose = 2,

        /// <summary>
        /// Debug output level (includes everything).
        /// </summary>
        Debug = 3,
    }
}
