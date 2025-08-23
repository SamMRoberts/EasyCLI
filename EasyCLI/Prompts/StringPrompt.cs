using EasyCLI.Console;

namespace EasyCLI.Prompts
{
    public sealed class StringPrompt(string prompt, IConsoleWriter writer, IConsoleReader reader, PromptOptions? options = null, string? @default = null, IPromptValidator<string>? validator = null) : BasePrompt<string>(prompt, writer, reader, options, @default, validator)
    {
        protected override bool TryConvert(string raw, out string value)
        {
            value = raw;
            return true;
        }
    }
}
