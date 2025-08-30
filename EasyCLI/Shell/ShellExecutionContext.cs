using EasyCLI.Console;
using EasyCLI.Shell.SignalHandling;

namespace EasyCLI.Shell
{
    /// <summary>
    /// Provides contextual services for a running command.
    /// </summary>
    public class ShellExecutionContext
    {
        internal ShellExecutionContext(CliShell shell, IConsoleWriter writer, IConsoleReader reader, ICleanupManager? cleanupManager = null)
        {
            Shell = shell;
            Writer = writer;
            Reader = reader;
            CleanupManager = cleanupManager;
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
        /// Gets the console reader.
        /// </summary>
        public IConsoleReader Reader { get; }

        /// <summary>
        /// Gets the cleanup manager for registering cleanup actions.
        /// May be null if signal handling is not enabled.
        /// </summary>
        public ICleanupManager? CleanupManager { get; }

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
