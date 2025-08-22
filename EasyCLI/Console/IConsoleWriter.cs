namespace EasyCLI
{
    public interface IConsoleWriter
    {
        void Write(string message);
        void WriteLine(string message);

        /// <summary>
        /// Style-aware methods using ANSI SGR via <see cref="ConsoleStyle"/>.
        /// </summary>
        void Write(string message, ConsoleStyle style);
        void WriteLine(string message, ConsoleStyle style);
    }
}
