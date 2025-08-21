namespace EasyCLI
{
    /// <summary>
    /// Predefined ANSI SGR styles for convenience.
    /// </summary>
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

    // Helper functions mirroring examples in the checklist
    public static string Red(string s) => new ConsoleStyle(31).Apply(s);
    public static string BoldText(string s) => new ConsoleStyle(1).Apply(s);

    // Truecolor helpers
    public static ConsoleStyle TrueColor(int r, int g, int b) => new(38, 2, r, g, b);
    public static ConsoleStyle TrueBg(int r, int g, int b) => new(48, 2, r, g, b);

#if DEBUG
    public static void PrintPalette(IConsoleWriter w)
    {
        w.WriteLine("ANSI Palette:");
        w.WriteLine("Normal colors:");
        w.WriteLine(ConsoleStyles.FgBlack.Apply(" black ") + " " +
            ConsoleStyles.FgRed.Apply(" red ") + " " +
            ConsoleStyles.FgGreen.Apply(" green ") + " " +
            ConsoleStyles.FgYellow.Apply(" yellow ") + " " +
            ConsoleStyles.FgBlue.Apply(" blue ") + " " +
            ConsoleStyles.FgMagenta.Apply(" magenta ") + " " +
            ConsoleStyles.FgCyan.Apply(" cyan ") + " " +
            ConsoleStyles.FgWhite.Apply(" white "));

        w.WriteLine("Bright colors:");
        w.WriteLine(ConsoleStyles.FgBrightBlack.Apply(" bright black ") + " " +
            ConsoleStyles.FgBrightRed.Apply(" bright red ") + " " +
            ConsoleStyles.FgBrightGreen.Apply(" bright green ") + " " +
            ConsoleStyles.FgBrightYellow.Apply(" bright yellow ") + " " +
            ConsoleStyles.FgBrightBlue.Apply(" bright blue ") + " " +
            ConsoleStyles.FgBrightMagenta.Apply(" bright magenta ") + " " +
            ConsoleStyles.FgBrightCyan.Apply(" bright cyan ") + " " +
            ConsoleStyles.FgBrightWhite.Apply(" bright white "));

        w.WriteLine("Effects:");
        w.WriteLine(ConsoleStyles.Bold.Apply(" bold ") + " " +
            ConsoleStyles.Dim.Apply(" dim ") + " " +
            ConsoleStyles.Italic.Apply(" italic ") + " " +
            ConsoleStyles.Underline.Apply(" underline "));
    }
#endif
    }
}
