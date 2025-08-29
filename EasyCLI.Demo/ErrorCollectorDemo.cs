using EasyCLI;
using EasyCLI.Console;
using EasyCLI.Extensions;
using EasyCLI.Formatting;
using EasyCLI.Styling;

namespace EasyCLI.Demo
{
    /// <summary>
    /// Demonstrates ErrorCollector functionality for batch error aggregation and reporting.
    /// </summary>
    public static class ErrorCollectorDemo
    {
        /// <summary>
        /// Runs the ErrorCollector demonstration.
        /// </summary>
        public static void Run()
        {
            var writer = new ConsoleWriter();
            var collector = new ErrorCollector(writer, ConsoleThemes.Dark);

            writer.WriteHeadingLine("ErrorCollector Demo - Batch Error Aggregation", ConsoleThemes.Dark);
            writer.WriteLine();

            // Simulate a batch operation with various errors
            SimulateBatchFileProcessing(collector);

            writer.WriteLine();
            writer.WriteHeadingLine("Error Summary Report", ConsoleThemes.Dark);
            writer.WriteLine();

            // Print summary without details
            collector.PrintSummary(showDetails: false);

            writer.WriteLine();
            writer.WriteTitleRule("Detailed Error Report", filler: '─', titleStyle: ConsoleThemes.Dark.Heading);
            writer.WriteLine();

            // Print summary with full details
            collector.PrintSummary(showDetails: true);

            writer.WriteLine();
            writer.WriteTitleRule("Category-Specific Details", filler: '─', titleStyle: ConsoleThemes.Dark.Heading);
            writer.WriteLine();

            // Show details for specific categories
            collector.PrintCategoryDetails(BatchErrorCategory.FileSystem);
        }

        private static void SimulateBatchFileProcessing(ErrorCollector collector)
        {
            var writer = new ConsoleWriter();
            writer.WriteInfoLine("Simulating batch file processing operation...", ConsoleThemes.Dark);
            
            // Simulate various types of errors that might occur during batch processing
            try
            {
                // File system errors
                throw new FileNotFoundException("Configuration file not found");
            }
            catch (Exception ex)
            {
                collector.AddError(ex, "config.json");
            }

            try
            {
                throw new UnauthorizedAccessException("Permission denied accessing log directory");
            }
            catch (Exception ex)
            {
                collector.AddError(ex, "/var/log/app");
            }

            // Network errors
            try
            {
                throw new HttpRequestException("Failed to connect to API endpoint");
            }
            catch (Exception ex)
            {
                collector.AddError(ex, "https://api.example.com/data");
            }

            // Validation errors
            collector.AddError(BatchErrorCategory.Validation, "Invalid email format", "user@invalid", "Expected format: user@domain.com");
            collector.AddError(BatchErrorCategory.Validation, "Age must be between 0 and 120", "user input: -5");
            collector.AddError(BatchErrorCategory.Validation, "Required field missing", "lastname field");

            // Additional file system errors
            collector.AddError(BatchErrorCategory.FileSystem, "Disk space insufficient", "/tmp", "Required: 500MB, Available: 50MB");
            collector.AddError(BatchErrorCategory.FileSystem, "File is locked by another process", "data.xlsx");

            // External service errors
            collector.AddError(BatchErrorCategory.ExternalService, "Database connection timeout", "postgresql://db:5432", "Timeout after 30 seconds");
            collector.AddError(BatchErrorCategory.ExternalService, "Rate limit exceeded", "API Gateway", "429 Too Many Requests");

            // Configuration errors
            collector.AddError(BatchErrorCategory.Configuration, "Missing required configuration key", "app.settings", "Key: 'DatabaseConnectionString'");

            writer.WriteSuccessLine($"✓ Simulation completed - {collector.TotalCount} errors collected", ConsoleThemes.Dark);
        }
    }
}