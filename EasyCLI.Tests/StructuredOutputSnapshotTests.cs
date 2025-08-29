using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using EasyCLI.Formatting;
using Xunit;

namespace EasyCLI.Tests
{
    /// <summary>
    /// Golden/snapshot tests for structured output formats (JSON, plain, table).
    /// These tests ensure stable output contracts for automation and scripting.
    /// </summary>
    public class StructuredOutputSnapshotTests
    {
        [Fact]
        public void JsonFormatter_KeyValues_ProducesStableOutput()
        {
            // Arrange
            var formatter = new JsonOutputFormatter();
            var keyValues = new[]
            {
                ("api_url", "https://api.example.com"),
                ("timeout", "30"),
                ("enable_logging", "true"),
                ("log_level", "Info"),
                ("output_format", "console"),
                ("use_colors", "true"),
            };

            // Act
            var result = formatter.FormatKeyValues(keyValues);

            // Assert - Verify stable JSON structure
            Assert.StartsWith("{", result.Trim());
            Assert.EndsWith("}", result.Trim());

            // Parse to ensure valid JSON
            var parsed = JsonDocument.Parse(result);
            Assert.NotNull(parsed);

            // Verify expected keys are present
            var root = parsed.RootElement;
            Assert.True(root.TryGetProperty("api_url", out var apiUrl));
            Assert.Equal("https://api.example.com", apiUrl.GetString());

            Assert.True(root.TryGetProperty("timeout", out var timeout));
            Assert.Equal("30", timeout.GetString());

            Assert.True(root.TryGetProperty("enable_logging", out var enableLogging));
            Assert.Equal("true", enableLogging.GetString());
        }

        [Fact]
        public void JsonFormatter_Table_ProducesStableArrayOutput()
        {
            // Arrange
            var formatter = new JsonOutputFormatter();
            var headers = new[] { "Name", "Age", "City" };
            var rows = new List<IReadOnlyList<string>>
            {
                new[] { "John", "30", "Seattle" },
                new[] { "Jane", "25", "Portland" },
                new[] { "Bob", "35", "San Francisco" },
            };

            // Act
            var result = formatter.FormatTable(headers, rows);

            // Assert - Verify stable JSON array structure
            Assert.StartsWith("[", result.Trim());
            Assert.EndsWith("]", result.Trim());

            // Parse to ensure valid JSON
            var parsed = JsonDocument.Parse(result);
            Assert.NotNull(parsed);

            // Verify array structure
            var array = parsed.RootElement;
            Assert.Equal(JsonValueKind.Array, array.ValueKind);
            Assert.Equal(3, array.GetArrayLength());

            // Verify first object structure
            var firstObject = array[0];
            Assert.True(firstObject.TryGetProperty("Name", out var name));
            Assert.Equal("John", name.GetString());
            Assert.True(firstObject.TryGetProperty("Age", out var age));
            Assert.Equal("30", age.GetString());
            Assert.True(firstObject.TryGetProperty("City", out var city));
            Assert.Equal("Seattle", city.GetString());
        }

        [Fact]
        public void PlainFormatter_KeyValues_ProducesStableOutput()
        {
            // Arrange
            var formatter = new PlainOutputFormatter();
            var keyValues = new[]
            {
                ("API URL", "https://api.example.com"),
                ("Timeout", "30 seconds"),
                ("Logging", "Enabled"),
            };

            // Act
            var result = formatter.FormatKeyValues(keyValues);

            // Assert - Verify stable plain text format
            Assert.Contains("API URL: https://api.example.com", result);
            Assert.Contains("Timeout: 30 seconds", result);
            Assert.Contains("Logging: Enabled", result);

            // Should not contain JSON or table formatting
            Assert.DoesNotContain("{", result);
            Assert.DoesNotContain("}", result);
            Assert.DoesNotContain("+", result); // No table borders
            Assert.DoesNotContain("|", result); // No table separators
        }

        [Fact]
        public void PlainFormatter_Table_ProducesStableOutput()
        {
            // Arrange
            var formatter = new PlainOutputFormatter();
            var headers = new[] { "Service", "Status", "Port" };
            var rows = new List<IReadOnlyList<string>>
            {
                new[] { "web", "running", "8080" },
                new[] { "api", "running", "8081" },
                new[] { "db", "stopped", "5432" },
            };

            // Act
            var result = formatter.FormatTable(headers, rows);

            // Assert - Verify stable plain text table format
            Assert.Contains("Service", result);
            Assert.Contains("Status", result);
            Assert.Contains("Port", result);
            Assert.Contains("web", result);
            Assert.Contains("running", result);
            Assert.Contains("8080", result);

            // Should not contain JSON formatting
            Assert.DoesNotContain("{", result);
            Assert.DoesNotContain("}", result);
            Assert.DoesNotContain("[", result);
            Assert.DoesNotContain("]", result);
        }

