namespace EasyCLI.Styling
{
    /// <summary>
    /// Represents an ANSI SGR style (e.g., bold, colors). Immutable.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ConsoleStyle"/> struct with the specified ANSI SGR codes.
    /// </remarks>
    /// <param name="codes">The ANSI SGR codes that define this style.</param>
    public readonly struct ConsoleStyle(params int[] codes)
    {

        /// <summary>
        /// Gets the ANSI SGR codes that define this style.
        /// </summary>
        public int[] Codes { get; } = codes ?? [];

        /// <summary>
        /// Applies this style to the specified message by wrapping it with ANSI escape sequences.
        /// </summary>
        /// <param name="message">The message to apply styling to.</param>
        /// <returns>The message wrapped with ANSI escape sequences for this style.</returns>
        public string Apply(string message)
        {
            return string.IsNullOrEmpty(message) || Codes.Length == 0
                ? message ?? string.Empty
                : $"\u001b[{string.Join(';', Codes)}m{message}\u001b[0m";
        }
    }
}
