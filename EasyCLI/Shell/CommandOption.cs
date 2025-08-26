namespace EasyCLI.Shell
{
    /// <summary>
    /// Represents a command option or flag.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="CommandOption"/> class.
    /// </remarks>
    /// <param name="longName">The long option name (e.g., "help").</param>
    /// <param name="shortName">The short option name (e.g., "h").</param>
    /// <param name="description">The option description.</param>
    /// <param name="hasValue">Whether the option takes a value.</param>
    /// <param name="defaultValue">The default value if any.</param>
    public class CommandOption(string longName, string? shortName, string description, bool hasValue = false, string? defaultValue = null)
    {

        /// <summary>
        /// Gets the long option name.
        /// </summary>
        public string LongName { get; } = longName;

        /// <summary>
        /// Gets the short option name.
        /// </summary>
        public string? ShortName { get; } = shortName;

        /// <summary>
        /// Gets the option description.
        /// </summary>
        public string Description { get; } = description;

        /// <summary>
        /// Gets a value indicating whether the option takes a value.
        /// </summary>
        public bool HasValue { get; } = hasValue;

        /// <summary>
        /// Gets the default value for the option.
        /// </summary>
        public string? DefaultValue { get; } = defaultValue;
    }
}
