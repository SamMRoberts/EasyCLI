namespace EasyCLI
{
    // Predefined ANSI SGR styles for convenience.
    public static class ConsoleStyles
    {
        // Effects
        public static readonly ConsoleStyle Reset = new(0);
        public static readonly ConsoleStyle Bold = new(1);
        public static readonly ConsoleStyle Dim = new(2);
        public static readonly ConsoleStyle Italic = new(3);
        public static readonly ConsoleStyle Underline = new(4);

        // Foreground basic colors
        public static readonly ConsoleStyle FgBlack = new(30);
        public static readonly ConsoleStyle FgRed = new(31);
        public static readonly ConsoleStyle FgGreen = new(32);
        public static readonly ConsoleStyle FgYellow = new(33);
        public static readonly ConsoleStyle FgBlue = new(34);
        public static readonly ConsoleStyle FgMagenta = new(35);
        public static readonly ConsoleStyle FgCyan = new(36);
        public static readonly ConsoleStyle FgWhite = new(37);

        // Bright foregrounds
        public static readonly ConsoleStyle FgBrightBlack = new(90);
        public static readonly ConsoleStyle FgBrightRed = new(91);
        public static readonly ConsoleStyle FgBrightGreen = new(92);
        public static readonly ConsoleStyle FgBrightYellow = new(93);
        public static readonly ConsoleStyle FgBrightBlue = new(94);
        public static readonly ConsoleStyle FgBrightMagenta = new(95);
        public static readonly ConsoleStyle FgBrightCyan = new(96);
        public static readonly ConsoleStyle FgBrightWhite = new(97);

        // Semantic styles
        public static readonly ConsoleStyle Success = new(32);        // green
        public static readonly ConsoleStyle Warning = new(33);        // yellow
        public static readonly ConsoleStyle Error = new(91);          // bright red
        public static readonly ConsoleStyle Heading = new(1, 36);     // bold + cyan
        public static readonly ConsoleStyle Info = new(36);           // cyan
        public static readonly ConsoleStyle Hint = new(90);           // bright black
    }
}
