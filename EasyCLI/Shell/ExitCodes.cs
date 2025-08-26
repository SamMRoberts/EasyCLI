namespace EasyCLI.Shell
{
    /// <summary>
    /// Standard exit codes for CLI applications.
    /// </summary>
    public static class ExitCodes
    {
        /// <summary>
        /// Command completed successfully.
        /// </summary>
        public const int Success = 0;

        /// <summary>
        /// General error occurred.
        /// </summary>
        public const int GeneralError = 1;

        /// <summary>
        /// File or directory not found.
        /// </summary>
        public const int FileNotFound = 2;

        /// <summary>
        /// Permission denied.
        /// </summary>
        public const int PermissionDenied = 3;

        /// <summary>
        /// Invalid command line arguments.
        /// </summary>
        public const int InvalidArguments = 4;

        /// <summary>
        /// Service or resource unavailable.
        /// </summary>
        public const int ServiceUnavailable = 5;

        /// <summary>
        /// User cancelled the operation.
        /// </summary>
        public const int UserCancelled = 6;

        /// <summary>
        /// Configuration error.
        /// </summary>
        public const int ConfigurationError = 7;

        /// <summary>
        /// Network error.
        /// </summary>
        public const int NetworkError = 8;

        /// <summary>
        /// Command not found (used for external process execution).
        /// </summary>
        public const int CommandNotFound = 127;
    }
}
