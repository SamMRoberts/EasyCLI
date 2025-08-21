namespace EasyCLI
{
    public interface IConsoleWriter
    {
        void Write(string message);
        void WriteLine(string message);

    // Style-aware methods using ANSI SGR via ConsoleStyle
    void Write(string message, ConsoleStyle style);
    void WriteLine(string message, ConsoleStyle style);
    }
}
