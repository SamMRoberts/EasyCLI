using System;
using System.IO;

namespace EasyCLI.Console
{
    /// <summary>
    /// Reads from the console. Honors input redirection for testability.
    /// </summary>
    public class ConsoleReader : IConsoleReader
    {
        public ConsoleReader(TextReader? input = null)
        {
            Input = input ?? System.Console.In;
        }

        public TextReader Input { get; }

        public string ReadLine()
        {
            return Input.ReadLine() ?? string.Empty;
        }
    }
}
