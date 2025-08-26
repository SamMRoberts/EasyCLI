namespace EasyCLI.Shell
{
    /// <summary>
    /// Represents help information for a CLI command.
    /// </summary>
    public class CommandHelp
    {
        /// <summary>
        /// Gets or sets the command usage pattern.
        /// </summary>
        public string Usage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the detailed description of the command.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets the collection of command arguments.
        /// </summary>
        public List<CommandArgument> Arguments { get; } = [];

        /// <summary>
        /// Gets the collection of command options/flags.
        /// </summary>
        public List<CommandOption> Options { get; } = [];

        /// <summary>
        /// Gets the collection of usage examples.
        /// </summary>
        public List<CommandExample> Examples { get; } = [];
    }
}
