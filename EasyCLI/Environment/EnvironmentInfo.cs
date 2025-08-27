namespace EasyCLI.Environment
{
    /// <summary>
    /// Provides information about the current execution environment.
    /// </summary>
    public class EnvironmentInfo
    {
        /// <summary>
        /// Gets or sets a value indicating whether the current directory is a Git repository.
        /// </summary>
        public bool IsGitRepository { get; set; }

        /// <summary>
        /// Gets or sets the current Git branch name.
        /// </summary>
        public string? GitBranch { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether running in a Docker container.
        /// </summary>
        public bool IsDockerEnvironment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether running in a CI environment.
        /// </summary>
        public bool IsContinuousIntegration { get; set; }

        /// <summary>
        /// Gets or sets the CI provider name if detected.
        /// </summary>
        public string? CiProvider { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a configuration file exists.
        /// </summary>
        public bool HasConfigFile { get; set; }

        /// <summary>
        /// Gets or sets the path to the detected configuration file.
        /// </summary>
        public string? ConfigFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the process is running interactively.
        /// </summary>
        public bool IsInteractive { get; set; }

        /// <summary>
        /// Gets or sets the operating system platform.
        /// </summary>
        public string Platform { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets additional environment-specific metadata.
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; } = [];
    }
}
