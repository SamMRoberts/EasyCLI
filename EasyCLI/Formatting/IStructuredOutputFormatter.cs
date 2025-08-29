namespace EasyCLI.Formatting
{
    /// <summary>
    /// Defines methods for formatting structured data in different output formats.
    /// </summary>
    public interface IStructuredOutputFormatter
    {
        /// <summary>
        /// Gets the name of the output format.
        /// </summary>
        string FormatName { get; }

        /// <summary>
        /// Formats key-value pairs.
        /// </summary>
        /// <param name="items">The key-value pairs to format.</param>
        /// <returns>Formatted output as a string.</returns>
        string FormatKeyValues(IEnumerable<(string key, string value)> items);

        /// <summary>
        /// Formats tabular data with headers and rows.
        /// </summary>
        /// <param name="headers">The column headers.</param>
        /// <param name="rows">The data rows.</param>
        /// <returns>Formatted output as a string.</returns>
        string FormatTable(IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<string>> rows);

        /// <summary>
        /// Formats an object as structured data.
        /// </summary>
        /// <param name="data">The object to format.</param>
        /// <returns>Formatted output as a string.</returns>
        string FormatObject(object data);
    }
}
