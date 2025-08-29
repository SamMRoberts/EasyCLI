using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace EasyCLI.Formatting
{
    /// <summary>
    /// Formats structured data as tables using ConsoleFormatting utilities.
    /// </summary>
    public class TableOutputFormatter : IStructuredOutputFormatter
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        /// <summary>
        /// Gets the name of the output format.
        /// </summary>
        public string FormatName => "table";

        /// <summary>
        /// Formats key-value pairs as a simple two-column table.
        /// </summary>
        /// <param name="items">The key-value pairs to format.</param>
        /// <returns>Table formatted output.</returns>
        public string FormatKeyValues(IEnumerable<(string key, string value)> items)
        {
            if (items == null)
            {
                return string.Empty;
            }

            var lines = ConsoleFormatting.BuildKeyValues(items);
            return string.Join("\n", lines);
        }

        /// <summary>
        /// Formats tabular data as a formatted table.
        /// </summary>
        /// <param name="headers">The column headers.</param>
        /// <param name="rows">The data rows.</param>
        /// <returns>Table formatted output.</returns>
        public string FormatTable(IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<string>> rows)
        {
            if (headers == null || !headers.Any())
            {
                return string.Empty;
            }

            var lines = ConsoleFormatting.BuildSimpleTable(headers, rows ?? Enumerable.Empty<IReadOnlyList<string>>());
            return string.Join("\n", lines);
        }

        /// <summary>
        /// Formats an object as table by converting to key-value pairs or JSON fallback.
        /// </summary>
        /// <param name="data">The object to format.</param>
        /// <returns>Table formatted output.</returns>
        public string FormatObject(object data)
        {
            if (data == null)
            {
                return string.Empty;
            }

            // Try to convert to key-value pairs using reflection
            var properties = data.GetType().GetProperties();
            var keyValues = properties.Select(prop =>
            {
                var value = prop.GetValue(data)?.ToString() ?? string.Empty;
                return (prop.Name, value);
            });

            if (keyValues.Any())
            {
                return FormatKeyValues(keyValues);
            }

            // Fallback to JSON if object structure is complex
            return JsonSerializer.Serialize(data, JsonOptions);
        }
    }
}
