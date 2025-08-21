using System.Globalization;

namespace EasyCLI.Prompts
{
    public sealed class IntPrompt : BasePrompt<int>
    {
        private static int DefaultOr(int? val) => val ?? 0;
        public IntPrompt(string prompt, IConsoleWriter writer, IConsoleReader reader, PromptOptions? options = null, int? @default = null, IPromptValidator<int>? validator = null)
            : base(prompt, writer, reader, options, @default ?? default, validator) { }

        protected override bool TryConvert(string raw, out int value)
            => int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
    }
}
