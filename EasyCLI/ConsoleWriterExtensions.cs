namespace EasyCLI
{
    // Semantic convenience methods for styled output without changing the interface surface.
    public static class ConsoleWriterExtensions
    {
        // Write (no newline)
        public static void WriteSuccess(this IConsoleWriter w, string message) => w.Write(message, ConsoleStyles.Success);
        public static void WriteWarning(this IConsoleWriter w, string message) => w.Write(message, ConsoleStyles.Warning);
        public static void WriteError(this IConsoleWriter w, string message) => w.Write(message, ConsoleStyles.Error);
        public static void WriteHeading(this IConsoleWriter w, string message) => w.Write(message, ConsoleStyles.Heading);
        public static void WriteInfo(this IConsoleWriter w, string message) => w.Write(message, ConsoleStyles.Info);
        public static void WriteHint(this IConsoleWriter w, string message) => w.Write(message, ConsoleStyles.Hint);

        // WriteLine (with newline)
        public static void WriteSuccessLine(this IConsoleWriter w, string message) => w.WriteLine(message, ConsoleStyles.Success);
        public static void WriteWarningLine(this IConsoleWriter w, string message) => w.WriteLine(message, ConsoleStyles.Warning);
        public static void WriteErrorLine(this IConsoleWriter w, string message) => w.WriteLine(message, ConsoleStyles.Error);
        public static void WriteHeadingLine(this IConsoleWriter w, string message) => w.WriteLine(message, ConsoleStyles.Heading);
        public static void WriteInfoLine(this IConsoleWriter w, string message) => w.WriteLine(message, ConsoleStyles.Info);
        public static void WriteHintLine(this IConsoleWriter w, string message) => w.WriteLine(message, ConsoleStyles.Hint);
    }
}
