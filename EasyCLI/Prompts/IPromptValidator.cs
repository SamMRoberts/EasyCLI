using System;

namespace EasyCLI.Prompts
{
    public interface IPromptValidator<T>
    {
        PromptValidationResult Validate(string raw, out T? value);
    }
}
