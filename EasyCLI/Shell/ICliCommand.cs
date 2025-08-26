namespace EasyCLI.Shell
{
    /// <summary>
    /// Contract for a shell command.
    /// </summary>
    public interface ICliCommand
    {
        /// <summary>
        /// Gets the primary name of the command.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a short description of the command.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Executes the command with the specified context and arguments.
        /// </summary>
        /// <param name="context">The shell execution context.</param>
        /// <param name="args">Arguments (excluding command name).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns>Exit code (0 for success).</returns>
        Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken cancellationToken);

        /// <summary>
        /// Produces completion suggestions for a given prefix.
        /// </summary>
        /// <param name="context">Execution context.</param>
        /// <param name="prefix">The already typed text (may be empty).</param>
        /// <returns>Possible completions; empty array if none.</returns>
        /// <summary>
        /// Gets completion suggestions for a partially typed token.
        /// </summary>
        /// <returns>Matching completion candidates.</returns>
        string[] GetCompletions(ShellExecutionContext context, string prefix)
        {
            return []; // override as needed
        }
    }
}
