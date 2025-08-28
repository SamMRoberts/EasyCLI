using EasyCLI.Console;
using EasyCLI.Environment;
using EasyCLI.Prompts;

namespace EasyCLI.Shell
{
    /// <summary>
    /// Utilities for confirming dangerous operations with appropriate safeguards.
    /// </summary>
    public static class DangerousOperationConfirmation
    {
        /// <summary>
        /// Confirms a dangerous operation with the user, respecting automation flags and environment context.
        /// </summary>
        /// <param name="operation">Description of the operation to be performed.</param>
        /// <param name="context">The shell execution context.</param>
        /// <param name="args">The parsed command line arguments.</param>
        /// <param name="additionalWarnings">Optional additional warnings to display.</param>
        /// <param name="customPrompt">Optional custom confirmation prompt text.</param>
        /// <returns>True if the operation should proceed, false if cancelled.</returns>
        public static bool ConfirmDangerous(
            string operation,
            ShellExecutionContext context,
            CommandLineArgs args,
            string[]? additionalWarnings = null,
            string? customPrompt = null)
        {
            ArgumentNullException.ThrowIfNull(operation);
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(args);

            // Check for explicit bypass flags first
            if (args.IsYes || args.IsForce)
            {
                return true;
            }

            // Detect automation context
            if (IsAutomationContext(context))
            {
                context.Writer.WriteErrorLine("Dangerous operation attempted in automation context without explicit confirmation.");
                context.Writer.WriteErrorLine($"Operation: {operation}");
                context.Writer.WriteHintLine("Use --yes or --force to confirm dangerous operations in automation.");
                return false;
            }

            // Interactive confirmation
            return GetInteractiveConfirmation(operation, context, additionalWarnings, customPrompt);
        }

        /// <summary>
        /// Determines if the current context is an automation environment.
        /// </summary>
        /// <param name="context">The shell execution context.</param>
        /// <returns>True if running in automation context.</returns>
        private static bool IsAutomationContext(ShellExecutionContext context)
        {
            var environment = EnvironmentDetector.DetectEnvironment();

            // Check if running in non-interactive mode or CI environment
            return !environment.IsInteractive || environment.IsContinuousIntegration;
        }

        /// <summary>
        /// Gets interactive confirmation from the user.
        /// </summary>
        /// <param name="operation">Description of the operation.</param>
        /// <param name="context">The shell execution context.</param>
        /// <param name="additionalWarnings">Additional warnings to display.</param>
        /// <param name="customPrompt">Custom confirmation prompt text.</param>
        /// <returns>True if the user confirms the operation.</returns>
        private static bool GetInteractiveConfirmation(
            string operation,
            ShellExecutionContext context,
            string[]? additionalWarnings,
            string? customPrompt)
        {
            // Display operation warning
            context.Writer.WriteWarningLine("⚠️  DANGEROUS OPERATION");
            context.Writer.WriteInfoLine($"Operation: {operation}");

            // Display additional warnings if provided
            if (additionalWarnings != null && additionalWarnings.Length > 0)
            {
                context.Writer.WriteLine(string.Empty);
                foreach (var warning in additionalWarnings)
                {
                    context.Writer.WriteWarningLine($"⚠️  {warning}");
                }
            }

            context.Writer.WriteLine(string.Empty);

            // Get confirmation
            var prompt = customPrompt ?? "Do you want to proceed with this dangerous operation?";
            var confirmation = new YesNoPrompt(prompt, context.Writer, context.Reader, @default: false);

            try
            {
                return confirmation.GetValue();
            }
            catch (OperationCanceledException)
            {
                context.Writer.WriteInfoLine("Operation cancelled by user.");
                return false;
            }
        }
    }
}
