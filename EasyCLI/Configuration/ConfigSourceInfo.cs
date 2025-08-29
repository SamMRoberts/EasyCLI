namespace EasyCLI.Configuration
{
    /// <summary>
    /// Provides detailed information about configuration sources and their status.
    /// </summary>
    public class ConfigSourceInfo
    {
        /// <summary>
        /// Gets the configuration precedence order.
        /// </summary>
        public static string[] PrecedenceOrder =>
        [
            "1. Command-line flags (highest precedence)",
            "2. Environment variables",
            "3. Local project config",
            "4. User config (XDG-compliant)",
            "5. System config (lowest precedence)"
        ];

        /// <summary>
        /// Gets or sets the system configuration file path.
        /// </summary>
        public string SystemPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the system configuration file exists.
        /// </summary>
        public bool SystemExists { get; set; }

        /// <summary>
        /// Gets or sets the user configuration file path (XDG-compliant).
        /// </summary>
        public string UserPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the user configuration file exists.
        /// </summary>
        public bool UserExists { get; set; }

        /// <summary>
        /// Gets or sets the local configuration file path.
        /// </summary>
        public string LocalPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the local configuration file exists.
        /// </summary>
        public bool LocalExists { get; set; }

        /// <summary>
        /// Gets or sets the XDG_CONFIG_HOME environment variable value.
        /// </summary>
        public string? XdgConfigHome { get; set; }
    }
}
