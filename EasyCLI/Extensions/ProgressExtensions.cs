using EasyCLI.Console;
using EasyCLI.Progress;

namespace EasyCLI.Extensions
{
    /// <summary>
    /// Extension methods for progress indicators and early feedback patterns.
    /// </summary>
    public static class ProgressExtensions
    {
        /// <summary>
        /// Writes a progress bar to the console using the specified progress values.
        /// </summary>
        /// <param name="writer">The console writer to write to.</param>
        /// <param name="current">The current progress value.</param>
        /// <param name="total">The total/maximum progress value.</param>
        /// <param name="width">The width of the progress bar. Defaults to ProgressBar.DefaultWidth.</param>
        /// <param name="filledChar">The character for filled portions. Defaults to ProgressBar.DefaultFilledChar.</param>
        /// <param name="emptyChar">The character for empty portions. Defaults to ProgressBar.DefaultEmptyChar.</param>
        /// <param name="showPercentage">Whether to show percentage. Defaults to true.</param>
        /// <param name="showFraction">Whether to show current/total fraction. Defaults to false.</param>
        /// <param name="theme">The theme to use for styling.</param>
        public static void WriteProgressBar(
            this IConsoleWriter writer,
            long current,
            long total,
            int width = ProgressBar.DefaultWidth,
            char filledChar = ProgressBar.DefaultFilledChar,
            char emptyChar = ProgressBar.DefaultEmptyChar,
            bool showPercentage = true,
            bool showFraction = false,
            ConsoleTheme? theme = null)
        {
            ArgumentNullException.ThrowIfNull(writer);

            ProgressBar progressBar = new(width, filledChar, emptyChar, showPercentage, showFraction);
            string progressString = progressBar.Render(current, total);

            if (theme != null)
            {
                writer.Write(progressString, theme.Info);
            }
            else
            {
                writer.Write(progressString);
            }
        }

        /// <summary>
        /// Writes a progress bar to the console followed by a newline.
        /// </summary>
        /// <param name="writer">The console writer to write to.</param>
        /// <param name="current">The current progress value.</param>
        /// <param name="total">The total/maximum progress value.</param>
        /// <param name="width">The width of the progress bar. Defaults to ProgressBar.DefaultWidth.</param>
        /// <param name="filledChar">The character for filled portions. Defaults to ProgressBar.DefaultFilledChar.</param>
        /// <param name="emptyChar">The character for empty portions. Defaults to ProgressBar.DefaultEmptyChar.</param>
        /// <param name="showPercentage">Whether to show percentage. Defaults to true.</param>
        /// <param name="showFraction">Whether to show current/total fraction. Defaults to false.</param>
        /// <param name="theme">The theme to use for styling.</param>
        public static void WriteProgressBarLine(
            this IConsoleWriter writer,
            long current,
            long total,
            int width = ProgressBar.DefaultWidth,
            char filledChar = ProgressBar.DefaultFilledChar,
            char emptyChar = ProgressBar.DefaultEmptyChar,
            bool showPercentage = true,
            bool showFraction = false,
            ConsoleTheme? theme = null)
        {
            writer.WriteProgressBar(current, total, width, filledChar, emptyChar, showPercentage, showFraction, theme);
            writer.WriteLine(string.Empty);
        }

        /// <summary>
        /// Writes a progress bar using percentage (0.0 to 1.0) to the console.
        /// </summary>
        /// <param name="writer">The console writer to write to.</param>
        /// <param name="percentage">The progress percentage (0.0 to 1.0).</param>
        /// <param name="width">The width of the progress bar. Defaults to ProgressBar.DefaultWidth.</param>
        /// <param name="filledChar">The character for filled portions. Defaults to ProgressBar.DefaultFilledChar.</param>
        /// <param name="emptyChar">The character for empty portions. Defaults to ProgressBar.DefaultEmptyChar.</param>
        /// <param name="showPercentage">Whether to show percentage. Defaults to true.</param>
        /// <param name="theme">The theme to use for styling.</param>
        public static void WriteProgressBar(
            this IConsoleWriter writer,
            double percentage,
            int width = ProgressBar.DefaultWidth,
            char filledChar = ProgressBar.DefaultFilledChar,
            char emptyChar = ProgressBar.DefaultEmptyChar,
            bool showPercentage = true,
            ConsoleTheme? theme = null)
        {
            ArgumentNullException.ThrowIfNull(writer);

            ProgressBar progressBar = new(width, filledChar, emptyChar, showPercentage, false);
            string progressString = progressBar.Render(percentage);

            if (theme != null)
            {
                writer.Write(progressString, theme.Info);
            }
            else
            {
                writer.Write(progressString);
            }
        }

