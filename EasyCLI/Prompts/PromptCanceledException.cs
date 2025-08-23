namespace EasyCLI.Prompts
{
    /// <summary>
    /// Exception thrown when a user cancels a prompt operation.
    /// </summary>
    public sealed class PromptCanceledException(string prompt) : OperationCanceledException($"Prompt '{prompt}' canceled by user")
    {
    }
}
