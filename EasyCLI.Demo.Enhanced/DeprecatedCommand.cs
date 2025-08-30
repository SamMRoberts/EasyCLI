using EasyCLI.Deprecation;
using EasyCLI.Extensions;
using EasyCLI.Shell;

namespace EasyCLI.Demo.Enhanced
{
    /// <summary>
    /// Demo command showcasing the deprecation warning framework.
    /// </summary>
    public class DeprecatedCommand : BaseCliCommand, IDeprecatedCliCommand
    {
        /// <inheritdoc />
        public override string Name => "old-command";

        /// <inheritdoc />
        public override string Description => "A deprecated command demonstrating deprecation warnings";

        /// <inheritdoc />
        public override string Category => "Demo";

        /// <inheritdoc />
        public DeprecationInfo DeprecationInfo => DeprecationInfo.Replaced(
            "new-command", 
            DeprecationVersion.NextMajor("1.0.0"));

        /// <summary>
        /// Configures the help information for this command.
        /// </summary>
        /// <param name="help">The help information to configure.</param>
        protected override void ConfigureHelp(CommandHelp help)
        {
            ArgumentNullException.ThrowIfNull(help);

            help.Usage = "old-command [options] <input>";
            help.Description = "This command is deprecated and will be removed in version 2.0.0. Use 'new-command' instead.";

            help.Arguments.Add(new CommandArgument("input", "Input to process", isRequired: true));

            // Regular option
            help.Options.Add(new CommandOption("verbose", "v", "Enable verbose output"));

            // Deprecated option
            help.Options.Add(CommandOption.Deprecated(
                "legacy-format", 
                "l", 
                "Use legacy output format",
                DeprecationInfo.Replaced("--json", DeprecationVersion.NextMinor("1.0.0"))));

            // Another deprecated option
            help.Options.Add(CommandOption.Deprecated(
                "old-style",
                "o",
                "Use old-style processing",
                DeprecationInfo.Obsolete(DeprecationVersion.NextMajor("1.0.0"), "no longer supported")));

            help.Examples.Add(new CommandExample("old-command myfile.txt", "Process a file (deprecated)"));
            help.Examples.Add(new CommandExample("new-command myfile.txt", "Use the new command instead"));
        }

        /// <summary>
        /// Executes the deprecated command.
        /// </summary>
        /// <param name="args">The parsed command line arguments.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Exit code.</returns>
        protected override async Task<int> ExecuteCommand(CommandLineArgs args, ShellExecutionContext context, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(args);
            ArgumentNullException.ThrowIfNull(context);

            var theme = GetTheme(context);

            // Simulate some work
            context.Writer.WriteInfoLine("Processing input...", theme);
            await Task.Delay(100, cancellationToken);

            if (args.HasFlag("verbose"))
            {
                context.Writer.WriteInfoLine("Verbose mode enabled", theme);
            }

            if (args.HasFlag("legacy-format"))
            {
                context.Writer.WriteInfoLine("Using legacy format (this feature is deprecated)", theme);
            }

            if (args.HasFlag("old-style"))
            {
                context.Writer.WriteInfoLine("Using old-style processing (this feature is deprecated)", theme);
            }

            context.Writer.WriteSuccessLine("✓ Processing completed", theme);
            context.Writer.WriteLine("");
            context.Writer.WriteHintLine("💡 Consider migrating to 'new-command' for better performance and features.", theme);

            return ExitCodes.Success;
        }
    }
}