namespace EasyCLI.Prompts
{
    /// <summary>
    /// Defines methods for validating user input for prompts.
    /// </summary>
    /// <typeparam name="T">The type of value to validate.</typeparam>
    public interface IPromptValidator<T>
    {
        /// <summary>
        /// Validates the raw user input and attempts to convert it to the target type.
        /// </summary>
        /// <param name="raw">The raw input string from the user.</param>
        /// <param name="value">When successful, contains the validated and converted value.</param>
        /// <returns>A validation result indicating success or failure with optional error message.</returns>
        PromptValidationResult Validate(string raw, out T value);
    }
}
