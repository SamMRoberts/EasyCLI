namespace EasyCLI.Prompts
{
    public sealed class PromptCanceledException(string prompt) : OperationCanceledException($"Prompt '{prompt}' canceled by user")
    {
    }
}
