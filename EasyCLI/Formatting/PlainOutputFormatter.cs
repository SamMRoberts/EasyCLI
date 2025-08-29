namespace EasyCLI.Formatting
{
    /// <summary>
    /// Formats structured data as plain text without styling or formatting.
    /// </summary>
    public class PlainOutputFormatter : IStructuredOutputFormatter
    {
        /// <summary>
        /// Gets the name of the output format.
        /// </summary>
        public string FormatName => "plain";

        /// <summary>
        /// Formats key-value pairs as plain text lines.
        /// </summary>
        /// <param name="items">The key-value pairs to format.</param>
        /// <returns>Plain text formatted output.</returns>
        public string FormatKeyValues(IEnumerable<(string key, string value)> items)
        {
            if (items == null)
            {
                return string.Empty;
            }

            StringBuilder sb = new();
            foreach ((string key, string value) in items)
            {
                _ = sb.AppendLine(System.Globalization.CultureInfo.InvariantCulture, $"{key}: {value}");
            }

            return sb.ToString().TrimEnd();
        }

        /// <summary>
        /// Formats tabular data as plain text with simple spacing.
        /// </summary>
        /// <param name="headers">The column headers.</param>
        /// <param name="rows">The data rows.</param>
        /// <returns>Plain text formatted output.</returns>
        public string FormatTable(IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<string>> rows)
        {
            if (headers == null || !headers.Any())
            {
                return string.Empty;
            }

            StringBuilder sb = new();
            List<IReadOnlyList<string>> rowList = rows?.ToList() ?? [];

            // Calculate column widths
            int[] columnWidths = new int[headers.Count];
            for (int i = 0; i < headers.Count; i++)
            {
                columnWidths[i] = headers[i]?.Length ?? 0;
            }

            foreach (IReadOnlyList<string>? row in rowList)
            {
                for (int i = 0; i < headers.Count && i < (row?.Count ?? 0); i++)
                {
                    string cellValue = row?[i] ?? string.Empty;
                    if (cellValue.Length > columnWidths[i])
                    {
                        columnWidths[i] = cellValue.Length;
                    }
                }
            }

            // Format headers
            for (int i = 0; i < headers.Count; i++)
            {
                if (i > 0)
                {
                    _ = sb.Append("  ");
                }

                _ = sb.Append((headers[i] ?? string.Empty).PadRight(columnWidths[i]));
            }
            _ = sb.AppendLine();

            // Format rows
            foreach (IReadOnlyList<string>? row in rowList)
            {
                for (int i = 0; i < headers.Count; i++)
                {
                    if (i > 0)
                    {
                        _ = sb.Append("  ");
                    }

                    string cellValue = (i < (row?.Count ?? 0)) ? (row?[i] ?? string.Empty) : string.Empty;
                    _ = sb.Append(cellValue.PadRight(columnWidths[i]));
                }
                _ = sb.AppendLine();
            }

            return sb.ToString().TrimEnd();
        }

        /// <summary>
        /// Formats an object as plain text by converting to key-value pairs.
        /// </summary>
        /// <param name="data">The object to format.</param>
        /// <returns>Plain text formatted output.</returns>
        public string FormatObject(object data)
        {
            if (data == null)
            {
                return string.Empty;
            }

            // Try to convert to key-value pairs using reflection
            System.Reflection.PropertyInfo[] properties = data.GetType().GetProperties();
            IEnumerable<(string Name, string value)> keyValues = properties.Select(prop =>
            {
                string value = prop.GetValue(data)?.ToString() ?? string.Empty;
                return (prop.Name, value);
            });

            if (keyValues.Any())
            {
                return FormatKeyValues(keyValues);
            }

            // Fallback to ToString
            return data.ToString() ?? string.Empty;
        }
    }
}
