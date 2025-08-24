namespace EasyCLI.Console
{
    /// <summary>
    /// Defines a method for reading input from the console.
    /// </summary>
    public interface IConsoleReader
    {
        /// <summary>
        /// Reads a line of input from the console.
        /// </summary>
        /// <returns>A string containing the line read from the console, or an empty string if no input is available.</returns>
        string ReadLine();
    }
}
