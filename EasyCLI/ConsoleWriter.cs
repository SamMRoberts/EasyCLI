namespace EasyCLI
{
    /// <summary>
    /// Compatibility wrapper for ConsoleWriter to maintain backward compatibility.
    /// </summary>
    /// <remarks>
    /// This class provides backward compatibility for code that references EasyCLI.ConsoleWriter.
    /// For new code, consider using EasyCLI.Console.ConsoleWriter directly.
    /// </remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ConsoleWriter"/> class.
    /// </remarks>
    /// <param name="enableColors">Explicit override for color support. If null, auto-detects based on environment.</param>
    /// <param name="output">The text writer to use for output. If null, uses <see cref="System.Console.Out"/>.</param>
    public class ConsoleWriter(bool? enableColors = null, TextWriter? output = null) : Console.ConsoleWriter(enableColors, output)
    {
    }
}
