using EasyCLI.Progress;

namespace EasyCLI.Shell.Commands
{
    /// <summary>
    /// Example command demonstrating progress indicators and early feedback patterns.
    /// Shows both progress bars and spinner animations for long-running operations.
    /// </summary>
    public class ProgressDemoCommand : BaseCliCommand
    {
        /// <summary>
        /// Gets the primary name of the command.
        /// </summary>
        public override string Name => "progress-demo";

        /// <summary>
        /// Gets a short description of the command.
        /// </summary>
        public override string Description => "Demonstrates progress indicators and early feedback patterns";

        /// <summary>
        /// Gets the category of the command for help organization.
        /// </summary>
        public override string Category => "Demo";

        /// <summary>
        /// Gets the detailed help information for this command.
        /// </summary>
        /// <returns>Command help information.</returns>
        public override CommandHelp GetHelp()
        {
            CommandHelp help = new()
            {
                Usage = "progress-demo [--spinner-only] [--progress-only] [--fast]",
                Description = Description,
            };

            help.Options.Add(new CommandOption("spinner-only", "s", "Show only spinner demo"));
            help.Options.Add(new CommandOption("progress-only", "p", "Show only progress bar demo"));
            help.Options.Add(new CommandOption("fast", "f", "Use fast timing for quick demonstration"));

            help.Examples.Add(new CommandExample("progress-demo", "Run full progress demonstration"));
            help.Examples.Add(new CommandExample("progress-demo --spinner-only", "Show only spinner animation"));
            help.Examples.Add(new CommandExample("progress-demo --progress-only --fast", "Quick progress bar demo"));

            return help;
        }

        /// <summary>
        /// Executes the progress demonstration command.
        /// </summary>
        /// <param name="args">The parsed command line arguments.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Exit code (0 for success).</returns>
        protected override async Task<int> ExecuteCommand(CommandLineArgs args, ShellExecutionContext context, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(args);
            ArgumentNullException.ThrowIfNull(context);
            ConsoleTheme theme = GetTheme(context);
            bool spinnerOnly = args.HasFlag("spinner-only");
            bool progressOnly = args.HasFlag("progress-only");
            bool fastMode = args.HasFlag("fast");

            // Timing adjustments for demo
            int longDelay = fastMode ? 200 : 1000;
            int shortDelay = fastMode ? 50 : 200;

            context.Writer.WriteHeadingLine("Progress Indicators Demonstration", theme);
            context.Writer.WriteLine("");

            if (args.IsDryRun)
            {
                context.Writer.WriteWarningLine("[DRY RUN] Would demonstrate progress indicators", theme);
                return ExitCodes.Success;
            }

            // Demo 1: Early Feedback Pattern
            if (!progressOnly)
            {
                await DemoEarlyFeedback(context, theme, longDelay, shortDelay, cancellationToken);
                context.Writer.WriteLine("");
            }

            // Demo 2: Spinner Animation
            if (!progressOnly)
            {
                await DemoSpinnerAnimation(context, theme, longDelay, shortDelay, cancellationToken);
                context.Writer.WriteLine("");
            }

            // Demo 3: Progress Bar
            if (!spinnerOnly)
            {
                await DemoProgressBar(context, theme, shortDelay, cancellationToken);
                context.Writer.WriteLine("");
            }

            // Demo 4: Combined Pattern
            if (!spinnerOnly && !progressOnly)
            {
                await DemoCombinedPattern(context, theme, longDelay, shortDelay, cancellationToken);
                context.Writer.WriteLine("");
            }

            context.Writer.WriteSuccessLine("✓ Progress demonstration completed", theme);
            return ExitCodes.Success;
        }

        private static async Task DemoEarlyFeedback(ShellExecutionContext context, ConsoleTheme theme, int longDelay, int shortDelay, CancellationToken cancellationToken)
        {
            context.Writer.WriteInfoLine("1. Early Feedback Pattern (100ms rule)", theme);
            context.Writer.WriteHintLine("Commands should provide feedback within 100ms for long operations", theme);
            context.Writer.WriteLine("");

            // Immediate feedback
            context.Writer.WriteStarting("database backup", theme);
            await Task.Delay(longDelay, cancellationToken);

            context.Writer.WriteStarting("file compression", theme);
            await Task.Delay(longDelay, cancellationToken);

            context.Writer.WriteCompleted("database backup", theme);
            await Task.Delay(shortDelay, cancellationToken);

            context.Writer.WriteCompleted("file compression", theme);
        }

