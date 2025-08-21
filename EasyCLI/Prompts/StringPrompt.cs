namespace EasyCLI.Prompts
{
    public sealed class StringPrompt : BasePrompt<string>
    {
        public StringPrompt(string prompt, IConsoleWriter writer, IConsoleReader reader, PromptOptions? options = null, string? @default = null, IPromptValidator<string>? validator = null)
            : base(prompt, writer, reader, options, @default, validator) { }

        protected override bool TryConvert(string raw, out string value)
        {
            value = raw;
            return true;
        }
    }
}
