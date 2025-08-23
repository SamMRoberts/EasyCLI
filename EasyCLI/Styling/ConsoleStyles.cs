using EasyCLI.Console;

namespace EasyCLI.Styling
{
    /// <summary>
    /// Predefined ANSI SGR styles for convenience.
    /// </summary>
    public static class ConsoleStyles
    {
        // Effects

        /// <summary>Reset all formatting to default.</summary>
        public static readonly ConsoleStyle Reset = new(0);

        /// <summary>Bold text.</summary>
        public static readonly ConsoleStyle Bold = new(1);

        /// <summary>Dim (faint) text.</summary>
        public static readonly ConsoleStyle Dim = new(2);

        /// <summary>Italic text.</summary>
        public static readonly ConsoleStyle Italic = new(3);

        /// <summary>Underlined text.</summary>
        public static readonly ConsoleStyle Underline = new(4);

        // Foreground basic colors

        /// <summary>Black foreground color.</summary>
        public static readonly ConsoleStyle FgBlack = new(30);

        /// <summary>Red foreground color.</summary>
        public static readonly ConsoleStyle FgRed = new(31);

        /// <summary>Green foreground color.</summary>
        public static readonly ConsoleStyle FgGreen = new(32);

        /// <summary>Yellow foreground color.</summary>
        public static readonly ConsoleStyle FgYellow = new(33);

        /// <summary>Blue foreground color.</summary>
        public static readonly ConsoleStyle FgBlue = new(34);

        /// <summary>Magenta foreground color.</summary>
        public static readonly ConsoleStyle FgMagenta = new(35);

        /// <summary>Cyan foreground color.</summary>
        public static readonly ConsoleStyle FgCyan = new(36);

        /// <summary>White foreground color.</summary>
        public static readonly ConsoleStyle FgWhite = new(37);

        // Bright foregrounds

        /// <summary>Bright black foreground color.</summary>
        public static readonly ConsoleStyle FgBrightBlack = new(90);

        /// <summary>Bright red foreground color.</summary>
        public static readonly ConsoleStyle FgBrightRed = new(91);

        /// <summary>Bright green foreground color.</summary>
        public static readonly ConsoleStyle FgBrightGreen = new(92);

        /// <summary>Bright yellow foreground color.</summary>
        public static readonly ConsoleStyle FgBrightYellow = new(93);

        /// <summary>Bright blue foreground color.</summary>
        public static readonly ConsoleStyle FgBrightBlue = new(94);

        /// <summary>Bright magenta foreground color.</summary>
        public static readonly ConsoleStyle FgBrightMagenta = new(95);

        /// <summary>Bright cyan foreground color.</summary>
        public static readonly ConsoleStyle FgBrightCyan = new(96);

        /// <summary>Bright white foreground color.</summary>
        public static readonly ConsoleStyle FgBrightWhite = new(97);

        // Semantic styles

        /// <summary>Style for success messages (green).</summary>
        public static readonly ConsoleStyle Success = new(32);        // green

        /// <summary>Style for warning messages (yellow).</summary>
        public static readonly ConsoleStyle Warning = new(33);        // yellow

        /// <summary>Style for error messages (bright red).</summary>
        public static readonly ConsoleStyle Error = new(91);          // bright red

        /// <summary>Style for heading text (bold cyan).</summary>
        public static readonly ConsoleStyle Heading = new(1, 36);     // bold + cyan

        /// <summary>Style for informational messages (cyan).</summary>
        public static readonly ConsoleStyle Info = new(36);           // cyan

        /// <summary>Style for hint messages (bright black).</summary>
        public static readonly ConsoleStyle Hint = new(90);           // bright black

        // Helper functions mirroring examples in the checklist

        /// <summary>
        /// Applies red foreground color to the specified text.
        /// </summary>
        /// <param name="s">The text to style.</param>
        /// <returns>The text with red foreground color applied.</returns>
        public static string Red(string s)
        {
            return new ConsoleStyle(31).Apply(s);
        }

        /// <summary>
        /// Applies bold formatting to the specified text.
        /// </summary>
        /// <param name="s">The text to style.</param>
        /// <returns>The text with bold formatting applied.</returns>
        public static string BoldText(string s)
        {
            return new ConsoleStyle(1).Apply(s);
        }

        // Truecolor helpers

        /// <summary>
        /// Creates a TrueColor (24-bit) foreground style with the specified RGB values.
        /// </summary>
        /// <param name="r">The red component (0-255).</param>
        /// <param name="g">The green component (0-255).</param>
        /// <param name="b">The blue component (0-255).</param>
        /// <returns>A console style with the specified TrueColor foreground.</returns>
        public static ConsoleStyle TrueColor(int r, int g, int b)
        {
            return new ConsoleStyle(38, 2, r, g, b);
        }

        /// <summary>
        /// Creates a TrueColor (24-bit) background style with the specified RGB values.
        /// </summary>
        /// <param name="r">The red component (0-255).</param>
        /// <param name="g">The green component (0-255).</param>
        /// <param name="b">The blue component (0-255).</param>
        /// <returns>A console style with the specified TrueColor background.</returns>
        public static ConsoleStyle TrueBg(int r, int g, int b)
        {
            return new ConsoleStyle(48, 2, r, g, b);
        }

        /// <summary>
        /// Creates a 256-color indexed foreground style with the specified color index.
        /// </summary>
        /// <param name="index">The color index (0-255).</param>
        /// <returns>A console style with the specified indexed color foreground.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when index is not between 0 and 255.</exception>
        public static ConsoleStyle Indexed256(int index)
        {
            if (index is < 0 or > 255)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return new ConsoleStyle(38, 5, index);
        }

#if DEBUG
        /// <summary>
        /// Prints a color palette demonstration to the specified console writer. Only available in DEBUG builds.
        /// </summary>
        /// <param name="w">The console writer to print to.</param>
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
