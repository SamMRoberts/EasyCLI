using System;
using System.IO;

namespace EasyCLI
{
    /// <summary>
    /// Writes to the console with optional ANSI styling. Honors NO_COLOR/FORCE_COLOR and output redirection.
    /// </summary>
    public class ConsoleWriter : IConsoleWriter
    {
        public ConsoleWriter(bool? enableColors = null, TextWriter? output = null)
        {
            Output = output ?? Console.Out;
            ColorEnabled = DecideColorEnabled(enableColors);
        }

        public bool ColorEnabled { get; }
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
                Output.Write(style.Apply(message));
            else
                Output.Write(message);
        }

        public void WriteLine(string message, ConsoleStyle style)
        {
            if (ColorEnabled && style.Codes.Length > 0)
                Output.WriteLine(style.Apply(message));
            else
                Output.WriteLine(message);
        }

        private static bool DecideColorEnabled(bool? enableColors)
        {
            if (enableColors.HasValue)
                return enableColors.Value;

            var noColor = Environment.GetEnvironmentVariable("NO_COLOR");
            if (!string.IsNullOrEmpty(noColor))
                return false;

            var forceColor = Environment.GetEnvironmentVariable("FORCE_COLOR");
            if (!string.IsNullOrEmpty(forceColor))
                return true;

            if (Console.IsOutputRedirected)
                return false;

            var term = Environment.GetEnvironmentVariable("TERM");
            if (string.Equals(term, "dumb", StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
