namespace EasyCLI.Shell
{
    /// <summary>
    /// Options controlling the behavior of the interactive CLI shell.
    /// </summary>
    public class ShellOptions
    {
        /// <summary>
        /// Gets or sets the prompt text displayed for each input line. Defaults to "easy>".
        /// </summary>
        public string Prompt { get; set; } = "easy>";

        /// <summary>
        /// Gets or sets an optional style to apply to the prompt text.
        /// </summary>
        public ConsoleStyle? PromptStyle { get; set; }
            = ConsoleStyles.FgGreen;

        /// <summary>
        /// Gets or sets a value indicating whether to echo executed commands into history.
        /// </summary>
        public bool EnableHistory { get; set; } = true;

        /// <summary>
        /// Gets or sets the maximum number of history entries to retain. 0 = unlimited.
        /// </summary>
        public int HistoryLimit { get; set; } = 500;

        /// <summary>
        /// Gets or sets a value indicating whether to delegate commands with shell operators to the native shell.
        /// When enabled, commands containing pipes, redirections, and other shell syntax are executed through
        /// the native shell (bash, cmd, etc.) to preserve full shell functionality.
        /// </summary>
        public bool EnableNativeShellDelegation { get; set; } = true;
    }
}
