namespace EasyCLI.Shell.SignalHandling
{
    /// <summary>
    /// Manages terminal state and provides restoration capabilities during shutdown.
    /// Handles cursor visibility, terminal modes, and other console state that may
    /// need to be restored when a CLI application is interrupted.
    /// </summary>
    public interface ITerminalStateManager : IDisposable
    {
        /// <summary>
        /// Captures the current terminal state for later restoration.
        /// </summary>
        void CaptureState();

        /// <summary>
        /// Restores the terminal to its previously captured state.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for the restore operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RestoreStateAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a value indicating whether terminal state has been captured.
        /// </summary>
        bool HasCapturedState { get; }

        /// <summary>
        /// Temporarily modifies terminal state (e.g., hide cursor) with automatic restoration.
        /// </summary>
        /// <param name="modification">The modification to apply.</param>
        /// <returns>A disposable that will restore the state when disposed.</returns>
        IDisposable TemporaryModification(TerminalModification modification);
    }

    /// <summary>
    /// Represents types of terminal modifications that can be applied.
    /// </summary>
    public enum TerminalModification
    {
        /// <summary>
        /// Hide the cursor.
        /// </summary>
        HideCursor,

        /// <summary>
        /// Show the cursor.
        /// </summary>
        ShowCursor,

        /// <summary>
        /// Enter raw mode (disable line buffering, echo, etc.).
        /// </summary>
        RawMode,

        /// <summary>
        /// Return to normal mode (restore line buffering, echo, etc.).
        /// </summary>
        NormalMode,
    }
}
