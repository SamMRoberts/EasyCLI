using System;

namespace EasyCLI.Prompts
{
    /// <summary>
    /// Base class implementing shared prompt loop & validation.
    /// </summary>
    /// <typeparam name="T">The type of value returned by the prompt.</typeparam>
    public abstract class BasePrompt<T>(string prompt, IConsoleWriter writer, IConsoleReader reader, PromptOptions? options = null, T? @default = default, IPromptValidator<T>? validator = null) : IUserPrompt<T>
    {
        protected readonly IConsoleWriter _writer = writer;
        protected readonly IConsoleReader _reader = reader;
        protected readonly PromptOptions _options = options ?? new PromptOptions();
        protected readonly IPromptValidator<T>? _validator = validator;

        public string Prompt { get; } = prompt;
        public T? Default { get; } = @default;

        public T GetValue()
        {
            while (true)
            {
                RenderPrompt();
                string? raw = _reader.ReadLine();

                // Treat explicit ESC entry as cancel if enabled. Our simple reader cannot intercept a single ESC key
                // so we adopt the convention that a user typing the literal sequence "<esc>" or just an empty string when no default
                // while EnableEscapeCancel is true and the raw input equals "\u001b" (if surfaced) triggers cancel.
                if (_options.EnableEscapeCancel && raw == "\u001b")
                {
                    return HandleCancel();
                }

                if (string.IsNullOrEmpty(raw))
                {
                    if (Default is not null)
                    {
                        return Default!;
                    }
                }

                if (_validator != null)
                {
                    PromptValidationResult result = _validator.Validate(raw, out T value);
                    if (result.IsValid)
                    {
                        return value;
                    }
                    if (!result.IsValid)
                    {
                        if (!string.IsNullOrEmpty(result.Error))
                        {
                            WriteError(result.Error);
                        }
                        continue; // ask again
                    }
                }
                else
                {
                    // Try simple parse
                    if (TryConvert(raw, out T converted))
                    {
                        return converted;
                    }
                    WriteError($"Invalid value: '{raw}'");
                }
            }
        }

        protected T HandleCancel()
        {
            if (_options.CancelBehavior == PromptCancelBehavior.ReturnDefault)
            {
                if (Default is not null)
                {
                    return Default!;
                }

                // No default -> return default(T)
                return default!;
            }

            // Throw behavior
            throw new PromptCanceledException(Prompt);
        }

        protected virtual void RenderPrompt()
        {
            if (_options.LabelStyle != null)
            {
                _writer.Write(Prompt, _options.LabelStyle.Value);
            }
            else
            {
                _writer.Write(Prompt);
            }

            if (Default is not null)
            {
                string defText = $"[{Default}]";
                if (_options.DefaultStyle != null)
                {
                    _writer.Write(" " + defText, _options.DefaultStyle.Value);
                }
                else
                {
                    _writer.Write(" " + defText);
                }
            }

            _writer.Write(_options.Suffix ?? ": ");
        }

        protected virtual void WriteError(string message)
        {
            if (_options.ErrorStyle != null)
            {
                _writer.WriteErrorLine(message);
            }
            else
            {
                _writer.WriteLine(message);
            }
        }

        protected abstract bool TryConvert(string raw, out T value);
    }
}
