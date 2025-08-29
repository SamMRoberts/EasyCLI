using System.Text.Json.Serialization;

namespace EasyCLI.Configuration
{
    /// <summary>
    /// Base configuration class for CLI applications.
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// Gets or sets the API URL for the application.
        /// </summary>
        [JsonPropertyName("api_url")]
        public string ApiUrl { get; set; } = "https://api.example.com";

        /// <summary>
        /// Gets or sets the timeout in seconds for operations.
        /// </summary>
        [JsonPropertyName("timeout")]
        public int Timeout { get; set; } = 30;

        /// <summary>
        /// Gets or sets a value indicating whether logging is enabled.
        /// </summary>
        [JsonPropertyName("enable_logging")]
        public bool EnableLogging { get; set; } = true;

        /// <summary>
        /// Gets or sets the default log level.
        /// </summary>
        [JsonPropertyName("log_level")]
        public string LogLevel { get; set; } = "Info";

        /// <summary>
        /// Gets or sets the output format preference.
        /// </summary>
        [JsonPropertyName("output_format")]
        public string OutputFormat { get; set; } = "console";

        /// <summary>
        /// Gets or sets a value indicating whether colors should be used in output.
        /// </summary>
        [JsonPropertyName("use_colors")]
        public bool UseColors { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the user has consented to telemetry collection.
        /// Telemetry is disabled by default and requires explicit user consent.
        /// </summary>
        [JsonPropertyName("telemetry_consent")]
        public bool TelemetryConsent { get; set; } = false;

        /// <summary>
        /// Gets configuration source information for tracking where values came from.
        /// </summary>
        [JsonIgnore]
        public ConfigurationSource Source { get; internal set; } = new();
    }
}
