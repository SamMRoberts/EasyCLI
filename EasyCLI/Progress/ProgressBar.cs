namespace EasyCLI.Progress
{
    /// <summary>
    /// Generates customizable progress bar strings for long-running operations.
    /// This is a pure formatter that builds strings; rendering is handled by IConsoleWriter.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ProgressBar"/> class.
    /// </remarks>
    /// <param name="width">The width of the progress bar in characters.</param>
    /// <param name="filledChar">The character to use for filled portions.</param>
    /// <param name="emptyChar">The character to use for empty portions.</param>
    /// <param name="showPercentage">Whether to show percentage text.</param>
    /// <param name="showFraction">Whether to show current/total fraction.</param>
    public class ProgressBar(
        int width = ProgressBar.DefaultWidth,
        char filledChar = ProgressBar.DefaultFilledChar,
        char emptyChar = ProgressBar.DefaultEmptyChar,
        bool showPercentage = true,
        bool showFraction = false)
    {
        /// <summary>
        /// Default progress bar width in characters.
        /// </summary>
        public const int DefaultWidth = 40;

        /// <summary>
        /// Default character used for filled portions of the progress bar.
        /// </summary>
        public const char DefaultFilledChar = '█';

        /// <summary>
        /// Default character used for empty portions of the progress bar.
        /// </summary>
        public const char DefaultEmptyChar = '░';

        private readonly int _width = Math.Max(1, width);
        private readonly char _filledChar = filledChar;
        private readonly char _emptyChar = emptyChar;
        private readonly bool _showPercentage = showPercentage;
        private readonly bool _showFraction = showFraction;

        /// <summary>
        /// Generates a progress bar string for the specified progress value.
        /// </summary>
        /// <param name="current">The current progress value.</param>
        /// <param name="total">The total/maximum progress value.</param>
        /// <returns>A formatted progress bar string.</returns>
        public string Render(long current, long total)
        {
            if (total <= 0)
            {
                return RenderIndeterminate();
            }

            double percentage = Math.Clamp((double)current / total, 0.0, 1.0);
            return RenderDeterminate(percentage, current, total);
        }

        /// <summary>
        /// Generates a progress bar string for the specified percentage (0.0 to 1.0).
        /// </summary>
        /// <param name="percentage">The progress percentage (0.0 to 1.0).</param>
        /// <returns>A formatted progress bar string.</returns>
        public string Render(double percentage)
        {
            percentage = Math.Clamp(percentage, 0.0, 1.0);
            return RenderDeterminate(percentage, null, null);
        }

        /// <summary>
        /// Generates an indeterminate progress bar (when total progress is unknown).
        /// </summary>
        /// <returns>An indeterminate progress bar string.</returns>
        public string RenderIndeterminate()
        {
            // For indeterminate progress, show a pattern that suggests motion
            string bar = new(_emptyChar, _width);
            string result = $"[{bar}]";

            if (_showPercentage)
            {
                result += " (working...)";
            }

            return result;
        }

        private string RenderDeterminate(double percentage, long? current, long? total)
        {
            int filledCount = (int)Math.Round(_width * percentage);
            filledCount = Math.Clamp(filledCount, 0, _width);
            int emptyCount = _width - filledCount;

            string filledPart = new(_filledChar, filledCount);
            string emptyPart = new(_emptyChar, emptyCount);
            string bar = $"[{filledPart}{emptyPart}]";

            List<string> suffixes = [];

            if (_showPercentage)
            {
                suffixes.Add($"{percentage * 100:F0}%");
            }

            if (_showFraction && current.HasValue && total.HasValue)
            {
                suffixes.Add($"{current.Value}/{total.Value}");
            }

            if (suffixes.Count > 0)
            {
                bar += " " + string.Join(" ", suffixes);
            }

            return bar;
        }

        /// <summary>
        /// Calculates the percentage for display formatting.
        /// </summary>
        /// <param name="current">Current progress value.</param>
        /// <param name="total">Total progress value.</param>
        /// <returns>Percentage as integer (0-100).</returns>
        public static int CalculatePercentage(long current, long total)
        {
            if (total <= 0)
            {
                return 0;
            }

            double percentage = (double)current / total;
            return (int)Math.Round(Math.Clamp(percentage * 100, 0, 100));
        }
    }
}
