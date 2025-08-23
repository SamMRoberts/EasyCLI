using EasyCLI.Console;

namespace EasyCLI.Extensions;

/// <summary>
/// Semantic convenience extension methods for styled output.
/// </summary>
public static class ConsoleWriterExtensions
{
    private static readonly ConsoleTheme DefaultTheme = new();

    /// <summary>
    /// Writes a success message to the console using the success style from the specified theme.
    /// </summary>
    /// <param name="w">The console writer to write to.</param>
    /// <param name="message">The message to write.</param>
    /// <param name="theme">The theme to use for styling. If null, uses the default theme.</param>
    public static void WriteSuccess(this IConsoleWriter w, string message, ConsoleTheme? theme = null)
    {
        ArgumentNullException.ThrowIfNull(w);
        w.Write(message, (theme ?? DefaultTheme).Success);
    }

    /// <summary>
    /// Writes a warning message to the console using the warning style from the specified theme.
    /// </summary>
    /// <param name="w">The console writer to write to.</param>
    /// <param name="message">The message to write.</param>
    /// <param name="theme">The theme to use for styling. If null, uses the default theme.</param>
    public static void WriteWarning(this IConsoleWriter w, string message, ConsoleTheme? theme = null)
    {
        ArgumentNullException.ThrowIfNull(w);
        w.Write(message, (theme ?? DefaultTheme).Warning);
    }

    /// <summary>
    /// Writes an error message to the console using the error style from the specified theme.
    /// </summary>
    /// <param name="w">The console writer to write to.</param>
    /// <param name="message">The message to write.</param>
    /// <param name="theme">The theme to use for styling. If null, uses the default theme.</param>
    public static void WriteError(this IConsoleWriter w, string message, ConsoleTheme? theme = null)
    {
        ArgumentNullException.ThrowIfNull(w);
        w.Write(message, (theme ?? DefaultTheme).Error);
    }

    /// <summary>
    /// Writes a heading message to the console using the heading style from the specified theme.
    /// </summary>
    /// <param name="w">The console writer to write to.</param>
    /// <param name="message">The message to write.</param>
    /// <param name="theme">The theme to use for styling. If null, uses the default theme.</param>
    public static void WriteHeading(this IConsoleWriter w, string message, ConsoleTheme? theme = null)
    {
        ArgumentNullException.ThrowIfNull(w);
        w.Write(message, (theme ?? DefaultTheme).Heading);
    }

    /// <summary>
    /// Writes an informational message to the console using the info style from the specified theme.
    /// </summary>
    /// <param name="w">The console writer to write to.</param>
    /// <param name="message">The message to write.</param>
    /// <param name="theme">The theme to use for styling. If null, uses the default theme.</param>
    public static void WriteInfo(this IConsoleWriter w, string message, ConsoleTheme? theme = null)
    {
        ArgumentNullException.ThrowIfNull(w);
        w.Write(message, (theme ?? DefaultTheme).Info);
    }

    /// <summary>
    /// Writes a hint message to the console using the hint style from the specified theme.
    /// </summary>
    /// <param name="w">The console writer to write to.</param>
    /// <param name="message">The message to write.</param>
    /// <param name="theme">The theme to use for styling. If null, uses the default theme.</param>
    public static void WriteHint(this IConsoleWriter w, string message, ConsoleTheme? theme = null)
    {
        ArgumentNullException.ThrowIfNull(w);
        w.Write(message, (theme ?? DefaultTheme).Hint);
    }

    /// <summary>
    /// Writes a success message to the console followed by a newline, using the success style from the specified theme.
    /// </summary>
    /// <param name="w">The console writer to write to.</param>
    /// <param name="message">The message to write.</param>
    /// <param name="theme">The theme to use for styling. If null, uses the default theme.</param>
    public static void WriteSuccessLine(this IConsoleWriter w, string message, ConsoleTheme? theme = null)
    {
        ArgumentNullException.ThrowIfNull(w);
        w.WriteLine(message, (theme ?? DefaultTheme).Success);
    }

    /// <summary>
    /// Writes a warning message to the console followed by a newline, using the warning style from the specified theme.
    /// </summary>
    /// <param name="w">The console writer to write to.</param>
    /// <param name="message">The message to write.</param>
    /// <param name="theme">The theme to use for styling. If null, uses the default theme.</param>
    public static void WriteWarningLine(this IConsoleWriter w, string message, ConsoleTheme? theme = null)
    {
        ArgumentNullException.ThrowIfNull(w);
        w.WriteLine(message, (theme ?? DefaultTheme).Warning);
    }

    /// <summary>
    /// Writes an error message to the console followed by a newline, using the error style from the specified theme.
    /// </summary>
    /// <param name="w">The console writer to write to.</param>
    /// <param name="message">The message to write.</param>
    /// <param name="theme">The theme to use for styling. If null, uses the default theme.</param>
    public static void WriteErrorLine(this IConsoleWriter w, string message, ConsoleTheme? theme = null)
    {
        ArgumentNullException.ThrowIfNull(w);
        w.WriteLine(message, (theme ?? DefaultTheme).Error);
    }

    /// <summary>
    /// Writes a heading message to the console followed by a newline, using the heading style from the specified theme.
    /// </summary>
    /// <param name="w">The console writer to write to.</param>
    /// <param name="message">The message to write.</param>
    /// <param name="theme">The theme to use for styling. If null, uses the default theme.</param>
    public static void WriteHeadingLine(this IConsoleWriter w, string message, ConsoleTheme? theme = null)
    {
        ArgumentNullException.ThrowIfNull(w);
        w.WriteLine(message, (theme ?? DefaultTheme).Heading);
    }

    /// <summary>
    /// Writes an informational message to the console followed by a newline, using the info style from the specified theme.
    /// </summary>
    /// <param name="w">The console writer to write to.</param>
    /// <param name="message">The message to write.</param>
    /// <param name="theme">The theme to use for styling. If null, uses the default theme.</param>
    public static void WriteInfoLine(this IConsoleWriter w, string message, ConsoleTheme? theme = null)
    {
        ArgumentNullException.ThrowIfNull(w);
        w.WriteLine(message, (theme ?? DefaultTheme).Info);
    }

    /// <summary>
    /// Writes a hint message to the console followed by a newline, using the hint style from the specified theme.
    /// </summary>
    /// <param name="w">The console writer to write to.</param>
    /// <param name="message">The message to write.</param>
    /// <param name="theme">The theme to use for styling. If null, uses the default theme.</param>
    public static void WriteHintLine(this IConsoleWriter w, string message, ConsoleTheme? theme = null)
    {
        ArgumentNullException.ThrowIfNull(w);
        w.WriteLine(message, (theme ?? DefaultTheme).Hint);
    }
}
