namespace EasyCLI.Prompts
{
    using System.Globalization;
    using EasyCLI.Console;

    public sealed class IntPrompt : BasePrompt<int>
    {
        public IntPrompt(string prompt, IConsoleWriter writer, IConsoleReader reader, PromptOptions? options = null, int? @default = null, IPromptValidator<int>? validator = null)
            : base(prompt, writer, reader, options, @default ?? default, validator)
        {
        }

        protected override bool TryConvert(string raw, out int value)
        {
            return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }
    }
}
