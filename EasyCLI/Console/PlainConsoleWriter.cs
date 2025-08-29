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

        [GeneratedRegex(@"[✓✗⚠•◉◯▶▷►‣⚡✨🔥💡⭐🎯📝📊📈📉🔍🔧⚙️🛠️🎨🎭🔄⏳⏰⏲️⏱️🚀🎪🎁🎊🎉🌟💎🏆🥇🏅🎖️🔔🔕ℹ]")] // Common CLI symbols
        private static partial Regex SymbolsRegex();

        [GeneratedRegex(@"[─━┌┐└┘├┤┬┴┼│┃╔╗╚╝╠╣╦╩╬║═╒╓╕╖╘╙╛╜╞╟╡╢╤╥╧╨╪╫╬]+", RegexOptions.Compiled)] // Box drawing characters
        private static partial Regex BoxDrawingRegex();

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

            // Replace decorative symbols with simpler alternatives or remove them
            text = SymbolsRegex().Replace(text, string.Empty);

            // Replace box drawing characters with simple alternatives
            text = BoxDrawingRegex().Replace(text, "-");

            // Clean up excessive whitespace that might result from symbol removal
            text = text.Trim();

            // Remove empty lines that might be left from decorative elements
            return string.IsNullOrWhiteSpace(text) ? string.Empty : text;
        }
    }
}
