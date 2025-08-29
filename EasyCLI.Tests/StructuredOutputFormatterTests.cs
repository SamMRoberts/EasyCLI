using System.Collections.Generic;
using EasyCLI.Formatting;
using EasyCLI.Shell;
using Xunit;

namespace EasyCLI.Tests
{
    public class StructuredOutputFormatterTests
    {
        [Fact]
        public void JsonFormatter_FormatKeyValues_ReturnsValidJson()
        {
            var formatter = new JsonOutputFormatter();
            var items = new[]
            {
                ("key1", "value1"),
                ("key2", "value2"),
            };

            var result = formatter.FormatKeyValues(items);

            Assert.Contains("\"key1\": \"value1\"", result);
            Assert.Contains("\"key2\": \"value2\"", result);
            Assert.StartsWith("{", result.Trim());
            Assert.EndsWith("}", result.Trim());
        }

        [Fact]
        public void JsonFormatter_FormatTable_ReturnsJsonArray()
        {
            var formatter = new JsonOutputFormatter();
            var headers = new[] { "Name", "Age" };
            var rows = new List<IReadOnlyList<string>>
            {
                new[] { "John", "30" },
                new[] { "Jane", "25" },
            };

            var result = formatter.FormatTable(headers, rows);

            Assert.Contains("\"Name\": \"John\"", result);
            Assert.Contains("\"Age\": \"30\"", result);
            Assert.Contains("\"Name\": \"Jane\"", result);
            Assert.Contains("\"Age\": \"25\"", result);
            Assert.StartsWith("[", result.Trim());
            Assert.EndsWith("]", result.Trim());
        }

        [Fact]
        public void JsonFormatter_FormatObject_ReturnsJsonString()
        {
            var formatter = new JsonOutputFormatter();
            var data = new { Name = "Test", Value = 42 };

            var result = formatter.FormatObject(data);

            Assert.Contains("\"Name\": \"Test\"", result);
            Assert.Contains("\"Value\": 42", result);
        }

        [Fact]
        public void PlainFormatter_FormatKeyValues_ReturnsPlainText()
        {
            var formatter = new PlainOutputFormatter();
            var items = new[]
            {
                ("key1", "value1"),
                ("key2", "value2"),
            };

            var result = formatter.FormatKeyValues(items);

            Assert.Contains("key1: value1", result);
            Assert.Contains("key2: value2", result);
            Assert.DoesNotContain("{", result);
            Assert.DoesNotContain("}", result);
        }

        [Fact]
        public void PlainFormatter_FormatTable_ReturnsFormattedTable()
        {
            var formatter = new PlainOutputFormatter();
            var headers = new[] { "Name", "Age" };
            var rows = new List<IReadOnlyList<string>>
            {
                new[] { "John", "30" },
                new[] { "Jane", "25" },
            };

            var result = formatter.FormatTable(headers, rows);

            Assert.Contains("Name", result);
            Assert.Contains("Age", result);
            Assert.Contains("John", result);
            Assert.Contains("Jane", result);
        }

        [Fact]
        public void TableFormatter_FormatKeyValues_UsesConsoleFormatting()
        {
            var formatter = new TableOutputFormatter();
            var items = new[]
            {
                ("key1", "value1"),
                ("key2", "value2"),
            };

            var result = formatter.FormatKeyValues(items);

            Assert.Contains("key1", result);
            Assert.Contains("value1", result);
            Assert.Contains("key2", result);
            Assert.Contains("value2", result);
        }

        [Fact]
        public void TableFormatter_FormatTable_UsesConsoleFormatting()
        {
            var formatter = new TableOutputFormatter();
            var headers = new[] { "Name", "Age" };
            var rows = new List<IReadOnlyList<string>>
            {
                new[] { "John", "30" },
                new[] { "Jane", "25" },
            };

            var result = formatter.FormatTable(headers, rows);

            Assert.Contains("Name", result);
            Assert.Contains("Age", result);
            Assert.Contains("John", result);
            Assert.Contains("Jane", result);
        }

