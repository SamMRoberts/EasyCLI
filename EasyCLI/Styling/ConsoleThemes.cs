namespace EasyCLI.Styling
{
    /// <summary>
    /// Built-in theme presets. You can clone and tweak as needed.
    /// </summary>
    public static class ConsoleThemes
    {
        public static ConsoleTheme Dark => new()
        {
            Success = ConsoleStyles.FgGreen,
            Warning = ConsoleStyles.FgYellow,
            Error = ConsoleStyles.FgBrightRed,
            Heading = new ConsoleStyle(1, 36), // bold + cyan
            Info = ConsoleStyles.FgCyan,
            Hint = ConsoleStyles.FgBrightBlack
        };

        public static ConsoleTheme Light => new()
        {
            Success = ConsoleStyles.FgGreen,
            Warning = ConsoleStyles.FgBlue,      // blue reads better on light backgrounds than yellow
            Error = ConsoleStyles.FgRed,
            Heading = new ConsoleStyle(1, 35),   // bold + magenta
            Info = ConsoleStyles.FgBlue,
            Hint = ConsoleStyles.FgBlack
        };

        public static ConsoleTheme HighContrast => new()
        {
            Success = ConsoleStyles.FgBrightGreen,
            Warning = ConsoleStyles.FgBrightYellow,
            Error = ConsoleStyles.FgBrightRed,
            Heading = new ConsoleStyle(1, 97),   // bold + bright white
            Info = ConsoleStyles.FgBrightCyan,
            Hint = ConsoleStyles.FgBrightWhite
        };
    }
}
