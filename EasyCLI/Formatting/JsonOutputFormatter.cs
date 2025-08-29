using System.Text.Json;

namespace EasyCLI.Formatting
{
    /// <summary>
    /// Formats structured data as JSON.
    /// </summary>
    public class JsonOutputFormatter : IStructuredOutputFormatter
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        /// <summary>
        /// Gets the name of the output format.
        /// </summary>
        public string FormatName => "json";

        /// <summary>
        /// Formats key-value pairs as JSON.
        /// </summary>
        /// <param name="items">The key-value pairs to format.</param>
        /// <returns>JSON formatted output.</returns>
        public string FormatKeyValues(IEnumerable<(string key, string value)> items)
        {
            if (items == null)
            {
                return "{}";
            }

            Dictionary<string, string> dictionary = items.ToDictionary(
                item => item.key ?? string.Empty,
                item => item.value ?? string.Empty);

            return JsonSerializer.Serialize(dictionary, JsonOptions);
        }

        /// <summary>
        /// Formats tabular data as JSON array of objects.
        /// </summary>
        /// <param name="headers">The column headers.</param>
        /// <param name="rows">The data rows.</param>
        /// <returns>JSON formatted output.</returns>
        public string FormatTable(IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<string>> rows)
        {
            if (headers == null || !headers.Any())
            {
                return "[]";
            }

            List<IReadOnlyList<string>> rowList = rows?.ToList() ?? [];
            List<Dictionary<string, string>> result = [];

            foreach (IReadOnlyList<string>? row in rowList)
            {
                Dictionary<string, string> rowDict = [];
                for (int i = 0; i < headers.Count && i < (row?.Count ?? 0); i++)
                {
                    rowDict[headers[i]] = row?[i] ?? string.Empty;
                }
                result.Add(rowDict);
            }

            return JsonSerializer.Serialize(result, JsonOptions);
        }

        /// <summary>
        /// Formats an object as JSON.
        /// </summary>
        /// <param name="data">The object to format.</param>
        /// <returns>JSON formatted output.</returns>
        public string FormatObject(object data)
        {
            return data == null ? "null" : JsonSerializer.Serialize(data, JsonOptions);
        }
    }
}
