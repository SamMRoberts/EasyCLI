namespace EasyCLI.Console
{
    /// <summary>
    /// Writes to the console with optional ANSI styling. Honors NO_COLOR/FORCE_COLOR and output redirection.
    /// </summary>
    public class ConsoleWriter : IConsoleWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleWriter"/> class.
        /// </summary>
        /// <param name="enableColors">Explicit override for color support. If null, auto-detects based on environment.</param>
        /// <param name="output">The text writer to use for output. If null, uses <see cref="System.Console.Out"/>.</param>
        public ConsoleWriter(bool? enableColors = null, TextWriter? output = null)
        {
            Output = output ?? System.Console.Out;
            ColorEnabled = DecideColorEnabled(enableColors);
            ColorLevel = ColorEnabled ? DetectColorLevel() : ColorSupportLevel.None;
        }

        /// <summary>
        /// Gets a value indicating whether color output is enabled.
        /// </summary>
        public bool ColorEnabled { get; }

        /// <summary>
        /// Gets the detected color support level of the terminal.
        /// </summary>
        public ColorSupportLevel ColorLevel { get; }

        /// <summary>
        /// Gets the text writer used for output.
        /// </summary>
        public TextWriter Output { get; }

        /// <summary>
        /// Writes the specified message to the console without a newline.
        /// </summary>
        /// <param name="message">The message to write to the console.</param>
        public void Write(string message)
        {
            Output.Write(message);
        }

        /// <summary>
        /// Writes the specified message to the console followed by a newline.
        /// </summary>
        /// <param name="message">The message to write to the console.</param>
        public void WriteLine(string message)
        {
            Output.WriteLine(message);
        }

        /// <summary>
        /// Writes the specified message to the console with ANSI styling applied.
        /// </summary>
        /// <param name="message">The message to write to the console.</param>
        /// <param name="style">The style to apply to the message.</param>
        public void Write(string message, ConsoleStyle style)
        {
            if (ColorEnabled && style.Codes.Length > 0)
            {
                Output.Write(style.Apply(message));
            }
            else
            {
                Output.Write(message);
            }
        }

        /// <summary>
        /// Writes the specified message to the console with ANSI styling applied, followed by a newline.
        /// </summary>
        /// <param name="message">The message to write to the console.</param>
        /// <param name="style">The style to apply to the message.</param>
        public void WriteLine(string message, ConsoleStyle style)
        {
            if (ColorEnabled && style.Codes.Length > 0)
            {
                Output.WriteLine(style.Apply(message));
            }
            else
            {
                Output.WriteLine(message);
            }
        }

        private static bool DecideColorEnabled(bool? enableColors)
        {
            if (enableColors.HasValue)
            {
                return enableColors.Value;
            }

            string? noColor = System.Environment.GetEnvironmentVariable("NO_COLOR");
            if (!string.IsNullOrEmpty(noColor))
            {
                return false;
            }

            string? forceColor = System.Environment.GetEnvironmentVariable("FORCE_COLOR");
            if (!string.IsNullOrEmpty(forceColor))
            {
                return true;
            }

            if (System.Console.IsOutputRedirected)
            {
                return false;
            }

            string? term = System.Environment.GetEnvironmentVariable("TERM");
            return !string.Equals(term, "dumb", StringComparison.OrdinalIgnoreCase);
        }

        private static ColorSupportLevel DetectColorLevel()
        {
            // Truecolor hint
            string? colorterm = System.Environment.GetEnvironmentVariable("COLORTERM");
            if (!string.IsNullOrEmpty(colorterm) && colorterm.Contains("truecolor", StringComparison.OrdinalIgnoreCase))
            {
                return ColorSupportLevel.TrueColor;
            }

            // TERM based detection
            string term = System.Environment.GetEnvironmentVariable("TERM") ?? string.Empty;
            if (term.Contains("256color", StringComparison.OrdinalIgnoreCase))
            {
                return ColorSupportLevel.Indexed256;
            }

            // Fallback basic
            return ColorSupportLevel.Basic16;
        }
    }
}
