namespace EasyCLI.Console
{
    /// <summary>
    /// Defines methods for writing styled and unstyled text to the console, supporting ANSI styling via <see cref="ConsoleStyle"/>.
    /// </summary>
    public interface IConsoleWriter
    {
        /// <summary>
        /// Writes the specified message to the console without a newline.
        /// </summary>
        /// <param name="message">The message to write to the console.</param>
        void Write(string message);

        /// <summary>
        /// Writes the specified message to the console followed by a newline.
        /// </summary>
        /// <param name="message">The message to write to the console.</param>
        void WriteLine(string message);

        /// <summary>
        /// Style-aware methods using ANSI SGR via <see cref="ConsoleStyle"/>.
        /// </summary>
        /// <param name="message">The message to write to the console.</param>
        /// <param name="style">The style to apply to the message.</param>
        void Write(string message, ConsoleStyle style);

        /// <summary>
        /// Writes a line to the console with the specified message and applies the given <see cref="ConsoleStyle"/>.
        /// </summary>
        /// <param name="message">The message to write to the console.</param>
        /// <param name="style">The style to apply to the message.</param>
        void WriteLine(string message, ConsoleStyle style);
    }
}
