using System;

namespace EasyCLI.Prompts
{
    /// <summary>
    /// Base class implementing shared prompt loop & validation.
    /// </summary>
    public abstract class BasePrompt<T> : IUserPrompt<T>
    {
        protected readonly IConsoleWriter _writer;
        protected readonly IConsoleReader _reader;
        protected readonly PromptOptions _options;
        protected readonly IPromptValidator<T>? _validator;

    protected BasePrompt(string prompt, IConsoleWriter writer, IConsoleReader reader, PromptOptions? options = null, T? @default = default, IPromptValidator<T>? validator = null)
        {
            Prompt = prompt;
            _writer = writer;
            _reader = reader;
            _options = options ?? new PromptOptions();
            Default = @default;
            _validator = validator;
        }

        public string Prompt { get; }
    public T? Default { get; }

        public T Get()
        {
            while (true)
            {
                RenderPrompt();
                var raw = _reader.ReadLine();
                if (string.IsNullOrEmpty(raw))
                {
                    if (Default is not null)
                        return Default!;
                }

                if (_validator != null)
                {
                    var result = _validator.Validate(raw, out var value);
                    if (result.IsValid && value is not null)
                        return value!;
                    if (!result.IsValid)
                    {
                        if (!string.IsNullOrEmpty(result.Error))
                            WriteError(result.Error);
                        continue; // ask again
                    }
                }
                else
                {
                    // Try simple parse
                    if (TryConvert(raw, out var converted))
                        return converted;
                    WriteError($"Invalid value: '{raw}'");
                }
            }
        }

        protected virtual void RenderPrompt()
        {
            if (_options.LabelStyle != null)
                _writer.Write(Prompt, _options.LabelStyle.Value);
            else
                _writer.Write(Prompt);

            if (Default is not null)
            {
                var defText = $"[{Default}]";
                if (_options.DefaultStyle != null)
                    _writer.Write(" " + defText, _options.DefaultStyle.Value);
                else
                    _writer.Write(" " + defText);
            }

            _writer.Write(_options.Suffix ?? ": ");
        }

        protected virtual void WriteError(string message)
        {
            if (_options.ErrorStyle != null)
                _writer.WriteErrorLine(message);
            else
                _writer.WriteLine(message);
        }

        protected abstract bool TryConvert(string raw, out T value);
    }
}
