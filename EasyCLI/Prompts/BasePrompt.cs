using EasyCLI.Console;

namespace EasyCLI.Prompts
{
    /// <summary>
    /// Base class implementing shared prompt loop &amp; validation.
    /// </summary>
    /// <typeparam name="T">The type of value returned by the prompt.</typeparam>
    public abstract class BasePrompt<T> : IUserPrompt<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasePrompt{T}"/> class.
        /// </summary>
        /// <param name="prompt">The prompt text to display to the user.</param>
        /// <param name="writer">The console writer to use for output.</param>
        /// <param name="reader">The console reader to use for input.</param>
        /// <param name="options">Options controlling prompt behavior. If null, default options are used.</param>
        /// <param name="default">The default value to use if the user provides no input.</param>
        /// <param name="validator">The validator to use for validating user input.</param>
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
        /// <summary>
        /// Gets the console writer used for output.
        /// </summary>
        protected IConsoleWriter Writer { get; }
        
        /// <summary>
        /// Gets the console reader used for input.
        /// </summary>
        protected IConsoleReader Reader { get; }
        
        /// <summary>
        /// Gets the options controlling prompt behavior.
        /// </summary>
        protected PromptOptions Options { get; }
        
        /// <summary>
        /// Gets the validator used for validating user input.
        /// </summary>
        protected IPromptValidator<T>? Validator { get; }

        /// <summary>
        /// Gets the prompt text displayed to the user.
        /// </summary>
        public string Prompt { get; }
        
        /// <summary>
        /// Gets the default value used when the user provides no input.
        /// </summary>
        public T? Default { get; }

        /// <summary>
        /// Prompts the user for input and returns the validated value.
        /// </summary>
        /// <returns>The validated value provided by the user.</returns>
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

        /// <summary>
        /// Handles user cancellation according to the configured cancel behavior.
        /// </summary>
        /// <returns>The default value or throws an exception based on the cancel behavior.</returns>
        /// <exception cref="PromptCanceledException">Thrown when cancel behavior is set to throw.</exception>
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

        /// <summary>
        /// Renders the prompt text and default value hint to the console.
        /// </summary>
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

        /// <summary>
        /// Writes an error message to the console using the configured error style.
        /// </summary>
        /// <param name="message">The error message to write.</param>
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

        /// <summary>
        /// Attempts to convert the raw user input string to the target type.
        /// </summary>
        /// <param name="raw">The raw input string from the user.</param>
        /// <param name="value">When successful, contains the converted value.</param>
        /// <returns><c>true</c> if the conversion was successful; otherwise, <c>false</c>.</returns>
        protected abstract bool TryConvert(string raw, out T value);
    }
}
