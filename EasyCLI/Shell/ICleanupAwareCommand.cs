using EasyCLI.Shell.SignalHandling;

namespace EasyCLI.Shell
{
    /// <summary>
    /// Extended interface for CLI commands that support cleanup hooks and graceful shutdown.
    /// This is an optional interface that commands can implement to register cleanup actions
    /// that will be executed when the application receives shutdown signals.
    /// </summary>
    public interface ICleanupAwareCommand : ICliCommand
    {
        /// <summary>
        /// Called when the command is about to execute, allowing it to register cleanup actions.
        /// This method is called before ExecuteAsync and provides access to the cleanup manager.
        /// </summary>
        /// <param name="cleanupManager">The cleanup manager for registering cleanup actions.</param>
        /// <param name="context">The shell execution context.</param>
        void RegisterCleanupActions(ICleanupManager cleanupManager, ShellExecutionContext context);
    }

    /// <summary>
    /// Extension methods for working with cleanup-aware commands.
    /// </summary>
    public static class CleanupAwareCommandExtensions
    {
        /// <summary>
        /// Determines if a command supports cleanup hooks.
        /// </summary>
        /// <param name="command">The command to check.</param>
        /// <returns>True if the command implements ICleanupAwareCommand, false otherwise.</returns>
        public static bool SupportsCleanup(this ICliCommand command)
        {
            return command is ICleanupAwareCommand;
        }

        /// <summary>
        /// Registers cleanup actions if the command supports them.
        /// </summary>
        /// <param name="command">The command to register cleanup for.</param>
        /// <param name="cleanupManager">The cleanup manager.</param>
        /// <param name="context">The shell execution context.</param>
        /// <returns>True if cleanup actions were registered, false if the command doesn't support cleanup.</returns>
        public static bool TryRegisterCleanup(this ICliCommand command, ICleanupManager cleanupManager, ShellExecutionContext context)
        {
            if (command is ICleanupAwareCommand cleanupAware)
            {
                cleanupAware.RegisterCleanupActions(cleanupManager, context);
                return true;
            }
            return false;
        }
    }
}
