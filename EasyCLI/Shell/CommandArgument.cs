using EasyCLI.Deprecation;

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
    /// <param name="deprecationInfo">Optional deprecation information for this argument.</param>
    public class CommandArgument(string name, string description, bool isRequired = true, DeprecationInfo? deprecationInfo = null)
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

        /// <summary>
        /// Gets the deprecation information for this argument, if any.
        /// </summary>
        public DeprecationInfo? DeprecationInfo { get; } = deprecationInfo;

        /// <summary>
        /// Gets a value indicating whether this argument is deprecated.
        /// </summary>
        public bool IsDeprecated => DeprecationInfo != null;

        /// <summary>
        /// Creates a deprecated command argument.
        /// </summary>
        /// <param name="name">The argument name.</param>
        /// <param name="description">The argument description.</param>
        /// <param name="deprecationInfo">The deprecation information.</param>
        /// <param name="isRequired">Whether the argument is required.</param>
        /// <returns>A deprecated CommandArgument.</returns>
        public static CommandArgument Deprecated(
            string name,
            string description,
            DeprecationInfo deprecationInfo,
            bool isRequired = true)
        {
            ArgumentNullException.ThrowIfNull(deprecationInfo);
            return new CommandArgument(name, description, isRequired, deprecationInfo);
        }
    }
}
