namespace EasyCLI.Console
{
    /// <summary>
    /// Reads from the console. Honors input redirection for testability.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ConsoleReader"/> class.
    /// </remarks>
    /// <param name="input">The text reader to use for input. If null, uses <see cref="System.Console.In"/>.</param>
    public class ConsoleReader(TextReader? input = null) : IConsoleReader
    {

        /// <summary>
        /// Gets the text reader used for input.
        /// </summary>
        public TextReader Input { get; } = input ?? System.Console.In;

        /// <summary>
        /// Reads a line of input from the console.
        /// </summary>
        /// <returns>A string containing the line read from the console, or an empty string if no input is available.</returns>
        public string ReadLine()
        {
            return Input.ReadLine() ?? string.Empty;
        }
    }
}
