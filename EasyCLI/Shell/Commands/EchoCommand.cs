namespace EasyCLI.Shell.Commands
{
    /// <summary>
    /// Example echo command demonstrating the enhanced CLI command pattern.
    /// </summary>
    public class EchoCommand : BaseCliCommand
    {
        /// <summary>
        /// Gets the primary name of the command.
        /// </summary>
        public override string Name => "echo";

        /// <summary>
        /// Gets a short description of the command.
        /// </summary>
        public override string Description => "Print text to the console with optional styling";

        /// <summary>
        /// Configures the help information for this command.
        /// </summary>
        /// <param name="help">The help information to configure.</param>
        protected override void ConfigureHelp(CommandHelp help)
        {
            ArgumentNullException.ThrowIfNull(help);

            help.Usage = "echo [options] <text...>";
            help.Description = "Prints the specified text to the console. Supports various styling options and output modes.";

            help.Arguments.Add(new CommandArgument("text", "The text to print (multiple words supported)", true));

            help.Options.Add(new CommandOption("success", "s", "Style output as success message"));
            help.Options.Add(new CommandOption("warning", "w", "Style output as warning message"));
            help.Options.Add(new CommandOption("error", "e", "Style output as error message"));
            help.Options.Add(new CommandOption("info", "i", "Style output as info message"));
            help.Options.Add(new CommandOption("hint", null, "Style output as hint message"));
            help.Options.Add(new CommandOption("uppercase", "u", "Convert text to uppercase"));
            help.Options.Add(new CommandOption("repeat", "r", "Number of times to repeat the text", true, "1"));
            help.Options.Add(new CommandOption("dry-run", "n", "Show what would be printed without actually printing"));

            help.Examples.Add(new CommandExample("echo Hello World", "Print 'Hello World' to the console"));
            help.Examples.Add(new CommandExample("echo --success \"Operation complete\"", "Print success message"));
            help.Examples.Add(new CommandExample("echo --warning --uppercase \"Warning message\"", "Print uppercase warning"));
            help.Examples.Add(new CommandExample("echo --repeat 3 \"Repeated text\"", "Print text 3 times"));
            help.Examples.Add(new CommandExample("echo --dry-run \"Test message\"", "Show what would be printed"));
        }

        /// <summary>
        /// Validates command arguments before execution.
        /// </summary>
        /// <param name="args">The parsed command line arguments.</param>
        /// <param name="context">The execution context.</param>
        /// <returns>Exit code (0 if valid, non-zero if invalid).</returns>
        protected override int ValidateArguments(CommandLineArgs args, ShellExecutionContext context)
        {
            ArgumentNullException.ThrowIfNull(args);
            ArgumentNullException.ThrowIfNull(context);

            if (args.Arguments.Count == 0)
            {
                context.Writer.WriteErrorLine("Error: No text specified to echo");
                ShowSuggestion(context, $"Use '{Name} --help' for usage information");
                return ExitCodes.InvalidArguments;
            }

            // Validate repeat count if specified
            string? repeatOption = args.GetOption("repeat");
            if (!string.IsNullOrEmpty(repeatOption))
            {
                if (!int.TryParse(repeatOption, out int repeatCount) || repeatCount < 1 || repeatCount > 100)
                {
                    context.Writer.WriteErrorLine("Error: Repeat count must be a number between 1 and 100");
                    return ExitCodes.InvalidArguments;
                }
            }

            return ExitCodes.Success;
        }

        /// <summary>
        /// Gets custom completion suggestions specific to this command.
        /// </summary>
        /// <param name="context">Execution context.</param>
        /// <param name="prefix">The prefix to match.</param>
        /// <returns>Custom completion suggestions.</returns>
        protected override string[] GetCustomCompletions(ShellExecutionContext context, string prefix)
        {
            string[] echoOptions = new[] { "--success", "--warning", "--error", "--info", "--hint", "--uppercase", "--repeat" };
            return echoOptions.Where(opt => opt.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToArray();
        }

        /// <summary>
        /// Executes the echo command logic.
        /// </summary>
        /// <param name="args">The parsed command line arguments.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Exit code (0 for success).</returns>
        protected override Task<int> ExecuteCommand(CommandLineArgs args, ShellExecutionContext context, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(args);
            ArgumentNullException.ThrowIfNull(context);

            string text = string.Join(" ", args.Arguments);
            ConsoleTheme theme = GetTheme(context);

            // Apply text transformations
            if (args.HasFlag("uppercase"))
            {
                text = text.ToUpperInvariant();
            }

            // Get repeat count
            int repeatCount = 1;
            string? repeatOption = args.GetOption("repeat");
            if (!string.IsNullOrEmpty(repeatOption) && int.TryParse(repeatOption, out int count))
            {
                repeatCount = count;
            }

            // Handle dry-run mode
            if (args.IsDryRun)
            {
                context.Writer.WriteWarningLine("[DRY RUN] Would print the following:", theme);
                context.Writer.WriteHintLine($"Text: \"{text}\"", theme);
                context.Writer.WriteHintLine($"Repeat count: {repeatCount}", theme);

                if (args.HasFlag("success"))
                {
                    context.Writer.WriteHintLine("Style: Success", theme);
                }
                else if (args.HasFlag("warning"))
                {
                    context.Writer.WriteHintLine("Style: Warning", theme);
                }
                else if (args.HasFlag("error"))
                {
                    context.Writer.WriteHintLine("Style: Error", theme);
                }
                else if (args.HasFlag("info"))
                {
                    context.Writer.WriteHintLine("Style: Info", theme);
                }
                else if (args.HasFlag("hint"))
                {
                    context.Writer.WriteHintLine("Style: Hint", theme);
                }
                else
                {
                    context.Writer.WriteHintLine("Style: Normal", theme);
                }

                return Task.FromResult(ExitCodes.Success);
            }

            // Print the text with appropriate styling
            for (int i = 0; i < repeatCount; i++)
            {
                if (args.HasFlag("success"))
                {
                    context.Writer.WriteSuccessLine(text, theme);
                }
                else if (args.HasFlag("warning"))
                {
                    context.Writer.WriteWarningLine(text, theme);
                }
                else if (args.HasFlag("error"))
                {
                    context.Writer.WriteErrorLine(text, theme);
                }
                else if (args.HasFlag("info"))
                {
                    context.Writer.WriteInfoLine(text, theme);
                }
                else if (args.HasFlag("hint"))
                {
                    context.Writer.WriteHintLine(text, theme);
                }
                else
                {
                    context.Writer.WriteLine(text);
                }

                // Add some feedback for verbose mode
                if (args.IsVerbose && i == 0)
                {
                    context.Writer.WriteHintLine($"[VERBOSE] Echoing text {repeatCount} time(s)", theme);
                }
            }

            return Task.FromResult(ExitCodes.Success);
        }
    }
}
