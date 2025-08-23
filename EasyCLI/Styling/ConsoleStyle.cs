namespace EasyCLI.Styling
{
    /// <summary>
    /// Represents an ANSI SGR style (e.g., bold, colors). Immutable.
    /// </summary>
    public readonly struct ConsoleStyle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleStyle"/> struct with the specified ANSI SGR codes.
        /// </summary>
        /// <param name="codes">The ANSI SGR codes that define this style.</param>
        public ConsoleStyle(params int[] codes)
        {
            Codes = codes ?? Array.Empty<int>();
        }

        /// <summary>
        /// Gets the ANSI SGR codes that define this style.
        /// </summary>
        public int[] Codes { get; }

        /// <summary>
        /// Applies this style to the specified message by wrapping it with ANSI escape sequences.
        /// </summary>
        /// <param name="message">The message to apply styling to.</param>
        /// <returns>The message wrapped with ANSI escape sequences for this style.</returns>
        public string Apply(string message)
        {
            if (string.IsNullOrEmpty(message) || Codes.Length == 0)
            {
                return message ?? string.Empty;
            }
            return $"\u001b[{string.Join(';', Codes)}m{message}\u001b[0m";
        }
    }
}
