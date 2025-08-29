using EasyCLI.Shell;

namespace EasyCLI.Console
{
    /// <summary>
    /// Factory for creating console writers that respect both --plain flags and NO_COLOR environment variables.
    /// Provides centralized logic for determining when to use plain vs styled output.
    /// </summary>
    public static class ConsoleWriterFactory
    {
        /// <summary>
        /// Creates a console writer based on command line arguments and environment variables.
        /// </summary>
        /// <param name="args">Command line arguments to check for --plain flag.</param>
        /// <param name="output">Optional text writer for output. If null, uses Console.Out.</param>
        /// <returns>A plain console writer if --plain flag is present, otherwise a regular console writer.</returns>
        public static IConsoleWriter Create(CommandLineArgs? args = null, TextWriter? output = null)
        {
            bool usePlainMode = ShouldUsePlainMode(args);

            // Create base writer - if plain mode is requested, we'll wrap it
            // If NO_COLOR is set, the base writer will already disable colors
            ConsoleWriter baseWriter = new(enableColors: !usePlainMode, output: output);

            // If --plain flag is explicitly requested, wrap with PlainConsoleWriter for symbol/decoration stripping
            return args?.IsPlainOutput == true ? new PlainConsoleWriter(baseWriter) : baseWriter;
        }

        /// <summary>
        /// Creates a console writer with explicit plain mode setting.
        /// </summary>
        /// <param name="plainMode">Whether to enable plain mode (strip all styling and decorations).</param>
        /// <param name="output">Optional text writer for output. If null, uses Console.Out.</param>
        /// <returns>A plain console writer if plainMode is true, otherwise a regular console writer.</returns>
        public static IConsoleWriter Create(bool plainMode, TextWriter? output = null)
        {
            ConsoleWriter baseWriter = new(enableColors: !plainMode, output: output);

            return plainMode ? new PlainConsoleWriter(baseWriter) : baseWriter;
        }

        /// <summary>
        /// Determines whether plain mode should be used based on command line arguments and environment variables.
        /// </summary>
        /// <param name="args">Command line arguments to check.</param>
        /// <returns>True if plain mode should be used, false otherwise.</returns>
        public static bool ShouldUsePlainMode(CommandLineArgs? args = null)
        {
            // --plain flag has highest priority
            if (args?.IsPlainOutput == true)
            {
                return true;
            }

            // NO_COLOR environment variable affects colors but not necessarily symbols/decorations
            // We keep this separate from --plain logic as requested in the issue
            string? noColor = System.Environment.GetEnvironmentVariable("NO_COLOR");
            if (!string.IsNullOrEmpty(noColor))
            {
                // NO_COLOR only disables colors, not symbols/decorations
                // This is handled by ConsoleWriter internally
                return false;
            }

            return false;
        }

        /// <summary>
        /// Checks if colors should be disabled based on environment variables.
        /// This is separate from plain mode - NO_COLOR only affects colors, not symbols/decorations.
        /// </summary>
        /// <returns>True if colors should be disabled due to environment variables.</returns>
        public static bool ShouldDisableColors()
        {
            string? noColor = System.Environment.GetEnvironmentVariable("NO_COLOR");
            if (!string.IsNullOrEmpty(noColor))
            {
                return true;
            }

            if (System.Console.IsOutputRedirected)
            {
                return true;
            }

            string? term = System.Environment.GetEnvironmentVariable("TERM");
            return string.Equals(term, "dumb", StringComparison.OrdinalIgnoreCase);
        }
    }
}
