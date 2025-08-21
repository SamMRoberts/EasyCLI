using System;

namespace EasyCLI.Prompts
{
    /// <summary>
    /// Secure/hidden input prompt (e.g., passwords). Falls back to plain prompt if hidden source not available.
    /// </summary>
    public sealed class HiddenInputPrompt : BasePrompt<string>
    {
        private readonly IHiddenInputSource? _hiddenSource;
        private readonly char? _mask;
        public HiddenInputPrompt(string prompt, IConsoleWriter writer, IConsoleReader reader, IHiddenInputSource? hiddenSource = null, PromptOptions? options = null, string? @default = null, char? mask = '*')
            : base(prompt, writer, reader, options, @default)
        {
            _hiddenSource = hiddenSource;
            _mask = mask;
        }

        protected override bool TryConvert(string raw, out string value)
        {
            value = raw;
            return true;
        }

        public new string Get()
        {
            // If we have a hidden source, override the base loop with single capture + optional default handling & no validation
            if (_hiddenSource != null)
            {
                RenderPrompt();
                var captured = _hiddenSource.ReadHidden(_mask);
                if (_options.EnableEscapeCancel && captured == "\u001b")
                {
                    return HandleCancel();
                }
                if (string.IsNullOrEmpty(captured) && Default is not null)
                    return Default;
                return captured;
            }
            return base.Get();
        }
    }
}
