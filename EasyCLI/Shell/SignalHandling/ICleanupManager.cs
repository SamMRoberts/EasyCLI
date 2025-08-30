namespace EasyCLI.Shell.SignalHandling
{
    /// <summary>
    /// Manages cleanup actions that should be executed during graceful shutdown.
    /// Provides a mechanism for commands and services to register cleanup hooks.
    /// </summary>
    public interface ICleanupManager : IDisposable
    {
        /// <summary>
        /// Registers a cleanup action to be executed during shutdown.
        /// </summary>
        /// <param name="cleanup">The cleanup action to register. Should be safe to call multiple times.</param>
        /// <param name="name">Optional name for the cleanup action (for debugging/logging).</param>
        /// <returns>A handle that can be used to unregister the cleanup action.</returns>
        ICleanupHandle RegisterCleanup(Func<CancellationToken, Task> cleanup, string? name = null);

        /// <summary>
        /// Registers a synchronous cleanup action to be executed during shutdown.
        /// </summary>
        /// <param name="cleanup">The cleanup action to register. Should be safe to call multiple times.</param>
        /// <param name="name">Optional name for the cleanup action (for debugging/logging).</param>
        /// <returns>A handle that can be used to unregister the cleanup action.</returns>
        ICleanupHandle RegisterCleanup(Action cleanup, string? name = null);

        /// <summary>
        /// Executes all registered cleanup actions in reverse order of registration.
        /// This method is typically called during graceful shutdown.
        /// </summary>
        /// <param name="timeout">Maximum time to wait for all cleanup actions to complete.</param>
        /// <param name="cancellationToken">Cancellation token for the cleanup operation.</param>
        /// <returns>True if all cleanup actions completed successfully, false if some failed or timed out.</returns>
        Task<bool> ExecuteCleanupAsync(TimeSpan timeout, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the number of currently registered cleanup actions.
        /// </summary>
        int RegisteredCleanupCount { get; }
    }

    /// <summary>
    /// Handle for a registered cleanup action that allows unregistration.
    /// </summary>
    public interface ICleanupHandle : IDisposable
    {
        /// <summary>
        /// Gets the name of the cleanup action (if provided during registration).
        /// </summary>
        string? Name { get; }

        /// <summary>
        /// Gets a value indicating whether gets whether this cleanup action is still registered.
        /// </summary>
        bool IsRegistered { get; }

        /// <summary>
        /// Unregisters the cleanup action. After calling this, the cleanup will not be executed.
        /// </summary>
        void Unregister();
    }
}
