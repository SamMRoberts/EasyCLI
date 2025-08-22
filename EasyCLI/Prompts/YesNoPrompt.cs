namespace EasyCLI.Prompts
{
    public sealed class YesNoPrompt : BasePrompt<bool>
    {
        public YesNoPrompt(string prompt, IConsoleWriter writer, IConsoleReader reader, PromptOptions? options = null, bool? @default = null)
            : base(prompt + " (y/n)", writer, reader, options, @default ?? default)
        {
        }

        protected override bool TryConvert(string raw, out bool value)
        {
            value = false;
            if (string.IsNullOrEmpty(raw))
            {
                return false;
            }

            char c = char.ToLowerInvariant(raw.Trim()[0]);
            if (c == 'y')
            {
                value = true;
                return true;
            }

            if (c == 'n')
            {
                value = false;
                return true;
            }

            return false;
        }
    }
}
