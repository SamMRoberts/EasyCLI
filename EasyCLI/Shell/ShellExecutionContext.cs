using EasyCLI.Console;

namespace EasyCLI.Shell
{
    /// <summary>
    /// Provides contextual services for a running command.
    /// </summary>
    public class ShellExecutionContext
    {
        internal ShellExecutionContext(CliShell shell, IConsoleWriter writer)
        {
            Shell = shell;
            Writer = writer;
        }

        /// <summary>
        /// Gets the shell instance.
        /// </summary>
        public CliShell Shell { get; }

        /// <summary>
        /// Gets the console writer.
        /// </summary>
        public IConsoleWriter Writer { get; }

        /// <summary>
        /// Writes an informational message.
        /// </summary>
        /// <param name="message">Message to write.</param>
        public void Info(string message)
        {
            Writer.WriteLine(message);
        }

        /// <summary>
        /// Gets or sets the current working directory for the shell. Changing this affects subsequent process execution.
        /// </summary>
        public string CurrentDirectory
        {
            get => Shell.CurrentDirectory;
            set => Shell.CurrentDirectory = value;
        }
    }
}
