namespace EasyCLI.Prompts
{
    /// <summary>
    /// Represents the result of a prompt validation operation.
    /// </summary>
    public readonly struct PromptValidationResult(bool isValid, string? error = null)
    {
        /// <summary>
        /// Gets a value indicating whether the validation was successful.
        /// </summary>
        public bool IsValid { get; } = isValid;

        /// <summary>
        /// Gets the error message if validation failed, or null if validation succeeded.
        /// </summary>
        public string? Error { get; } = error;

        /// <summary>
        /// Creates a successful validation result.
        /// </summary>
        /// <returns>A validation result indicating success.</returns>
        public static PromptValidationResult Success()
        {
            return new(true);
        }

        /// <summary>
        /// Creates a failed validation result with an error message.
        /// </summary>
        /// <param name="message">The error message describing why validation failed.</param>
        /// <returns>A validation result indicating failure with the specified message.</returns>
        public static PromptValidationResult Fail(string message)
        {
            return new(false, message);
        }
    }
}
