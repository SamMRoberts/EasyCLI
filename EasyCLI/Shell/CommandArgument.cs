namespace EasyCLI.Shell
{
    /// <summary>
    /// Represents a command argument.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="CommandArgument"/> class.
    /// </remarks>
    /// <param name="name">The argument name.</param>
    /// <param name="description">The argument description.</param>
    /// <param name="isRequired">Whether the argument is required.</param>
    public class CommandArgument(string name, string description, bool isRequired = true)
    {

        /// <summary>
        /// Gets the argument name.
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Gets the argument description.
        /// </summary>
        public string Description { get; } = description;

        /// <summary>
        /// Gets a value indicating whether the argument is required.
        /// </summary>
        public bool IsRequired { get; } = isRequired;
    }
}
