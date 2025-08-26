namespace EasyCLI.Shell
{
    /// <summary>
    /// Represents a command usage example.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="CommandExample"/> class.
    /// </remarks>
    /// <param name="command">The example command.</param>
    /// <param name="description">The example description.</param>
    public class CommandExample(string command, string description)
    {

        /// <summary>
        /// Gets the example command.
        /// </summary>
        public string Command { get; } = command;

        /// <summary>
        /// Gets the example description.
        /// </summary>
        public string Description { get; } = description;
    }
}
