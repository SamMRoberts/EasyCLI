using System.IO;
using EasyCLI.Console;
using EasyCLI.Extensions;
using EasyCLI.Styling;
using Xunit;

namespace EasyCLI.Tests
{
    /// <summary>
    /// Golden/snapshot tests for theme-based output consistency.
    /// These tests ensure that different themes produce consistent styling patterns.
    /// </summary>
    public class ThemeOutputSnapshotTests
    {
        [Fact]
        public void DarkTheme_ProducesExpectedAnsiCodes()
        {
            // Arrange
            var output = new StringWriter();
            var writer = new ConsoleWriter(enableColors: true, output: output);
            var theme = ConsoleThemes.Dark;

            // Act
            writer.WriteSuccessLine("Success message", theme);
            writer.WriteWarningLine("Warning message", theme);
            writer.WriteErrorLine("Error message", theme);
            writer.WriteInfoLine("Info message", theme);
            writer.WriteHeadingLine("Heading", theme);

            var result = output.ToString();

            // Assert - Verify Dark theme ANSI codes
            Assert.Contains("\u001b[92m", result); // Bright green for success
            Assert.Contains("\u001b[93m", result); // Bright yellow for warning
            Assert.Contains("\u001b[91m", result); // Bright red for error
            Assert.Contains("\u001b[96m", result); // Bright cyan for info
            Assert.Contains("\u001b[1;36m", result); // Bold cyan for heading
        }

        [Fact]
        public void LightTheme_ProducesExpectedAnsiCodes()
        {
            // Arrange
            var output = new StringWriter();
            var writer = new ConsoleWriter(enableColors: true, output: output);
            var theme = ConsoleThemes.Light;

            // Act
            writer.WriteSuccessLine("Success message", theme);
            writer.WriteWarningLine("Warning message", theme);
            writer.WriteErrorLine("Error message", theme);
            writer.WriteInfoLine("Info message", theme);
            writer.WriteHeadingLine("Heading", theme);

            var result = output.ToString();

            // Assert - Verify Light theme ANSI codes
            Assert.Contains("\u001b[32m", result); // Green for success
            Assert.Contains("\u001b[34m", result); // Blue for warning (different from dark)
            Assert.Contains("\u001b[31m", result); // Red for error
            Assert.Contains("\u001b[36m", result); // Cyan for info
            Assert.Contains("\u001b[1;35m", result); // Bold magenta for heading
        }

        [Fact]
        public void HighContrastTheme_ProducesExpectedAnsiCodes()
        {
            // Arrange
            var output = new StringWriter();
            var writer = new ConsoleWriter(enableColors: true, output: output);
            var theme = ConsoleThemes.HighContrast;

            // Act
            writer.WriteSuccessLine("Success message", theme);
            writer.WriteWarningLine("Warning message", theme);
            writer.WriteErrorLine("Error message", theme);
            writer.WriteInfoLine("Info message", theme);
            writer.WriteHeadingLine("Heading", theme);

            var result = output.ToString();

            // Assert - Verify HighContrast theme ANSI codes
            Assert.Contains("\u001b[92m", result); // Bright green for success
            Assert.Contains("\u001b[93m", result); // Bright yellow for warning
            Assert.Contains("\u001b[91m", result); // Bright red for error
            Assert.Contains("\u001b[97m", result); // Bright white for info
            Assert.Contains("\u001b[1;97m", result); // Bold bright white for heading
        }

        [Fact]
        public void AllThemes_ProduceConsistentStructure()
        {
            // Arrange
            var themes = new[] { ConsoleThemes.Dark, ConsoleThemes.Light, ConsoleThemes.HighContrast };
            var expectedContent = new[] { "Success", "Warning", "Error", "Info", "Heading" };

            foreach (var theme in themes)
            {
                var output = new StringWriter();
                var writer = new ConsoleWriter(enableColors: true, output: output);

                // Act
                writer.WriteSuccessLine("Success", theme);
                writer.WriteWarningLine("Warning", theme);
                writer.WriteErrorLine("Error", theme);
                writer.WriteInfoLine("Info", theme);
                writer.WriteHeadingLine("Heading", theme);

                var result = output.ToString();

                // Assert - All themes should contain the same text content
                foreach (var content in expectedContent)
                {
                    Assert.Contains(content, result);
                }

                // Should have ANSI codes when colors are enabled
                Assert.Contains("\u001b[", result);
                Assert.Contains("\u001b[0m", result); // Reset codes
            }
        }

        [Fact]
        public void PlainOutput_NoAnsiCodes_AllThemes()
        {
            // Arrange
            var themes = new[] { ConsoleThemes.Dark, ConsoleThemes.Light, ConsoleThemes.HighContrast };

            foreach (var theme in themes)
            {
                var output = new StringWriter();
                var writer = new ConsoleWriter(enableColors: false, output: output); // Colors disabled

                // Act
                writer.WriteSuccessLine("Success", theme);
                writer.WriteWarningLine("Warning", theme);
                writer.WriteErrorLine("Error", theme);
                writer.WriteInfoLine("Info", theme);
                writer.WriteHeadingLine("Heading", theme);

                var result = output.ToString();

                // Assert - Should contain content but no ANSI codes
                Assert.Contains("Success", result);
                Assert.Contains("Warning", result);
                Assert.Contains("Error", result);
                Assert.Contains("Info", result);
                Assert.Contains("Heading", result);

                // Should not have ANSI codes when colors are disabled
                Assert.DoesNotContain("\u001b[", result);
            }
        }

