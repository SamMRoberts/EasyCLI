using EasyCLI.Extensions;
using EasyCLI.Shell;
using EasyCLI.Telemetry;
using EasyCLI.Styling;

namespace EasyCLI.Demo.Enhanced
{
    /// <summary>
    /// Demo command showing telemetry and privacy functionality.
    /// </summary>
    public class PrivacyCommand : ICliCommand
    {
        public string Name => "privacy";
        public string Description => "Display privacy information and telemetry status";
        public string Category => "Demo";

        public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(args);
            
            var theme = ConsoleThemes.Dark;
            
            // Show current telemetry status
            var telemetryService = new NoOpTelemetryService(); // Default service
            var status = PrivacyNotices.GetTelemetryStatus(
                telemetryService.IsEnabled, 
                telemetryService.HasUserConsent);
                
            context.Writer.WriteHeadingLine("Privacy & Telemetry Status", theme);
            context.Writer.WriteSuccessLine(status, theme);
            context.Writer.WriteLine(string.Empty);
            
            // Handle arguments
            if (args.Length > 0 && args[0] == "--show-full")
            {
                // Display the full privacy notice
                PrivacyNotices.DisplayPrivacyNotice(context.Writer, theme);
            }
            else
            {
                // Show basic privacy summary
                context.Writer.WriteInfoLine("Basic Privacy Summary:", theme);
                context.Writer.WriteLine("✓ No data collection by default");
                context.Writer.WriteLine("✓ No telemetry without explicit consent");
                context.Writer.WriteLine("✓ Complete local operation");
                context.Writer.WriteLine("✓ Privacy-by-design approach");
                context.Writer.WriteLine(string.Empty);
                
                context.Writer.WriteHintLine("Use 'privacy --show-full' to see detailed privacy information.", theme);
                context.Writer.WriteHintLine("Use '--telemetry-consent' flag to enable optional telemetry.", theme);
                context.Writer.WriteLine(string.Empty);
            }

            return Task.FromResult(0);
        }
    }
}