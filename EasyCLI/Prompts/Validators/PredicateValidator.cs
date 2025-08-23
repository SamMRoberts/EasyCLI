using System;

namespace EasyCLI.Prompts.Validators
{
    /// <summary>
    /// Wraps a predicate for simple custom validation logic.
    /// </summary>
    /// <typeparam name="T">The converted value type.</typeparam>
    public sealed class PredicateValidator<T> : IPromptValidator<T>
    {
        private readonly Func<string, (bool Ok, T Value, string? Error)> func;

        /// <summary>
        /// Initializes a new instance of the <see cref="PredicateValidator{T}"/> class.
        /// </summary>
        /// <param name="func">The validation function that takes a string and returns a validation result tuple.</param>
        public PredicateValidator(Func<string, (bool Ok, T Value, string? Error)> func)
        {
            this.func = func;
        }

        /// <summary>
        /// Validates the input using the configured predicate function.
        /// </summary>
        /// <param name="raw">The raw input string from the user.</param>
        /// <param name="value">When successful, contains the converted value.</param>
        /// <returns>A validation result indicating success or failure.</returns>
        public PromptValidationResult Validate(string raw, out T value)
        {
            (bool Ok, T Value, string? Error) result = this.func(raw);
            value = result.Value;

            if (!result.Ok)
            {
                return PromptValidationResult.Fail(result.Error ?? "Invalid value");
            }

            return PromptValidationResult.Success();
        }
    }
}
