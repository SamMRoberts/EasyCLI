using System;
using System.IO;

namespace EasyCLI.Console
{
    /// <summary>
    /// Reads from the console. Honors input redirection for testability.
    /// </summary>
    public class ConsoleReader : IConsoleReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleReader"/> class.
        /// </summary>
        /// <param name="input">The text reader to use for input. If null, uses <see cref="Console.In"/>.</param>
        public ConsoleReader(TextReader? input = null)
        {
            Input = input ?? System.Console.In;
        }

        /// <summary>
        /// Gets the text reader used for input.
        /// </summary>
        public TextReader Input { get; }

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
