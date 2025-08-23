using System;

namespace EasyCLI.Prompts
{
    /// <summary>
    /// Defines methods for reading keyboard input.
    /// </summary>
    public interface IKeyReader
    {
        /// <summary>
        /// Reads the next key pressed by the user.
        /// </summary>
        /// <param name="intercept">If true, the key is not displayed in the console window.</param>
        /// <returns>Information about the key that was pressed.</returns>
        ConsoleKeyInfo ReadKey(bool intercept = true);
    }

    /// <summary>
    /// Default implementation of <see cref="IKeyReader"/> that reads from the console.
    /// </summary>
    public sealed class ConsoleKeyReader : IKeyReader
    {
        /// <summary>
        /// Reads the next key pressed by the user from the console.
        /// </summary>
        /// <param name="intercept">If true, the key is not displayed in the console window.</param>
        /// <returns>Information about the key that was pressed.</returns>
        public ConsoleKeyInfo ReadKey(bool intercept = true) => System.Console.ReadKey(intercept);
    }
}
