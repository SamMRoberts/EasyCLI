using System.IO;

namespace EasyCLI
{
    /// <summary>
    /// Compatibility wrapper for ConsoleWriter to maintain backward compatibility.
    /// </summary>
    /// <remarks>
    /// This class provides backward compatibility for code that references EasyCLI.ConsoleWriter.
    /// For new code, consider using EasyCLI.Console.ConsoleWriter directly.
    /// </remarks>
    public class ConsoleWriter : Console.ConsoleWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleWriter"/> class.
        /// </summary>
        /// <param name="enableColors">Explicit override for color support. If null, auto-detects based on environment.</param>
        /// <param name="output">The text writer to use for output. If null, uses <see cref="System.Console.Out"/>.</param>
        public ConsoleWriter(bool? enableColors = null, TextWriter? output = null)
            : base(enableColors, output)
        {
        }
    }
}
