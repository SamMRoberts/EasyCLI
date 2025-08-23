namespace EasyCLI.Prompts
{
    /// <summary>
    /// Base class implementing shared prompt loop & validation.
    /// </summary>
    /// <typeparam name="T">The type of value returned by the prompt.</typeparam>
    public abstract class BasePrompt<T> : IUserPrompt<T>
    {
        protected BasePrompt(string prompt, IConsoleWriter writer, IConsoleReader reader, PromptOptions? options = null, T? @default = default, IPromptValidator<T>? validator = null)
        {
            Prompt = prompt;
            Writer = writer;
            Reader = reader;
            Options = options ?? new PromptOptions();
            Default = @default;
            Validator = validator;
        }

        // Exposed to derived prompts as protected read-only properties (preferred over protected fields per analyzers CA1051/SA1401)
        protected IConsoleWriter Writer { get; }
        protected IConsoleReader Reader { get; }
        protected PromptOptions Options { get; }
        protected IPromptValidator<T>? Validator { get; }

        public string Prompt { get; }
        public T? Default { get; }

        public T GetValue()
        {
            while (true)
            {
                RenderPrompt();
                string? raw = Reader.ReadLine();

                // Treat explicit ESC entry as cancel if enabled. Our simple reader cannot intercept a single ESC key
                // so we adopt the convention that a user typing the literal sequence "<esc>" or just an empty string when no default
                // while EnableEscapeCancel is true and the raw input equals "\u001b" (if surfaced) triggers cancel.
                if (Options.EnableEscapeCancel && raw == "\u001b")
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

                if (Validator != null)
                {
                    PromptValidationResult result = Validator.Validate(raw, out T value);
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
            if (Options.CancelBehavior == PromptCancelBehavior.ReturnDefault)
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
            if (Options.LabelStyle != null)
            {
                Writer.Write(Prompt, Options.LabelStyle.Value);
            }
            else
            {
                Writer.Write(Prompt);
            }

            if (Default is not null)
            {
                string defText = $"[{Default}]";
                if (Options.DefaultStyle != null)
                {
                    Writer.Write(" " + defText, Options.DefaultStyle.Value);
                }
                else
                {
                    Writer.Write(" " + defText);
                }
            }

            Writer.Write(Options.Suffix ?? ": ");
        }

    protected virtual void WriteError(string message)
        {
            if (Options.ErrorStyle != null)
            {
                Writer.WriteErrorLine(message);
            }
            else
            {
                Writer.WriteLine(message);
            }
        }

    protected abstract bool TryConvert(string raw, out T value);
    }
}
