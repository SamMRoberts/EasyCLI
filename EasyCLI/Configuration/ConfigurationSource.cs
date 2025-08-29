namespace EasyCLI.Configuration
{
    /// <summary>
    /// Tracks the source of configuration values for debugging and help.
    /// </summary>
    public class ConfigurationSource
    {
        /// <summary>
        /// Gets or sets the source of the API URL setting.
        /// </summary>
        public string ApiUrlSource { get; set; } = "default";

        /// <summary>
        /// Gets or sets the source of the timeout setting.
        /// </summary>
        public string TimeoutSource { get; set; } = "default";

        /// <summary>
        /// Gets or sets the source of the enable logging setting.
        /// </summary>
        public string EnableLoggingSource { get; set; } = "default";

        /// <summary>
        /// Gets or sets the source of the log level setting.
        /// </summary>
        public string LogLevelSource { get; set; } = "default";

        /// <summary>
        /// Gets or sets the source of the output format setting.
        /// </summary>
        public string OutputFormatSource { get; set; } = "default";

        /// <summary>
        /// Gets or sets the source of the use colors setting.
        /// </summary>
        public string UseColorsSource { get; set; } = "default";

        /// <summary>
        /// Gets or sets the source of the telemetry consent setting.
        /// </summary>
        public string TelemetryConsentSource { get; set; } = "default";
    }
}
