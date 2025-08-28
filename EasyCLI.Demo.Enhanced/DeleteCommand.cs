using EasyCLI.Extensions;
using EasyCLI.Shell;

namespace EasyCLI.Demo.Enhanced
{
    /// <summary>
    /// Demo command that showcases dangerous operation confirmation framework.
    /// </summary>
    public class DeleteCommand : BaseCliCommand
    {
        /// <summary>
        /// Gets the command name.
        /// </summary>
        public override string Name => "delete";

        /// <summary>
        /// Gets the command description.
        /// </summary>
        public override string Description => "Demonstrates dangerous operation confirmation (safe demo - no actual deletion)";

        /// <summary>
        /// Configures the help information for this command.
        /// </summary>
        /// <param name="help">The help information to configure.</param>
        protected override void ConfigureHelp(CommandHelp help)
        {
            ArgumentNullException.ThrowIfNull(help);
            
            help.Usage = "delete [options] <target>";
            help.Description = "Simulates deletion operations with proper safety confirmations.";

            help.Arguments.Add(new CommandArgument("target", "What to delete (files, database, config)", isRequired: true));

            help.Options.Add(new CommandOption("recursive", "r", "Delete recursively (increases danger level)"));
            help.Options.Add(new CommandOption("permanent", "p", "Permanent deletion (cannot be undone)"));

            help.Examples.Add(new CommandExample("delete files", "Delete files with confirmation"));
            help.Examples.Add(new CommandExample("delete files --yes", "Delete files without prompting"));
            help.Examples.Add(new CommandExample("delete database --force", "Force delete database"));
            help.Examples.Add(new CommandExample("delete config --dry-run", "Show what would be deleted"));
        }

        /// <summary>
        /// Executes the delete command with appropriate safety checks.
        /// </summary>
        /// <param name="args">The parsed command line arguments.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Exit code (0 for success).</returns>
        protected override Task<int> ExecuteCommand(CommandLineArgs args, ShellExecutionContext context, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(args);
            ArgumentNullException.ThrowIfNull(context);
            
            if (args.Arguments.Count == 0)
            {
                context.Writer.WriteErrorLine("Target must be specified");
                ShowSuggestion(context, $"Use '{Name} --help' for usage information");
                return Task.FromResult(ExitCodes.InvalidArguments);
            }

            string target = args.Arguments[0];
            bool isRecursive = args.HasFlag("recursive");
            bool isPermanent = args.HasFlag("permanent");

            // Build operation description
            string operation = $"delete {target}";
            if (isRecursive) operation += " recursively";
            if (isPermanent) operation += " permanently";

            // Build warnings based on flags
            var warnings = new List<string>();

            if (isRecursive)
            {
                warnings.Add("This will delete ALL contents including subdirectories");
            }

            if (isPermanent)
            {
                warnings.Add("This operation CANNOT be undone");
                warnings.Add("No backups will be created");
            }

            if (target.Equals("database", StringComparison.OrdinalIgnoreCase))
            {
                warnings.Add("Database deletion will affect all applications");
                warnings.Add("All data will be lost");
            }

            // Handle dry-run mode
            if (args.IsDryRun)
            {
                context.Writer.WriteWarningLine("[DRY RUN] Would execute dangerous operation:");
                context.Writer.WriteInfoLine($"Operation: {operation}");
                
                if (warnings.Count > 0)
                {
                    context.Writer.WriteLine(string.Empty);
                    context.Writer.WriteWarningLine("Warnings that would be shown:");
                    foreach (var warning in warnings)
                    {
                        context.Writer.WriteWarningLine($"⚠️  {warning}");
                    }
                }

                context.Writer.WriteHintLine("Run without --dry-run to see actual confirmation flow");
                return Task.FromResult(ExitCodes.Success);
            }

            // Get confirmation for dangerous operation
            bool confirmed = ConfirmDangerous(
                operation, 
                context, 
                args, 
                warnings.Count > 0 ? warnings.ToArray() : null,
                customPrompt: isPermanent ? "Are you absolutely sure you want to proceed?" : null);

            if (!confirmed)
            {
                context.Writer.WriteInfoLine("Operation cancelled by user");
                return Task.FromResult(ExitCodes.UserCancelled);
            }

            // Simulate the operation (this is just a demo!)
            context.Writer.WriteSuccessLine("✓ Demo operation completed successfully");
            context.Writer.WriteHintLine("(This was just a simulation - nothing was actually deleted)");

            return Task.FromResult(ExitCodes.Success);
        }
    }
}