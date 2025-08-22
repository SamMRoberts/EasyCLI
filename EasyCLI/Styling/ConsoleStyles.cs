namespace EasyCLI.Styling
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
        public static string Red(string s)
        {
            return new ConsoleStyle(31).Apply(s);
        }

        public static string BoldText(string s)
        {
            return new ConsoleStyle(1).Apply(s);
        }

        // Truecolor helpers
        public static ConsoleStyle TrueColor(int r, int g, int b)
        {
            return new ConsoleStyle(38, 2, r, g, b);
        }

        public static ConsoleStyle TrueBg(int r, int g, int b)
        {
            return new ConsoleStyle(48, 2, r, g, b);
        }

        public static ConsoleStyle Indexed256(int index)
        {
            if (index is < 0 or > 255)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return new ConsoleStyle(38, 5, index);
        }

#if DEBUG
        public static void PrintPalette(IConsoleWriter w)
        {
            ArgumentNullException.ThrowIfNull(w);
            w.WriteLine("ANSI Palette:");
            w.WriteLine("Normal colors:");
            w.WriteLine(FgBlack.Apply(" black ") + " " +
                FgRed.Apply(" red ") + " " +
                FgGreen.Apply(" green ") + " " +
                FgYellow.Apply(" yellow ") + " " +
                FgBlue.Apply(" blue ") + " " +
                FgMagenta.Apply(" magenta ") + " " +
                FgCyan.Apply(" cyan ") + " " +
                FgWhite.Apply(" white "));

            w.WriteLine("Bright colors:");
            w.WriteLine(FgBrightBlack.Apply(" bright black ") + " " +
                FgBrightRed.Apply(" bright red ") + " " +
                FgBrightGreen.Apply(" bright green ") + " " +
                FgBrightYellow.Apply(" bright yellow ") + " " +
                FgBrightBlue.Apply(" bright blue ") + " " +
                FgBrightMagenta.Apply(" bright magenta ") + " " +
                FgBrightCyan.Apply(" bright cyan ") + " " +
                FgBrightWhite.Apply(" bright white "));

            w.WriteLine("Effects:");
            w.WriteLine(Bold.Apply(" bold ") + " " +
                Dim.Apply(" dim ") + " " +
                Italic.Apply(" italic ") + " " +
                Underline.Apply(" underline "));
        }
#endif
    }
}
