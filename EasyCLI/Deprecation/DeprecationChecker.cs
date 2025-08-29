using EasyCLI.Console;
using EasyCLI.Shell;

namespace EasyCLI.Deprecation
{
    /// <summary>
    /// Utility class for checking and handling deprecation warnings.
    /// </summary>
    public static class DeprecationChecker
    {
        /// <summary>
        /// Checks command line arguments for deprecated options and displays warnings.
        /// </summary>
        /// <param name="args">The parsed command line arguments.</param>
        /// <param name="commandHelp">The command help information containing option definitions.</param>
        /// <param name="writer">The console writer to output warnings to.</param>
        /// <param name="theme">The theme to use for styling warnings.</param>
        /// <returns>The number of deprecated features used.</returns>
        public static int CheckAndWarnDeprecatedOptions(
            CommandLineArgs args,
            CommandHelp commandHelp,
            IConsoleWriter writer,
            ConsoleTheme? theme = null)
        {
            ArgumentNullException.ThrowIfNull(args);
            ArgumentNullException.ThrowIfNull(commandHelp);
            ArgumentNullException.ThrowIfNull(writer);

            int deprecatedCount = 0;

            // Check each option in the command help
            foreach (CommandOption option in commandHelp.Options)
            {
                if (!option.IsDeprecated)
                {
                    continue;
                }

                // Check if this deprecated option was used
                bool wasUsed = args.HasFlag(option.LongName) ||
                               (!string.IsNullOrEmpty(option.ShortName) && args.HasFlag(option.ShortName));

                if (wasUsed)
                {
                    _ = writer.CheckAndWarnIfDeprecated(option, theme);
                    deprecatedCount++;
                }
            }

            return deprecatedCount;
        }

        /// <summary>
        /// Checks command arguments for deprecated ones and displays warnings.
        /// </summary>
        /// <param name="argumentValues">The actual argument values provided.</param>
        /// <param name="commandHelp">The command help information containing argument definitions.</param>
        /// <param name="writer">The console writer to output warnings to.</param>
        /// <param name="theme">The theme to use for styling warnings.</param>
        /// <returns>The number of deprecated arguments used.</returns>
        public static int CheckAndWarnDeprecatedArguments(
            IReadOnlyList<string> argumentValues,
            CommandHelp commandHelp,
            IConsoleWriter writer,
            ConsoleTheme? theme = null)
        {
            ArgumentNullException.ThrowIfNull(argumentValues);
            ArgumentNullException.ThrowIfNull(commandHelp);
            ArgumentNullException.ThrowIfNull(writer);

            int deprecatedCount = 0;

            // Check each argument that was provided
            for (int i = 0; i < Math.Min(argumentValues.Count, commandHelp.Arguments.Count); i++)
            {
                CommandArgument argumentDef = commandHelp.Arguments[i];
                if (argumentDef.IsDeprecated)
                {
                    _ = writer.CheckAndWarnIfDeprecated(argumentDef, theme);
                    deprecatedCount++;
                }
            }

            return deprecatedCount;
        }

        /// <summary>
        /// Checks if a command is deprecated and displays a warning.
        /// </summary>
        /// <param name="command">The command to check.</param>
        /// <param name="writer">The console writer to output warnings to.</param>
        /// <param name="theme">The theme to use for styling warnings.</param>
        /// <returns>True if the command is deprecated, false otherwise.</returns>
        public static bool CheckAndWarnDeprecatedCommand(
            ICliCommand command,
            IConsoleWriter writer,
            ConsoleTheme? theme = null)
        {
            ArgumentNullException.ThrowIfNull(command);
            ArgumentNullException.ThrowIfNull(writer);

            // Check if the command implements deprecated command interface
            if (command is IDeprecatedCliCommand deprecatedCommand)
            {
                writer.WriteDeprecationWarning(command.Name, deprecatedCommand.DeprecationInfo, theme);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Performs a comprehensive deprecation check for a command execution.
        /// </summary>
        /// <param name="command">The command being executed.</param>
        /// <param name="args">The parsed command line arguments.</param>
        /// <param name="commandHelp">The command help information.</param>
        /// <param name="writer">The console writer to output warnings to.</param>
        /// <param name="theme">The theme to use for styling warnings.</param>
        /// <returns>The total number of deprecated features used.</returns>
        public static int PerformComprehensiveDeprecationCheck(
            ICliCommand command,
            CommandLineArgs args,
            CommandHelp commandHelp,
            IConsoleWriter writer,
            ConsoleTheme? theme = null)
        {
            ArgumentNullException.ThrowIfNull(command);
            ArgumentNullException.ThrowIfNull(args);
            ArgumentNullException.ThrowIfNull(commandHelp);
            ArgumentNullException.ThrowIfNull(writer);

            int totalDeprecated = 0;

            // Check the command itself
            if (CheckAndWarnDeprecatedCommand(command, writer, theme))
            {
                totalDeprecated++;
            }

            // Check options
            totalDeprecated += CheckAndWarnDeprecatedOptions(args, commandHelp, writer, theme);

            // Check arguments
            totalDeprecated += CheckAndWarnDeprecatedArguments(args.Arguments, commandHelp, writer, theme);

            return totalDeprecated;
        }
    }
}
