namespace EasyCLI.Prompts
{
    public readonly struct PromptValidationResult(bool isValid, string? error = null)
    {
        public bool IsValid { get; } = isValid;
        public string? Error { get; } = error;
        public static PromptValidationResult Success()
        {
            return new(true);
        }

        public static PromptValidationResult Fail(string message)
        {
            return new(false, message);
        }
    }
}
