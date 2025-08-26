namespace EasyCLI.Shell
{
    /// <summary>
    /// Base implementation of <see cref="ICliCommand"/> with common CLI patterns and help support.
    /// </summary>
    public abstract class BaseCliCommand : ICliCommand
    {
        /// <summary>
        /// Gets the primary name of the command.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets a short description of the command.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Gets the detailed help information for this command.
        /// </summary>
        /// <returns>Command help information.</returns>
        public virtual CommandHelp GetHelp()
        {
            CommandHelp help = new()
            {
                Usage = $"{Name} [options]",
                Description = Description,
            };

            // Add standard options that all commands support
            help.Options.Add(new CommandOption("help", "h", "Show help information"));
            help.Options.Add(new CommandOption("verbose", "v", "Enable verbose output"));
            help.Options.Add(new CommandOption("quiet", "q", "Suppress non-essential output"));

            // Allow derived classes to add specific help content
            ConfigureHelp(help);

            return help;
        }

        /// <summary>
        /// Executes the command with structured argument parsing and error handling.
        /// </summary>
        /// <param name="context">The shell execution context.</param>
        /// <param name="args">Arguments (excluding command name).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Exit code (0 for success).</returns>
        public async Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(args);

            try
            {
                CommandLineArgs parsedArgs = new(args);

                // Handle help request
                if (parsedArgs.IsHelpRequested)
                {
                    ShowHelp(context);
                    return ExitCodes.Success;
                }

                // Validate arguments before execution
                int validationResult = ValidateArguments(parsedArgs, context);
                if (validationResult != ExitCodes.Success)
                {
                    return validationResult;
                }

                // Execute the command
                return await ExecuteCommand(parsedArgs, context, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                context.Writer.WriteWarningLine("Operation was cancelled");
                return ExitCodes.UserCancelled;
            }
            catch (FileNotFoundException ex)
            {
                context.Writer.WriteErrorLine($"File not found: {ex.FileName}");
                ShowSuggestion(context, "Make sure the file exists and you have read permissions");
                return ExitCodes.FileNotFound;
            }
            catch (UnauthorizedAccessException)
            {
                context.Writer.WriteErrorLine("Permission denied");
                ShowSuggestion(context, "Try running with elevated permissions or check file ownership");
                return ExitCodes.PermissionDenied;
            }
            catch (DirectoryNotFoundException ex)
            {
                context.Writer.WriteErrorLine($"Directory not found: {ex.Message}");
                ShowSuggestion(context, "Make sure the directory exists and is accessible");
                return ExitCodes.FileNotFound;
            }
            catch (ArgumentException ex)
            {
                context.Writer.WriteErrorLine($"Invalid argument: {ex.Message}");
                ShowSuggestion(context, $"Use '{Name} --help' for usage information");
                return ExitCodes.InvalidArguments;
            }
            catch (Exception ex)
            {
                CommandLineArgs parsedArgs = new(args);
                if (parsedArgs.IsVerbose)
                {
                    context.Writer.WriteErrorLine($"Unexpected error: {ex}");
                }
                else
                {
                    context.Writer.WriteErrorLine($"Unexpected error: {ex.Message}");
                    ShowSuggestion(context, "Run with --verbose for detailed error information");
                }

                return ExitCodes.GeneralError;
            }
        }

        /// <summary>
        /// Gets completion suggestions for a partially typed token.
        /// </summary>
        /// <param name="context">Execution context.</param>
        /// <param name="prefix">The already typed text (may be empty).</param>
        /// <returns>Matching completion candidates.</returns>
        public virtual string[] GetCompletions(ShellExecutionContext context, string prefix)
        {
            // Provide common option completions
            string[] commonOptions = ["--help", "--verbose", "--quiet", "--dry-run", "--force", "--yes"];

            if (string.IsNullOrEmpty(prefix))
            {
                return commonOptions;
            }

            string[] matches = [.. commonOptions.Where(opt => opt.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))];

            // Allow derived classes to add specific completions
            string[] customCompletions = GetCustomCompletions(context, prefix);
            return [.. matches, .. customCompletions];
        }

        /// <summary>
        /// Shows help information for this command.
        /// </summary>
        /// <param name="context">The execution context.</param>
        protected virtual void ShowHelp(ShellExecutionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            CommandHelp help = GetHelp();
            ConsoleTheme theme = GetTheme(context);

            context.Writer.WriteHeadingLine($"{Name} - {Description}", theme);
            context.Writer.WriteLine("");

            // Usage
            context.Writer.WriteInfoLine("USAGE:", theme);
            context.Writer.WriteLine($"  {help.Usage}");
            context.Writer.WriteLine("");

            // Description
            if (!string.IsNullOrEmpty(help.Description) && help.Description != Description)
            {
                context.Writer.WriteInfoLine("DESCRIPTION:", theme);
                context.Writer.WriteWrapped(help.Description, indent: 2);
                context.Writer.WriteLine("");
            }

            // Arguments
            if (help.Arguments.Count > 0)
            {
                context.Writer.WriteInfoLine("ARGUMENTS:", theme);
                foreach (CommandArgument arg in help.Arguments)
                {
                    string required = arg.IsRequired ? " (required)" : " (optional)";
                    context.Writer.WriteLine($"  {arg.Name,-20} {arg.Description}{required}");
                }

                context.Writer.WriteLine("");
            }

            // Options
            if (help.Options.Count > 0)
            {
                context.Writer.WriteInfoLine("OPTIONS:", theme);
                foreach (CommandOption opt in help.Options)
                {
                    string shortForm = !string.IsNullOrEmpty(opt.ShortName) ? $"-{opt.ShortName}, " : "    ";
                    string longForm = $"--{opt.LongName}";
                    string defaultInfo = !string.IsNullOrEmpty(opt.DefaultValue) ? $" (default: {opt.DefaultValue})" : "";
                    context.Writer.WriteLine($"  {shortForm}{longForm,-18} {opt.Description}{defaultInfo}");
                }

                context.Writer.WriteLine("");
            }

            // Examples
            if (help.Examples.Count > 0)
            {
                context.Writer.WriteInfoLine("EXAMPLES:", theme);
                foreach (CommandExample example in help.Examples)
                {
                    context.Writer.WriteLine($"  {example.Command}");
                    context.Writer.WriteHintLine($"    {example.Description}", theme);
                    context.Writer.WriteLine("");
                }
            }
        }

        /// <summary>
        /// Shows a helpful suggestion to the user.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="suggestion">The suggestion text.</param>
        protected virtual void ShowSuggestion(ShellExecutionContext context, string suggestion)
        {
            ArgumentNullException.ThrowIfNull(context);

            ConsoleTheme theme = GetTheme(context);
            context.Writer.WriteHintLine($"💡 {suggestion}", theme);
        }

        /// <summary>
        /// Gets the console theme to use for output.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <returns>The console theme.</returns>
        protected virtual ConsoleTheme GetTheme(ShellExecutionContext context)
        {
            // Could be made configurable in future
            return ConsoleThemes.Dark;
        }

        /// <summary>
        /// Validates command arguments before execution.
        /// </summary>
        /// <param name="args">The parsed command line arguments.</param>
        /// <param name="context">The execution context.</param>
        /// <returns>Exit code (0 if valid, non-zero if invalid).</returns>
        protected virtual int ValidateArguments(CommandLineArgs args, ShellExecutionContext context)
        {
            // Default implementation - derived classes can override
            return ExitCodes.Success;
        }

        /// <summary>
        /// Configures the help information for this command.
        /// </summary>
        /// <param name="help">The help information to configure.</param>
        protected virtual void ConfigureHelp(CommandHelp help)
        {
            // Override in derived classes to add command-specific help
        }

        /// <summary>
        /// Gets custom completion suggestions specific to this command.
        /// </summary>
        /// <param name="context">Execution context.</param>
        /// <param name="prefix">The prefix to match.</param>
        /// <returns>Custom completion suggestions.</returns>
        protected virtual string[] GetCustomCompletions(ShellExecutionContext context, string prefix)
        {
            return [];
        }

        /// <summary>
        /// Executes the command logic with parsed arguments.
        /// </summary>
        /// <param name="args">The parsed command line arguments.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Exit code (0 for success).</returns>
        protected abstract Task<int> ExecuteCommand(CommandLineArgs args, ShellExecutionContext context, CancellationToken cancellationToken);
    }
}
