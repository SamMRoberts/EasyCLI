using System;

namespace EasyCLI.Prompts
{
    public sealed class PromptCanceledException : OperationCanceledException
    {
        public PromptCanceledException(string prompt)
            : base($"Prompt '{prompt}' canceled by user")
        {
        }
    }
}
