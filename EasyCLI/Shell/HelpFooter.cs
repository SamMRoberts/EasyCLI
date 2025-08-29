using System.Reflection;
using EasyCLI.Console;

namespace EasyCLI.Shell
{
    /// <summary>
    /// Provides consistent help footer content for all CLI help outputs.
    /// </summary>
    public static class HelpFooter
    {
        /// <summary>
        /// Gets the current assembly version.
        /// </summary>
        /// <returns>The assembly version string.</returns>
        public static string GetVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version? version = assembly.GetName().Version;
            return version?.ToString() ?? "Unknown";
        }

        /// <summary>
        /// Gets the standard help footer with support paths and version information.
        /// </summary>
        /// <returns>Formatted footer lines.</returns>
        public static string[] GetFooterLines()
        {
            return
            [
                "",
                "SUPPORT:",
                $"  Version: {GetVersion()}",
                "  Issues:  https://github.com/SamMRoberts/EasyCLI/issues",
                "  Docs:    https://github.com/SamMRoberts/EasyCLI"
            ];
        }

        /// <summary>
        /// Writes the standard help footer to the specified writer.
        /// </summary>
        /// <param name="writer">The console writer to write to.</param>
        /// <param name="theme">The console theme to use for formatting.</param>
        public static void WriteFooter(IConsoleWriter writer, ConsoleTheme theme)
        {
            ArgumentNullException.ThrowIfNull(writer);

            string[] footerLines = GetFooterLines();

            foreach (string line in footerLines)
            {
                if (line.StartsWith("SUPPORT:", StringComparison.Ordinal))
                {
                    writer.WriteInfoLine(line, theme);
                }
                else if (string.IsNullOrEmpty(line))
                {
                    writer.WriteLine(line);
                }
                else
                {
                    writer.WriteLine(line);
                }
            }
        }
    }
}
