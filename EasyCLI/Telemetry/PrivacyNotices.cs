using EasyCLI.Console;

namespace EasyCLI.Telemetry
{
    /// <summary>
    /// Provides privacy-related functionality and disclaimers for telemetry.
    /// </summary>
    public static class PrivacyNotices
    {
        /// <summary>
        /// Gets the privacy disclaimer text for telemetry.
        /// </summary>
        public static string TelemetryDisclaimer =>
            "EasyCLI does not collect any usage data or personal information by default. " +
            "Telemetry is completely disabled unless you explicitly opt-in. " +
            "When enabled, telemetry helps improve the library by collecting anonymous usage statistics.";

        /// <summary>
        /// Gets information about what data would be collected if telemetry were enabled.
        /// </summary>
        public static string[] WhatDataIsCollected =>
        [
            "• Command names and execution status (success/failure)",
            "• Error types and frequencies (no personal data)",
            "• Feature usage patterns (no file names or content)",
            "• Performance metrics (execution times)",
            "• Environment information (OS, .NET version, CI detection)",
        ];

        /// <summary>
        /// Gets information about what data is NOT collected.
        /// </summary>
        public static string[] WhatDataIsNotCollected =>
        [
            "• File names, paths, or file contents",
            "• Personal information or identifiers",
            "• Command arguments or input data",
            "• Configuration values or secrets",
            "• Network requests or API calls",
        ];

        /// <summary>
        /// Displays the telemetry privacy notice using the provided console writer.
        /// </summary>
        /// <param name="writer">The console writer to use for output.</param>
        /// <param name="theme">The console theme for styling.</param>
        public static void DisplayPrivacyNotice(IConsoleWriter writer, ConsoleTheme theme)
        {
            ArgumentNullException.ThrowIfNull(writer);
            ArgumentNullException.ThrowIfNull(theme);

            writer.WriteLine(string.Empty);
            writer.WriteHeadingLine("Privacy Notice", theme);
            writer.WriteLine(TelemetryDisclaimer);
            writer.WriteLine(string.Empty);

            writer.WriteInfoLine("Data collected (if enabled):", theme);
            foreach (string item in WhatDataIsCollected)
            {
                writer.WriteLine($"  {item}");
            }

            writer.WriteLine(string.Empty);

            writer.WriteInfoLine("Data NOT collected:", theme);
            foreach (string item in WhatDataIsNotCollected)
            {
                writer.WriteLine($"  {item}");
            }

            writer.WriteLine(string.Empty);

            writer.WriteHintLine("You can enable telemetry with --telemetry-consent or by setting telemetry_consent: true in your configuration.", theme);
            writer.WriteLine(string.Empty);
        }

        /// <summary>
        /// Gets the current telemetry status message.
        /// </summary>
        /// <param name="isEnabled">Whether telemetry is currently enabled.</param>
        /// <param name="hasConsent">Whether the user has provided consent.</param>
        /// <returns>A status message describing the current telemetry state.</returns>
        public static string GetTelemetryStatus(bool isEnabled, bool hasConsent)
        {
            return !hasConsent
                ? "Telemetry: DISABLED (no consent provided - privacy protected)"
                : isEnabled
                ? "Telemetry: ENABLED (collecting anonymous usage data)"
                : "Telemetry: DISABLED (consent provided but service inactive)";
        }
    }
}
