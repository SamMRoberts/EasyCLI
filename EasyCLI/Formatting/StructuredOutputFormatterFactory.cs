namespace EasyCLI.Formatting
{
    /// <summary>
    /// Factory for creating structured output formatters.
    /// </summary>
    public static class StructuredOutputFormatterFactory
    {
        /// <summary>
        /// Creates an output formatter based on command line arguments.
        /// </summary>
        /// <param name="args">The command line arguments to determine output format.</param>
        /// <returns>The appropriate formatter for the requested output format.</returns>
        public static IStructuredOutputFormatter CreateFormatter(Shell.CommandLineArgs args)
        {
            ArgumentNullException.ThrowIfNull(args);

            if (args.IsJsonOutput)
            {
                return new JsonOutputFormatter();
            }

            if (args.IsPlainOutput)
            {
                return new PlainOutputFormatter();
            }

            // Default to table format
            return new TableOutputFormatter();
        }

        /// <summary>
        /// Creates an output formatter by format name.
        /// </summary>
        /// <param name="formatName">The name of the format (json, plain, table).</param>
        /// <returns>The appropriate formatter for the requested format.</returns>
        public static IStructuredOutputFormatter CreateFormatter(string formatName)
        {
            return formatName?.ToLowerInvariant() switch
            {
                "json" => new JsonOutputFormatter(),
                "plain" => new PlainOutputFormatter(),
                "table" => new TableOutputFormatter(),
                _ => new TableOutputFormatter(), // Default
            };
        }

        /// <summary>
        /// Gets all available format names.
        /// </summary>
        /// <returns>An array of available format names.</returns>
        public static string[] GetAvailableFormats()
        {
            return ["json", "plain", "table",];
        }
    }
}