        [Fact]
        public void KeyValueOutput_ConsistentAcrossThemes()
        {
            // Arrange
            var themes = new[] { ConsoleThemes.Dark, ConsoleThemes.Light, ConsoleThemes.HighContrast };
            var keyValues = new[]
            {
                ("Status", "Running"),
                ("Uptime", "2h 35m"),
                ("Memory", "512MB"),
            };

            foreach (var theme in themes)
            {
                var output = new StringWriter();
                var writer = new ConsoleWriter(enableColors: true, output: output);

                // Act
                writer.WriteKeyValues(keyValues, keyStyle: theme.Info, valueStyle: theme.Hint);

                var result = output.ToString();

                // Assert - Should contain all key-value pairs
                Assert.Contains("Status", result);
                Assert.Contains("Running", result);
                Assert.Contains("Uptime", result);
                Assert.Contains("2h 35m", result);
                Assert.Contains("Memory", result);
                Assert.Contains("512MB", result);
            }
        }

        [Fact]
        public void BoxedOutput_ConsistentAcrossThemes()
        {
            // Arrange
            var themes = new[] { ConsoleThemes.Dark, ConsoleThemes.Light, ConsoleThemes.HighContrast };

            foreach (var theme in themes)
            {
                var output = new StringWriter();
                var writer = new ConsoleWriter(enableColors: true, output: output);

                // Act
                writer.WriteTitledBox(new[] { "Test content inside the box" }, "Test Title", theme.Heading, theme.Heading, theme.Info);

                var result = output.ToString();

                // Assert - Should contain box structure and content
                Assert.Contains("Test Title", result);
                Assert.Contains("Test content inside the box", result);
                Assert.Contains("┌", result); // Box top-left corner
                Assert.Contains("┐", result); // Box top-right corner
                Assert.Contains("└", result); // Box bottom-left corner
                Assert.Contains("┘", result); // Box bottom-right corner
                Assert.Contains("─", result); // Horizontal lines
                Assert.Contains("│", result); // Vertical lines
            }
        }

        [Fact]
        public void TableOutput_ConsistentFormattingAcrossThemes()
        {
            // Arrange
            var themes = new[] { ConsoleThemes.Dark, ConsoleThemes.Light, ConsoleThemes.HighContrast };
            var headers = new[] { "Name", "Status", "Port" };
            var rows = new[]
            {
                new[] { "web", "running", "8080" },
                new[] { "api", "running", "8081" },
            };

            foreach (var theme in themes)
            {
                var output = new StringWriter();
                var writer = new ConsoleWriter(enableColors: true, output: output);

                // Act
                writer.WriteTableSimple(headers, rows, headerStyle: theme.Heading, borderStyle: theme.Hint);

                var result = output.ToString();

                // Assert - Should contain table structure and content
                Assert.Contains("Name", result);
                Assert.Contains("Status", result);
                Assert.Contains("Port", result);
                Assert.Contains("web", result);
                Assert.Contains("running", result);
                Assert.Contains("8080", result);
                Assert.Contains("+", result); // Table borders
                Assert.Contains("|", result); // Column separators
            }
        }

        [Fact]
        public void CustomTheme_CanOverrideDefaults()
        {
            // Arrange
            var customTheme = new ConsoleTheme
            {
                Success = new ConsoleStyle(42), // Custom green
                Error = new ConsoleStyle(41),   // Custom red
                Heading = new ConsoleStyle(44), // Custom blue
            };

            var output = new StringWriter();
            var writer = new ConsoleWriter(enableColors: true, output: output);

            // Act
            writer.WriteSuccessLine("Custom success", customTheme);
            writer.WriteErrorLine("Custom error", customTheme);
            writer.WriteHeadingLine("Custom heading", customTheme);

            var result = output.ToString();

            // Assert - Should use custom ANSI codes
            Assert.Contains("\u001b[42m", result); // Custom success color
            Assert.Contains("\u001b[41m", result); // Custom error color
            Assert.Contains("\u001b[44m", result); // Custom heading color
            Assert.Contains("Custom success", result);
            Assert.Contains("Custom error", result);
            Assert.Contains("Custom heading", result);
        }

        [Fact]
        public void ThemeConsistency_AcrossMultipleOperations()
        {
            // Arrange
            var theme = ConsoleThemes.Dark;
            var output = new StringWriter();
            var writer = new ConsoleWriter(enableColors: true, output: output);

            // Act - Multiple operations with the same theme
            writer.WriteSuccessLine("Operation 1 completed", theme);
            writer.WriteInfoLine("Processing step 2", theme);
            writer.WriteWarningLine("Non-critical issue detected", theme);
            writer.WriteSuccessLine("Operation 2 completed", theme);
            writer.WriteInfoLine("All operations finished", theme);

            var result = output.ToString();

            // Assert - Should maintain consistent styling
            var successMatches = System.Text.RegularExpressions.Regex.Matches(result, @"\u001b\[92m");
            var infoMatches = System.Text.RegularExpressions.Regex.Matches(result, @"\u001b\[96m");
            var warningMatches = System.Text.RegularExpressions.Regex.Matches(result, @"\u001b\[93m");

            Assert.Equal(2, successMatches.Count); // Two success messages
            Assert.Equal(2, infoMatches.Count);    // Two info messages
            Assert.Equal(1, warningMatches.Count); // One warning message
        }
    }
}