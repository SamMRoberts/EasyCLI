namespace EasyCLI
{
    /// <summary>
    /// Defines methods for writing styled and unstyled text to the console, supporting ANSI styling via <see cref="ConsoleStyle"/>.
    /// </summary>
    public interface IConsoleWriter
    {
        void Write(string message);
        void WriteLine(string message);
        void Write(string message, ConsoleStyle style);
        void WriteLine(string message, ConsoleStyle style);
    }
}
