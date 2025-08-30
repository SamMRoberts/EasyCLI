using EasyCLI.Console;
using EasyCLI.Deprecation;

namespace EasyCLI.Extensions
{
    /// <summary>
    /// Extension methods for writing deprecation warnings to the console.
    /// </summary>
    public static class DeprecationExtensions
    {
        private static readonly ConsoleTheme DefaultTheme = new();

        /// <summary>
        /// Writes a deprecation warning to the console using consistent formatting.
        /// </summary>
        /// <param name="writer">The console writer to write to.</param>
        /// <param name="featureName">The name of the deprecated feature.</param>
        /// <param name="deprecationInfo">The deprecation information.</param>
        /// <param name="theme">The theme to use for styling. If null, uses the default theme.</param>
        public static void WriteDeprecationWarning(
            this IConsoleWriter writer,
            string featureName,
            DeprecationInfo deprecationInfo,
            ConsoleTheme? theme = null)
        {
            ArgumentNullException.ThrowIfNull(writer);
            ArgumentException.ThrowIfNullOrEmpty(featureName);
            ArgumentNullException.ThrowIfNull(deprecationInfo);

            ConsoleTheme effectiveTheme = theme ?? DefaultTheme;
            string message = deprecationInfo.GetFormattedMessage(featureName);

            writer.WriteWarningLine($"⚠ Deprecation Warning: {message}", effectiveTheme);
        }

        /// <summary>
        /// Writes a deprecation warning line to the console using consistent formatting.
        /// </summary>
        /// <param name="writer">The console writer to write to.</param>
        /// <param name="featureName">The name of the deprecated feature.</param>
        /// <param name="deprecationInfo">The deprecation information.</param>
        /// <param name="theme">The theme to use for styling. If null, uses the default theme.</param>
        public static void WriteDeprecationWarningLine(
            this IConsoleWriter writer,
            string featureName,
            DeprecationInfo deprecationInfo,
            ConsoleTheme? theme = null)
        {
            ArgumentNullException.ThrowIfNull(writer);
            ArgumentException.ThrowIfNullOrEmpty(featureName);
            ArgumentNullException.ThrowIfNull(deprecationInfo);

            WriteDeprecationWarning(writer, featureName, deprecationInfo, theme);
            writer.WriteLine();
        }

        /// <summary>
        /// Writes a simple deprecation warning with just a message.
        /// </summary>
        /// <param name="writer">The console writer to write to.</param>
        /// <param name="message">The deprecation message.</param>
        /// <param name="theme">The theme to use for styling. If null, uses the default theme.</param>
        public static void WriteDeprecationWarning(
            this IConsoleWriter writer,
            string message,
            ConsoleTheme? theme = null)
        {
            ArgumentNullException.ThrowIfNull(writer);
            ArgumentException.ThrowIfNullOrEmpty(message);

            ConsoleTheme effectiveTheme = theme ?? DefaultTheme;
            writer.WriteWarningLine($"⚠ Deprecation Warning: {message}", effectiveTheme);
        }

        /// <summary>
        /// Checks if a command option is deprecated and writes a warning if it is.
        /// </summary>
        /// <param name="writer">The console writer to write to.</param>
        /// <param name="option">The command option to check.</param>
        /// <param name="theme">The theme to use for styling. If null, uses the default theme.</param>
        /// <returns>True if the option is deprecated, false otherwise.</returns>
        public static bool CheckAndWarnIfDeprecated(
            this IConsoleWriter writer,
            Shell.CommandOption option,
            ConsoleTheme? theme = null)
        {
            ArgumentNullException.ThrowIfNull(writer);
            ArgumentNullException.ThrowIfNull(option);

            if (option.IsDeprecated && option.DeprecationInfo != null)
            {
                string optionName = $"--{option.LongName}";
                if (!string.IsNullOrEmpty(option.ShortName))
                {
                    optionName += $" (-{option.ShortName})";
                }

                WriteDeprecationWarning(writer, optionName, option.DeprecationInfo, theme);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a command argument is deprecated and writes a warning if it is.
        /// </summary>
        /// <param name="writer">The console writer to write to.</param>
        /// <param name="argument">The command argument to check.</param>
        /// <param name="theme">The theme to use for styling. If null, uses the default theme.</param>
        /// <returns>True if the argument is deprecated, false otherwise.</returns>
        public static bool CheckAndWarnIfDeprecated(
            this IConsoleWriter writer,
            Shell.CommandArgument argument,
            ConsoleTheme? theme = null)
        {
            ArgumentNullException.ThrowIfNull(writer);
            ArgumentNullException.ThrowIfNull(argument);

            if (argument.IsDeprecated && argument.DeprecationInfo != null)
            {
                WriteDeprecationWarning(writer, argument.Name, argument.DeprecationInfo, theme);
                return true;
            }

            return false;
        }
    }
}
