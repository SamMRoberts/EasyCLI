namespace EasyCLI.Prompts
{
    /// <summary>
    /// Specifies the behavior when a prompt is cancelled by the user.
    /// </summary>
    public enum PromptCancelBehavior
    {
        /// <summary>
        /// Return the default value for the prompt type when cancelled.
        /// </summary>
        ReturnDefault,

        /// <summary>
        /// Throw an exception when the prompt is cancelled.
        /// </summary>
        Throw,
    }
}
