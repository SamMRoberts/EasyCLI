namespace EasyCLI.Styling
{
    /// <summary>
    /// Theme map for semantic styles with easy overrides.
    /// </summary>
    public class ConsoleTheme
    {
        /// <summary>
        /// Gets or sets the style used for success messages.
        /// </summary>
        public ConsoleStyle Success { get; set; } = ConsoleStyles.Success;

        /// <summary>
        /// Gets or sets the style used for warning messages.
        /// </summary>
        public ConsoleStyle Warning { get; set; } = ConsoleStyles.Warning;

        /// <summary>
        /// Gets or sets the style used for error messages.
        /// </summary>
        public ConsoleStyle Error { get; set; } = ConsoleStyles.Error;

        /// <summary>
        /// Gets or sets the style used for heading text.
        /// </summary>
        public ConsoleStyle Heading { get; set; } = ConsoleStyles.Heading;

        /// <summary>
        /// Gets or sets the style used for informational messages.
        /// </summary>
        public ConsoleStyle Info { get; set; } = ConsoleStyles.Info;

        /// <summary>
        /// Gets or sets the style used for hint messages.
        /// </summary>
        public ConsoleStyle Hint { get; set; } = ConsoleStyles.Hint;
    }
}
