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

        public PredicateValidator(Func<string, (bool Ok, T Value, string? Error)> func)
        {
            this.func = func;
        }

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
