using System.Text.RegularExpressions;

namespace EasyCLI.Console
{
    /// <summary>
    /// A decorator for IConsoleWriter that strips all colors, symbols, and decorative padding to provide plain text output.
    /// This class implements the --plain flag functionality by normalizing styled output to simple text.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="PlainConsoleWriter"/> class.
    /// </remarks>
    /// <param name="inner">The underlying console writer to decorate.</param>
    public partial class PlainConsoleWriter(IConsoleWriter inner) : IConsoleWriter
    {
        private readonly IConsoleWriter _inner = inner ?? throw new ArgumentNullException(nameof(inner));

        // Regex patterns for stripping decorative elements
        [GeneratedRegex(@"\u001b\[[0-9;]*m")] // ANSI escape sequences
        private static partial Regex AnsiEscapeRegex();

        [GeneratedRegex(@"[─━┌┐└┘├┤┬┴┼│┃╔╗╚╝╠╣╦╩╬║═╒╓╕╖╘╙╛╜╞╟╡╢╤╥╧╨╪╫╬]+", RegexOptions.Compiled)] // Box drawing characters
        private static partial Regex BoxDrawingRegex();

        /// <summary>
        /// Common CLI symbols organized by category for maintainability.
        /// These symbols are stripped from output in plain mode.
        /// </summary>
        private static class CliSymbols
        {
            /// <summary>Status indicator symbols (success, warning, error, info).</summary>
            public static readonly HashSet<string> StatusSymbols =
            [
                "✓", // Success/checkmark
                "✗", // Error/cross
                "⚠", // Warning
                "ℹ", // Information
            ];

            /// <summary>Bullet point and list indicator symbols.</summary>
            public static readonly HashSet<string> BulletSymbols =
            [
                "•", // Bullet point
                "◉", // Large filled circle
                "◯", // Large empty circle
                "‣", // Triangular bullet
            ];

            /// <summary>Directional and navigation symbols.</summary>
            public static readonly HashSet<string> DirectionalSymbols =
            [
                "▶", // Play/right triangle filled
                "▷", // Play/right triangle outline
                "►", // Right pointer
            ];

            /// <summary>Special effect and emphasis symbols.</summary>
            public static readonly HashSet<string> EffectSymbols =
            [
                "⚡", // Lightning
                "✨", // Sparkles
                "🔥", // Fire
                "💡", // Light bulb
                "🌟", // Star
            ];

            /// <summary>Goal and achievement symbols.</summary>
            public static readonly HashSet<string> AchievementSymbols =
            [
                "⭐", // Star
                "🎯", // Target
                "🏆", // Trophy
                "🥇", // Gold medal
                "🏅", // Medal
                "🎖", // Military medal
                "💎", // Diamond
            ];

            /// <summary>Document and data symbols.</summary>
            public static readonly HashSet<string> DocumentSymbols =
            [
                "📝", // Memo
                "📊", // Bar chart
                "📈", // Chart increasing
                "📉", // Chart decreasing
            ];

            /// <summary>Tool and utility symbols.</summary>
            public static readonly HashSet<string> ToolSymbols =
            [
                "🔍", // Magnifying glass
                "🔧", // Wrench
                "⚙", // Gear
                "🛠", // Hammer and wrench
            ];

            /// <summary>Creative and theme symbols.</summary>
            public static readonly HashSet<string> CreativeSymbols =
            [
                "🎨", // Artist palette
                "🎭", // Theater masks
            ];

            /// <summary>Time and process symbols.</summary>
            public static readonly HashSet<string> TimeSymbols =
            [
                "🔄", // Counterclockwise arrows
                "⏳", // Hourglass flowing
                "⏰", // Alarm clock
                "⏲", // Timer clock
                "⏱", // Stopwatch
            ];

            /// <summary>Celebration and event symbols.</summary>
            public static readonly HashSet<string> CelebrationSymbols =
            [
                "🚀", // Rocket
                "🎪", // Circus tent
                "🎁", // Gift
                "🎊", // Confetti ball
                "🎉", // Party popper
            ];

            /// <summary>Notification symbols.</summary>
            public static readonly HashSet<string> NotificationSymbols =
            [
                "🔔", // Bell
                "🔕", // Bell with slash
            ];

            /// <summary>Combined set of all CLI symbols for efficient lookup.</summary>
            public static readonly HashSet<string> AllSymbols = [
                ..StatusSymbols,
                ..BulletSymbols,
                ..DirectionalSymbols,
                ..EffectSymbols,
                ..AchievementSymbols,
                ..DocumentSymbols,
                ..ToolSymbols,
                ..CreativeSymbols,
                ..TimeSymbols,
                ..CelebrationSymbols,
                ..NotificationSymbols
            ];
        }

        /// <summary>
        /// Writes the specified message to the console without a newline, with all styling stripped.
        /// </summary>
        /// <param name="message">The message to write to the console.</param>
        public void Write(string message)
        {
            _inner.Write(NormalizeText(message));
        }

        /// <summary>
        /// Writes the specified message to the console followed by a newline, with all styling stripped.
        /// </summary>
        /// <param name="message">The message to write to the console.</param>
        public void WriteLine(string message)
        {
            _inner.WriteLine(NormalizeText(message));
        }

        /// <summary>
        /// Writes the specified message to the console with styling stripped (ignores the style parameter).
        /// </summary>
        /// <param name="message">The message to write to the console.</param>
        /// <param name="style">The style to apply to the message (ignored in plain mode).</param>
        public void Write(string message, ConsoleStyle style)
        {
            _inner.Write(NormalizeText(message));
        }

        /// <summary>
        /// Writes the specified message to the console with styling stripped, followed by a newline (ignores the style parameter).
        /// </summary>
        /// <param name="message">The message to write to the console.</param>
        /// <param name="style">The style to apply to the message (ignored in plain mode).</param>
        public void WriteLine(string message, ConsoleStyle style)
        {
            _inner.WriteLine(NormalizeText(message));
        }

        /// <summary>
        /// Normalizes text by removing ANSI escape sequences, decorative symbols, and box drawing characters.
        /// </summary>
        /// <param name="text">The text to normalize.</param>
        /// <returns>Plain text with all decorative elements removed.</returns>
        private static string NormalizeText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            // Remove ANSI escape sequences
            text = AnsiEscapeRegex().Replace(text, string.Empty);

            // Remove decorative symbols using efficient HashSet lookup
            text = RemoveCliSymbols(text);

            // Replace box drawing characters with simple alternatives
            text = BoxDrawingRegex().Replace(text, "-");

            // Clean up excessive whitespace that might result from symbol removal
            text = text.Trim();

            // Remove empty lines that might be left from decorative elements
            return string.IsNullOrWhiteSpace(text) ? string.Empty : text;
        }

        /// <summary>
        /// Removes CLI symbols from text using efficient HashSet lookup.
        /// This provides better performance than regex for symbol filtering.
        /// </summary>
        /// <param name="text">The text to process.</param>
        /// <returns>Text with CLI symbols removed.</returns>
        private static string RemoveCliSymbols(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            string result = text;
            foreach (string symbol in CliSymbols.AllSymbols)
            {
                result = result.Replace(symbol, string.Empty);
            }

            return result;
        }
    }
}