        /// <summary>
        /// Writes a progress bar using percentage (0.0 to 1.0) followed by a newline.
        /// </summary>
        /// <param name="writer">The console writer to write to.</param>
        /// <param name="percentage">The progress percentage (0.0 to 1.0).</param>
        /// <param name="width">The width of the progress bar. Defaults to ProgressBar.DefaultWidth.</param>
        /// <param name="filledChar">The character for filled portions. Defaults to ProgressBar.DefaultFilledChar.</param>
        /// <param name="emptyChar">The character for empty portions. Defaults to ProgressBar.DefaultEmptyChar.</param>
        /// <param name="showPercentage">Whether to show percentage. Defaults to true.</param>
        /// <param name="theme">The theme to use for styling.</param>
        public static void WriteProgressBarLine(
            this IConsoleWriter writer,
            double percentage,
            int width = ProgressBar.DefaultWidth,
            char filledChar = ProgressBar.DefaultFilledChar,
            char emptyChar = ProgressBar.DefaultEmptyChar,
            bool showPercentage = true,
            ConsoleTheme? theme = null)
        {
            writer.WriteProgressBar(percentage, width, filledChar, emptyChar, showPercentage, theme);
            writer.WriteLine(string.Empty);
        }

        /// <summary>
        /// Creates a new progress scope for long-running operations with spinner animation.
        /// Immediately shows starting message for early feedback (100ms requirement).
        /// </summary>
        /// <param name="writer">The console writer to use for output.</param>
        /// <param name="message">The message to display alongside the spinner.</param>
        /// <param name="spinnerChars">The characters to use for the spinner animation.</param>
        /// <param name="theme">The theme to use for styling.</param>
        /// <param name="intervalMs">The animation interval in milliseconds. Defaults to 100ms.</param>
        /// <param name="cancellationToken">Token to observe for cancellation.</param>
        /// <returns>A disposable ProgressScope that manages the spinner lifecycle.</returns>
        public static ProgressScope CreateProgressScope(
            this IConsoleWriter writer,
            string message,
            char[]? spinnerChars = null,
            ConsoleTheme? theme = null,
            int intervalMs = 100,
            CancellationToken cancellationToken = default)
        {
            return new ProgressScope(writer, message, spinnerChars, theme, intervalMs, cancellationToken);
        }

        /// <summary>
        /// Immediately writes a "Starting..." message for early feedback in long-running operations.
        /// This method ensures users get feedback within 100ms as per CLI best practices.
        /// </summary>
        /// <param name="writer">The console writer to write to.</param>
        /// <param name="operationName">The name of the operation starting.</param>
        /// <param name="theme">The theme to use for styling.</param>
        public static void WriteStarting(this IConsoleWriter writer, string operationName, ConsoleTheme? theme = null)
        {
            ArgumentNullException.ThrowIfNull(writer);
            ArgumentException.ThrowIfNullOrWhiteSpace(operationName);

            string message = $"Starting {operationName}...";
            if (theme != null)
            {
                writer.WriteLine(message, theme.Info);
            }
            else
            {
                writer.WriteLine(message);
            }
        }

        /// <summary>
        /// Writes a completion message for operations that were started with WriteStarting.
        /// </summary>
        /// <param name="writer">The console writer to write to.</param>
        /// <param name="operationName">The name of the operation that completed.</param>
        /// <param name="theme">The theme to use for styling.</param>
        public static void WriteCompleted(this IConsoleWriter writer, string operationName, ConsoleTheme? theme = null)
        {
            ArgumentNullException.ThrowIfNull(writer);
            ArgumentException.ThrowIfNullOrWhiteSpace(operationName);

            string message = $"✓ {operationName} completed";
            if (theme != null)
            {
                writer.WriteLine(message, theme.Success);
            }
            else
            {
                writer.WriteLine(message);
            }
        }

        /// <summary>
        /// Writes a failure message for operations that were started with WriteStarting.
        /// </summary>
        /// <param name="writer">The console writer to write to.</param>
        /// <param name="operationName">The name of the operation that failed.</param>
        /// <param name="errorMessage">Optional error details.</param>
        /// <param name="theme">The theme to use for styling.</param>
        public static void WriteFailed(this IConsoleWriter writer, string operationName, string? errorMessage = null, ConsoleTheme? theme = null)
        {
            ArgumentNullException.ThrowIfNull(writer);
            ArgumentException.ThrowIfNullOrWhiteSpace(operationName);

            string message = $"✗ {operationName} failed";
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                message += $": {errorMessage}";
            }

            if (theme != null)
            {
                writer.WriteLine(message, theme.Error);
            }
            else
            {
                writer.WriteLine(message);
            }
        }
    }
}