        private static async Task DemoSpinnerAnimation(ShellExecutionContext context, ConsoleTheme theme, int longDelay, int shortDelay, CancellationToken cancellationToken)
        {
            context.Writer.WriteInfoLine("2. Spinner Animation with ProgressScope", theme);
            context.Writer.WriteHintLine("Use ProgressScope for indeterminate operations", theme);
            context.Writer.WriteLine("");

            // Basic spinner
            using (ProgressScope scope = context.Writer.CreateProgressScope("processing data", ProgressScope.DefaultSpinnerChars, theme, cancellationToken: cancellationToken))
            {
                await Task.Delay(longDelay * 2, cancellationToken);
                scope.Complete();
            }

            await Task.Delay(shortDelay, cancellationToken);

            // Braille spinner with message updates
            using (ProgressScope scope = context.Writer.CreateProgressScope("downloading files", ProgressScope.BrailleSpinnerChars, theme, cancellationToken: cancellationToken))
            {
                await Task.Delay(longDelay, cancellationToken);
                scope.UpdateMessage("verifying checksums");
                await Task.Delay(longDelay, cancellationToken);
                scope.UpdateMessage("extracting archives");
                await Task.Delay(longDelay, cancellationToken);
                scope.Complete("Files processed successfully");
            }

            await Task.Delay(shortDelay, cancellationToken);

            // Error scenario
            using (ProgressScope scope = context.Writer.CreateProgressScope("connecting to remote server", ProgressScope.DotsSpinnerChars, theme, cancellationToken: cancellationToken))
            {
                await Task.Delay(longDelay, cancellationToken);
                scope.Fail("Connection timeout");
            }
        }

        private static async Task DemoProgressBar(ShellExecutionContext context, ConsoleTheme theme, int shortDelay, CancellationToken cancellationToken)
        {
            context.Writer.WriteInfoLine("3. Progress Bar for Determinate Operations", theme);
            context.Writer.WriteHintLine("Use progress bars when you know total progress", theme);
            context.Writer.WriteLine("");

            // File processing simulation
            const int totalFiles = 50;
            context.Writer.WriteInfoLine($"Processing {totalFiles} files...", theme);

            for (int i = 0; i <= totalFiles; i++)
            {
                context.Writer.Write("\r");
                context.Writer.WriteProgressBar(i, (long)totalFiles, theme: theme);

                if (i < totalFiles)
                {
                    await Task.Delay(shortDelay / 4, cancellationToken);
                }
            }

            context.Writer.WriteLine("");
            context.Writer.WriteLine("");

            // Different progress bar styles
            context.Writer.WriteInfoLine("Different progress bar styles:", theme);

            // Standard with percentage and fraction
            context.Writer.Write("Standard: ");
            context.Writer.WriteProgressBarLine(75L, 100L, showFraction: true, theme: theme);

            // Custom characters
            context.Writer.Write("Custom:   ");
            context.Writer.WriteProgressBarLine(75L, 100L, filledChar: '#', emptyChar: '-', theme: theme);

            // Percentage only
            context.Writer.Write("Compact:  ");
            context.Writer.WriteProgressBarLine(0.75, width: 20, theme: theme);
        }

        private static async Task DemoCombinedPattern(ShellExecutionContext context, ConsoleTheme theme, int longDelay, int shortDelay, CancellationToken cancellationToken)
        {
            context.Writer.WriteInfoLine("4. Combined Pattern: Spinner + Progress Bar", theme);
            context.Writer.WriteHintLine("Start with spinner, switch to progress bar when total is known", theme);
            context.Writer.WriteLine("");

            // Start with indeterminate spinner
            using (ProgressScope scope = context.Writer.CreateProgressScope("analyzing files", theme: theme, cancellationToken: cancellationToken))
            {
                await Task.Delay(longDelay, cancellationToken);
                scope.Complete("Found 25 files to process");
            }

            await Task.Delay(shortDelay, cancellationToken);

            // Switch to determinate progress bar
            const int totalSteps = 25;
            context.Writer.WriteInfoLine("Processing files:", theme);

            for (int i = 0; i <= totalSteps; i++)
            {
                context.Writer.Write("\r");
                context.Writer.WriteProgressBar(i, totalSteps, showFraction: true, theme: theme);

                if (i < totalSteps)
                {
                    await Task.Delay(shortDelay / 3, cancellationToken);
                }
            }

            context.Writer.WriteLine("");
            context.Writer.WriteCompleted("file processing", theme);
        }
    }
}