        [Fact]
        public void StructuredOutputFormatterFactory_CreateFormatter_WithJsonFlag_ReturnsJsonFormatter()
        {
            var args = new CommandLineArgs(new[] { "--json" });

            var formatter = StructuredOutputFormatterFactory.CreateFormatter(args);

            Assert.IsType<JsonOutputFormatter>(formatter);
            Assert.Equal("json", formatter.FormatName);
        }

        [Fact]
        public void StructuredOutputFormatterFactory_CreateFormatter_WithPlainFlag_ReturnsPlainFormatter()
        {
            var args = new CommandLineArgs(new[] { "--plain" });

            var formatter = StructuredOutputFormatterFactory.CreateFormatter(args);

            Assert.IsType<PlainOutputFormatter>(formatter);
            Assert.Equal("plain", formatter.FormatName);
        }

        [Fact]
        public void StructuredOutputFormatterFactory_CreateFormatter_WithNoFlags_ReturnsTableFormatter()
        {
            var args = new CommandLineArgs(Array.Empty<string>());

            var formatter = StructuredOutputFormatterFactory.CreateFormatter(args);

            Assert.IsType<TableOutputFormatter>(formatter);
            Assert.Equal("table", formatter.FormatName);
        }

        [Fact]
        public void StructuredOutputFormatterFactory_CreateFormatter_ByName_ReturnsCorrectFormatter()
        {
            var jsonFormatter = StructuredOutputFormatterFactory.CreateFormatter("json");
            var plainFormatter = StructuredOutputFormatterFactory.CreateFormatter("plain");
            var tableFormatter = StructuredOutputFormatterFactory.CreateFormatter("table");
            var defaultFormatter = StructuredOutputFormatterFactory.CreateFormatter("unknown");

            Assert.IsType<JsonOutputFormatter>(jsonFormatter);
            Assert.IsType<PlainOutputFormatter>(plainFormatter);
            Assert.IsType<TableOutputFormatter>(tableFormatter);
            Assert.IsType<TableOutputFormatter>(defaultFormatter);
        }

        [Fact]
        public void StructuredOutputFormatterFactory_GetAvailableFormats_ReturnsExpectedFormats()
        {
            var formats = StructuredOutputFormatterFactory.GetAvailableFormats();

            Assert.Contains("json", formats);
            Assert.Contains("plain", formats);
            Assert.Contains("table", formats);
            Assert.Equal(3, formats.Length);
        }

        [Fact]
        public void CommandLineArgs_IsJsonOutput_ReturnsTrueWhenJsonFlagPresent()
        {
            var argsLong = new CommandLineArgs(new[] { "--json" });
            var argsShort = new CommandLineArgs(new[] { "-j" });

            Assert.True(argsLong.IsJsonOutput);
            Assert.True(argsShort.IsJsonOutput);
        }

        [Fact]
        public void CommandLineArgs_IsPlainOutput_ReturnsTrueWhenPlainFlagPresent()
        {
            var argsLong = new CommandLineArgs(new[] { "--plain" });
            var argsShort = new CommandLineArgs(new[] { "-p" });

            Assert.True(argsLong.IsPlainOutput);
            Assert.True(argsShort.IsPlainOutput);
        }

        [Fact]
        public void Formatters_HandleNullInput_Gracefully()
        {
            var jsonFormatter = new JsonOutputFormatter();
            var plainFormatter = new PlainOutputFormatter();
            var tableFormatter = new TableOutputFormatter();

            Assert.Equal("{}", jsonFormatter.FormatKeyValues(null!));
            Assert.Equal("[]", jsonFormatter.FormatTable(null!, null!));
            Assert.Equal("null", jsonFormatter.FormatObject(null!));

            Assert.Equal(string.Empty, plainFormatter.FormatKeyValues(null!));
            Assert.Equal(string.Empty, plainFormatter.FormatTable(null!, null!));
            Assert.Equal(string.Empty, plainFormatter.FormatObject(null!));

            Assert.Equal(string.Empty, tableFormatter.FormatKeyValues(null!));
            Assert.Equal(string.Empty, tableFormatter.FormatTable(null!, null!));
            Assert.NotNull(tableFormatter.FormatObject(null!));
        }
    }
}
