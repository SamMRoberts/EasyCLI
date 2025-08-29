using System.IO;
using EasyCLI.Console;
using EasyCLI.Extensions;
using EasyCLI.Formatting;
using EasyCLI.Shell;
using EasyCLI.Styling;

namespace EasyCLI
{
    /// <summary>
    /// Demonstration of the PlainConsoleWriter functionality.
    /// This shows how --plain flag affects output by stripping all colors, symbols, and decorative padding.
    /// </summary>
    public static class PlainModeDemo
    {
        public static void RunDemo(bool plainMode = false)
        {
            Console.WriteLine($"\n=== EasyCLI PlainConsoleWriter Demo (Plain Mode: {plainMode}) ===\n");

            // Create console writers
            var baseWriter = new ConsoleWriter(enableColors: !plainMode);
            var writer = plainMode ? new PlainConsoleWriter(baseWriter) : baseWriter;

            // Test styled output
            Console.WriteLine("1. Styled Messages:");
            writer.WriteSuccessLine("✓ Operation completed successfully");
            writer.WriteWarningLine("⚠ This is a warning message");
            writer.WriteErrorLine("✗ An error occurred");
            writer.WriteInfoLine("ℹ Informational message");
            writer.WriteHintLine("💡 Hint: Try using --help for more options");

            Console.WriteLine("\n2. Formatted Output:");

            // Test key-value pairs
            var keyValues = new[]
            {
                ("Server", "api.example.com"),
                ("Status", "✓ Online"),
                ("Uptime", "2d 4h 15m"),
                ("Response Time", "⚡ 45ms")
            };

            writer.WriteKeyValues(keyValues, keyStyle: ConsoleStyles.FgCyan, valueStyle: ConsoleStyles.FgGreen);

            Console.WriteLine("\n3. Table Output:");

            // Test table
            var headers = new[] { "Service", "Status", "Response" };
            var rows = new List<IReadOnlyList<string>>
            {
                new[] { "API", "✓ Healthy", "⚡ 23ms" },
                new[] { "Database", "⚠ Slow", "🐌 156ms" },
                new[] { "Cache", "✗ Error", "❌ Timeout" }
            };

            writer.WriteTableSimple(
                headers,
                rows,
                headerStyle: ConsoleStyles.FgCyan + ConsoleStyles.Bold,
                cellStyle: ConsoleStyles.FgWhite
            );

            Console.WriteLine("\n4. Decorative Elements:");

            // Test rules and boxes
            writer.WriteRule(60, '─', ConsoleStyles.FgBlue);
            writer.WriteTitleRule("System Information", 60, '─', titleStyle: ConsoleStyles.FgYellow + ConsoleStyles.Bold);

            var boxContent = new[]
            {
                "🎯 Goal: Demonstrate plain mode",
                "✨ Features: Color stripping, symbol removal",
                "🚀 Performance: Fast and efficient"
            };

            writer.WriteTitledBox(boxContent, "PlainConsoleWriter Demo",
                borderStyle: ConsoleStyles.FgGreen,
                titleStyle: ConsoleStyles.FgYellow + ConsoleStyles.Bold,
                textStyle: ConsoleStyles.FgWhite);

            Console.WriteLine("\n5. Mixed Decorations:");
            
            // Test complex styled content
            var complexText = ConsoleStyles.FgGreen.Apply("✓ ") + 
                             ConsoleStyles.FgWhite.Apply("Server ") +
                             ConsoleStyles.FgCyan.Apply("api.example.com ") +
                             ConsoleStyles.FgGreen.Apply("is ") +
                             ConsoleStyles.FgYellow.Apply("⚡ online");
            
            writer.WriteLine(complexText);

            Console.WriteLine("\n=== Demo Complete ===\n");
        }

        public static void CompareOutputs()
        {
            Console.WriteLine("=== Styled vs Plain Output Comparison ===\n");

            Console.WriteLine("STYLED OUTPUT:");
            Console.WriteLine("─────────────");
            RunDemo(plainMode: false);

            Console.WriteLine("\nPLAIN OUTPUT:");
            Console.WriteLine("────────────");
            RunDemo(plainMode: true);
        }

        public static void Main(string[] args)
        {
            var arguments = new CommandLineArgs(args);
            
            if (arguments.HasFlag("compare"))
            {
                CompareOutputs();
            }
            else
            {
                bool plainMode = arguments.IsPlainOutput;
                RunDemo(plainMode);
            }
        }
    }
}