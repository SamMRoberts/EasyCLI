using System.IO;
using EasyCLI.Console; // explicit for TextWriter

namespace EasyCLI
{
    /// <summary>
    /// Writes to the console with optional ANSI styling. Honors NO_COLOR/FORCE_COLOR and output redirection.
    /// </summary>
    public class ConsoleWriter : IConsoleWriter
    {
        public ConsoleWriter(bool? enableColors = null, TextWriter? output = null)
        {
            Output = output ?? System.Console.Out;
            ColorEnabled = DecideColorEnabled(enableColors);
            ColorLevel = ColorEnabled ? DetectColorLevel() : ColorSupportLevel.None;
        }

        public bool ColorEnabled { get; }
        public ColorSupportLevel ColorLevel { get; }
        public TextWriter Output { get; }

        public void Write(string message)
        {
            Output.Write(message);
        }

        public void WriteLine(string message)
        {
            Output.WriteLine(message);
        }

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

            string? noColor = Environment.GetEnvironmentVariable("NO_COLOR");
            if (!string.IsNullOrEmpty(noColor))
            {
                return false;
            }

            string? forceColor = Environment.GetEnvironmentVariable("FORCE_COLOR");
            if (!string.IsNullOrEmpty(forceColor))
            {
                return true;
            }

            if (System.Console.IsOutputRedirected)
            {
                return false;
            }

            string? term = Environment.GetEnvironmentVariable("TERM");
            if (string.Equals(term, "dumb", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        private static ColorSupportLevel DetectColorLevel()
        {
            // Truecolor hint
            string? colorterm = Environment.GetEnvironmentVariable("COLORTERM");
            if (!string.IsNullOrEmpty(colorterm) && colorterm.Contains("truecolor", StringComparison.OrdinalIgnoreCase))
            {
                return ColorSupportLevel.TrueColor;
            }

            // TERM based detection
            string term = Environment.GetEnvironmentVariable("TERM") ?? string.Empty;
            if (term.Contains("256color", StringComparison.OrdinalIgnoreCase))
            {
                return ColorSupportLevel.Indexed256;
            }

            // Fallback basic
            return ColorSupportLevel.Basic16;
        }
    }
}
