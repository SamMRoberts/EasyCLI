using EasyCLI.Deprecation;
using EasyCLI.Extensions;
using EasyCLI.Shell.Utilities;

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
        /// Gets the category of the command for help organization.
        /// Override in derived classes to specify a custom category.
        /// </summary>
        public virtual string Category => "General";

        /// <summary>
        /// Gets a value indicating whether to show concise help when no arguments are provided
        /// instead of validation errors. Default is true for better CLI UX.
        /// </summary>
        protected virtual bool ShowConciseHelpOnNoArguments => true;

        /// <summary>
        /// Confirms a dangerous operation with the user, respecting automation flags and environment context.
        /// </summary>
        /// <param name="operation">Description of the operation to be performed.</param>
        /// <param name="context">The shell execution context.</param>
        /// <param name="args">The parsed command line arguments.</param>
        /// <param name="additionalWarnings">Optional additional warnings to display.</param>
        /// <param name="customPrompt">Optional custom confirmation prompt text.</param>
        /// <returns>True if the operation should proceed, false if cancelled.</returns>
        protected static bool ConfirmDangerous(
            string operation,
            ShellExecutionContext context,
            CommandLineArgs args,
            string[]? additionalWarnings = null,
            string? customPrompt = null)
        {
            return DangerousOperationConfirmation.ConfirmDangerous(
                operation, context, args, additionalWarnings, customPrompt);
        }

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
            help.Options.Add(new CommandOption("yes", "y", "Confirm dangerous operations without prompting"));
            help.Options.Add(new CommandOption("force", "f", "Force execution, bypassing safety checks"));
            help.Options.Add(new CommandOption("dry-run", "n", "Show what would be done without executing"));

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

                // Check for no arguments and show concise help if enabled
                // Show concise help when there are no positional arguments
                if (ShowConciseHelpOnNoArguments && parsedArgs.Arguments.Count == 0)
                {
                    ShowConciseHelp(context);
                    return ExitCodes.Success;
                }

                // Validate arguments before execution
                int validationResult = ValidateArguments(parsedArgs, context);
                if (validationResult != ExitCodes.Success)
                {
                    return validationResult;
                }

                // Check for deprecated features and display warnings
                CheckDeprecatedFeatures(parsedArgs, context);

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

            // Scripting guidance section
            WriteScriptingGuidance(context, theme);

            // Standard footer with support paths and version
            HelpFooter.WriteFooter(context.Writer, theme);
        }

        /// <summary>
        /// Shows concise help when no arguments are provided.
        /// </summary>
        /// <param name="context">The execution context.</param>
        protected virtual void ShowConciseHelp(ShellExecutionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            CommandHelp help = GetHelp();
            ConsoleTheme theme = GetTheme(context);

            // Brief description
            context.Writer.WriteLine($"{Name} - {Description}");
            context.Writer.WriteLine("");

            // Usage
            context.Writer.WriteInfoLine("USAGE:", theme);
            context.Writer.WriteLine($"  {help.Usage}");
            context.Writer.WriteLine("");

            // Show first 1-2 examples if available
            if (help.Examples.Count > 0)
            {
                context.Writer.WriteInfoLine("EXAMPLES:", theme);
                int exampleCount = Math.Min(2, help.Examples.Count);
                for (int i = 0; i < exampleCount; i++)
                {
                    CommandExample example = help.Examples[i];
                    context.Writer.WriteLine($"  {example.Command}");
                    context.Writer.WriteHintLine($"    {example.Description}", theme);
                    if (i < exampleCount - 1)
                    {
                        context.Writer.WriteLine("");
                    }
                }
                context.Writer.WriteLine("");
            }

            // Pointer to full help
            context.Writer.WriteHintLine($"For more information, run: {Name} --help", theme);
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
        /// Writes scripting guidance section to help output.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="theme">The console theme to use.</param>
        protected virtual void WriteScriptingGuidance(ShellExecutionContext context, ConsoleTheme theme)
        {
            ArgumentNullException.ThrowIfNull(context);

            context.Writer.WriteInfoLine("SCRIPTING:", theme);
            context.Writer.WriteLine("  For automation and scripts, use explicit output formats:");
            context.Writer.WriteLine($"    {Name} --json    # Machine-readable JSON output");
            context.Writer.WriteLine($"    {Name} --plain   # Script-friendly plain text");
            context.Writer.WriteLine("");
            context.Writer.WriteLine("  Handle errors properly in scripts:");
            context.Writer.WriteLine($"    if ! result=$({Name} --json 2>/dev/null); then");
            context.Writer.WriteLine("      echo \"Command failed with exit code $?\"");
            context.Writer.WriteLine("      exit 1");
            context.Writer.WriteLine("    fi");
            context.Writer.WriteLine("");
            context.Writer.WriteHintLine("  Default table format may change between versions - avoid in scripts", theme);
            context.Writer.WriteLine("");
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
        /// Checks for deprecated features and displays appropriate warnings.
        /// </summary>
        /// <param name="args">The parsed command line arguments.</param>
        /// <param name="context">The execution context.</param>
        protected virtual void CheckDeprecatedFeatures(CommandLineArgs args, ShellExecutionContext context)
        {
            ArgumentNullException.ThrowIfNull(args);
            ArgumentNullException.ThrowIfNull(context);

            // Get command help information for deprecation checking
            var help = GetHelp();
            var theme = GetTheme(context);

            // Check if this command itself is deprecated
            DeprecationChecker.CheckAndWarnDeprecatedCommand(this, context.Writer, theme);

            // Check for deprecated options and arguments
            DeprecationChecker.PerformComprehensiveDeprecationCheck(this, args, help, context.Writer, theme);
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
        /// Suggests similar valid options for a mistyped or unknown option.
        /// </summary>
        /// <param name="unknownOption">The unknown option that was provided.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="availableOptions">Optional list of available options. If null, uses common CLI options.</param>
        protected virtual void SuggestSimilarOption(string unknownOption, ShellExecutionContext context, IEnumerable<string>? availableOptions = null)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (string.IsNullOrEmpty(unknownOption))
            {
                return;
            }

            // Default to common CLI options if no specific options provided
            availableOptions ??= ["--help", "--verbose", "--quiet", "--dry-run", "--force", "--yes", "--config", "--output"];

            string? suggestion = LevenshteinDistance.FindBestMatch(unknownOption, availableOptions);

            if (!string.IsNullOrEmpty(suggestion))
            {
                ShowSuggestion(context, $"Did you mean '{suggestion}'?");
            }
            else
            {
                // If no close match found, show available options
                IEnumerable<string> sortedOptions = availableOptions.OrderBy(o => o).Take(5);
                ShowSuggestion(context, $"Available options: {string.Join(", ", sortedOptions)}");
            }
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