        [Fact]
        public void TableFormatter_ProducesConsistentBorders()
        {
            // Arrange
            var formatter = new TableOutputFormatter();
            var headers = new[] { "Column1", "Column2" };
            var rows = new List<IReadOnlyList<string>>
            {
                new[] { "Value1", "Value2" },
                new[] { "LongerValue", "Short" },
            };

            // Act
            var result = formatter.FormatTable(headers, rows);

            // Assert - Verify table structure
            Assert.Contains("+", result); // Should have table borders
            Assert.Contains("|", result); // Should have column separators
            Assert.Contains("-", result); // Should have horizontal rules
            Assert.Contains("Column1", result);
            Assert.Contains("Column2", result);
            Assert.Contains("Value1", result);
            Assert.Contains("Value2", result);
        }

        [Fact]
        public void AllFormatters_HandleEmptyData_Gracefully()
        {
            // Arrange
            var jsonFormatter = new JsonOutputFormatter();
            var plainFormatter = new PlainOutputFormatter();
            var tableFormatter = new TableOutputFormatter();
            var emptyKeyValues = new (string, string)[0];
            var emptyHeaders = new string[0];
            var emptyRows = new List<IReadOnlyList<string>>();

            // Act & Assert - Should not throw exceptions
            var jsonKeyValues = jsonFormatter.FormatKeyValues(emptyKeyValues);
            var jsonTable = jsonFormatter.FormatTable(emptyHeaders, emptyRows);
            var plainKeyValues = plainFormatter.FormatKeyValues(emptyKeyValues);
            var plainTable = plainFormatter.FormatTable(emptyHeaders, emptyRows);
            var tableResult = tableFormatter.FormatTable(emptyHeaders, emptyRows);

            // Verify they return some output (not null or empty)
            Assert.NotNull(jsonKeyValues);
            Assert.NotNull(jsonTable);
            Assert.NotNull(plainKeyValues);
            Assert.NotNull(plainTable);
            Assert.NotNull(tableResult);
        }

        [Fact]
        public void JsonFormatter_Object_ProducesValidJson()
        {
            // Arrange
            var formatter = new JsonOutputFormatter();
            var testObject = new
            {
                status = "success",
                timestamp = "2024-01-01T00:00:00Z",
                data = new
                {
                    count = 42,
                    items = new[] { "item1", "item2" }
                }
            };

            // Act
            var result = formatter.FormatObject(testObject);

            // Assert - Verify valid JSON
            var parsed = JsonDocument.Parse(result);
            Assert.NotNull(parsed);

            var root = parsed.RootElement;
            Assert.True(root.TryGetProperty("status", out var status));
            Assert.Equal("success", status.GetString());

            Assert.True(root.TryGetProperty("data", out var data));
            Assert.True(data.TryGetProperty("count", out var count));
            Assert.Equal(42, count.GetInt32());
        }

        [Fact]
        public void FormatterFactory_CreatesExpectedFormatters()
        {
            // Arrange & Act
            var jsonFormatter = StructuredOutputFormatterFactory.CreateFormatter("json");
            var plainFormatter = StructuredOutputFormatterFactory.CreateFormatter("plain");
            var tableFormatter = StructuredOutputFormatterFactory.CreateFormatter("table");
            var defaultFormatter = StructuredOutputFormatterFactory.CreateFormatter("unknown");

            // Assert - Verify correct types
            Assert.IsType<JsonOutputFormatter>(jsonFormatter);
            Assert.IsType<PlainOutputFormatter>(plainFormatter);
            Assert.IsType<TableOutputFormatter>(tableFormatter);
            Assert.IsType<TableOutputFormatter>(defaultFormatter); // Default to table
        }

        [Fact]
        public void FormatterFactory_GetAvailableFormats_ReturnsExpectedList()
        {
            // Act
            var formats = StructuredOutputFormatterFactory.GetAvailableFormats();

            // Assert
            Assert.Contains("json", formats);
            Assert.Contains("plain", formats);
            Assert.Contains("table", formats);
            Assert.Equal(3, formats.Length);
        }

        [Fact]
        public void JsonFormatter_SpecialCharacters_HandledCorrectly()
        {
            // Arrange
            var formatter = new JsonOutputFormatter();
            var keyValues = new[]
            {
                ("path", "C:\\Users\\test\\file.txt"),
                ("message", "Line 1\nLine 2"),
                ("quote", "He said \"hello\""),
                ("unicode", "Test 🚀 emoji"),
            };

            // Act
            var result = formatter.FormatKeyValues(keyValues);

            // Assert - Should produce valid JSON despite special characters
            var parsed = JsonDocument.Parse(result);
            Assert.NotNull(parsed);

            var root = parsed.RootElement;
            Assert.True(root.TryGetProperty("path", out var path));
            Assert.Equal("C:\\Users\\test\\file.txt", path.GetString());

            Assert.True(root.TryGetProperty("message", out var message));
            Assert.Equal("Line 1\nLine 2", message.GetString());

            Assert.True(root.TryGetProperty("quote", out var quote));
            Assert.Equal("He said \"hello\"", quote.GetString());

            Assert.True(root.TryGetProperty("unicode", out var unicode));
            Assert.Equal("Test 🚀 emoji", unicode.GetString());
        }
    }
}