namespace EasyCLI
{
    // Semantic convenience methods for styled output without changing the interface surface.
    public static class ConsoleWriterExtensions
    {
        public static void WriteSuccess(this IConsoleWriter w, string message, ConsoleTheme? theme = null) => w.Write(message, (theme ?? DefaultTheme).Success);
        public static void WriteWarning(this IConsoleWriter w, string message, ConsoleTheme? theme = null) => w.Write(message, (theme ?? DefaultTheme).Warning);
        public static void WriteError(this IConsoleWriter w, string message, ConsoleTheme? theme = null) => w.Write(message, (theme ?? DefaultTheme).Error);
        public static void WriteHeading(this IConsoleWriter w, string message, ConsoleTheme? theme = null) => w.Write(message, (theme ?? DefaultTheme).Heading);
        public static void WriteInfo(this IConsoleWriter w, string message, ConsoleTheme? theme = null) => w.Write(message, (theme ?? DefaultTheme).Info);
        public static void WriteHint(this IConsoleWriter w, string message, ConsoleTheme? theme = null) => w.Write(message, (theme ?? DefaultTheme).Hint);

        public static void WriteSuccessLine(this IConsoleWriter w, string message, ConsoleTheme? theme = null) => w.WriteLine(message, (theme ?? DefaultTheme).Success);
        public static void WriteWarningLine(this IConsoleWriter w, string message, ConsoleTheme? theme = null) => w.WriteLine(message, (theme ?? DefaultTheme).Warning);
        public static void WriteErrorLine(this IConsoleWriter w, string message, ConsoleTheme? theme = null) => w.WriteLine(message, (theme ?? DefaultTheme).Error);
        public static void WriteHeadingLine(this IConsoleWriter w, string message, ConsoleTheme? theme = null) => w.WriteLine(message, (theme ?? DefaultTheme).Heading);
        public static void WriteInfoLine(this IConsoleWriter w, string message, ConsoleTheme? theme = null) => w.WriteLine(message, (theme ?? DefaultTheme).Info);
        public static void WriteHintLine(this IConsoleWriter w, string message, ConsoleTheme? theme = null) => w.WriteLine(message, (theme ?? DefaultTheme).Hint);

        private static readonly ConsoleTheme DefaultTheme = new();
    }
}
