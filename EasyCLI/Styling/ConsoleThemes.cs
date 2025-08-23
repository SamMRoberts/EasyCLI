namespace EasyCLI.Styling
{
    /// <summary>
    /// Built-in theme presets. You can clone and tweak as needed.
    /// </summary>
    public static class ConsoleThemes
    {
        /// <summary>
        /// Gets a dark theme suitable for dark terminal backgrounds.
        /// </summary>
        public static ConsoleTheme Dark => new()
        {
            Success = ConsoleStyles.FgGreen,
            Warning = ConsoleStyles.FgYellow,
            Error = ConsoleStyles.FgBrightRed,
            Heading = new ConsoleStyle(1, 36), // bold + cyan
            Info = ConsoleStyles.FgCyan,
            Hint = ConsoleStyles.FgBrightBlack,
        };

        /// <summary>
        /// Gets a light theme suitable for light terminal backgrounds.
        /// </summary>
        public static ConsoleTheme Light => new()
        {
            Success = ConsoleStyles.FgGreen,
            Warning = ConsoleStyles.FgBlue,      // blue reads better on light backgrounds than yellow
            Error = ConsoleStyles.FgRed,
            Heading = new ConsoleStyle(1, 35),   // bold + magenta
            Info = ConsoleStyles.FgBlue,
            Hint = ConsoleStyles.FgBlack,
        };

        /// <summary>
        /// Gets a high contrast theme for maximum visibility.
        /// </summary>
        public static ConsoleTheme HighContrast => new()
        {
            Success = ConsoleStyles.FgBrightGreen,
            Warning = ConsoleStyles.FgBrightYellow,
            Error = ConsoleStyles.FgBrightRed,
            Heading = new ConsoleStyle(1, 97),   // bold + bright white
            Info = ConsoleStyles.FgBrightCyan,
            Hint = ConsoleStyles.FgBrightWhite,
        };
    }
}
