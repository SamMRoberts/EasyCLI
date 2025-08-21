namespace EasyCLI.Prompts
{
    public readonly struct PromptValidationResult
    {
        public PromptValidationResult(bool isValid, string? error = null)
        {
            IsValid = isValid;
            Error = error;
        }
        public bool IsValid { get; }
        public string? Error { get; }
        public static PromptValidationResult Success() => new(true);
        public static PromptValidationResult Fail(string message) => new(false, message);
    }
}
