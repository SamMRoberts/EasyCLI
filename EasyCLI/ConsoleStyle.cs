using System;

namespace EasyCLI
{
    /// <summary>
    /// Represents an ANSI SGR style (e.g., bold, colors). Immutable.
    /// </summary>
    /// Represents an ANSI SGR style (e.g., bold, colors). Immutable.
    /// </summary>
    /// Represents an ANSI SGR style (e.g., bold, colors). Immutable.
    /// </summary>
    /// Represents an ANSI SGR style (e.g., bold, colors). Immutable.
    /// </summary>
    public readonly struct ConsoleStyle
    {
        public ConsoleStyle(params int[] codes)
        {
            Codes = codes ?? Array.Empty<int>();
        }

        public int[] Codes { get; }

        public string Apply(string message)
        {
            if (string.IsNullOrEmpty(message) || Codes.Length == 0)
                return message ?? string.Empty;
            return $"\u001b[{string.Join(';', Codes)}m{message}\u001b[0m";
        }
    }
}
