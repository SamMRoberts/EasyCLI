using EasyCLI.Console;

namespace EasyCLI.Prompts
{
    /// <summary>
    /// Base class implementing shared prompt loop &amp; validation.
    /// </summary>
    /// <typeparam name="T">The type of value returned by the prompt.</typeparam>
    /// <remarks>
    /// Initializes a new instance of the <see cref="BasePrompt{T}"/> class.
    /// </remarks>
    /// <param name="prompt">The prompt text to display to the user.</param>
    /// <param name="writer">The console writer to use for output.</param>
    /// <param name="reader">The console reader to use for input.</param>
    /// <param name="options">Options controlling prompt behavior. If null, default options are used.</param>
    /// <param name="default">The default value to use if the user provides no input.</param>
    /// <param name="hasDefault">Whether a default value was explicitly provided.</param>
    /// <param name="validator">The validator to use for validating user input.</param>
    public abstract class BasePrompt<T>(string prompt, IConsoleWriter writer, IConsoleReader reader, PromptOptions? options = null, T? @default = default, bool hasDefault = false, IPromptValidator<T>? validator = null) : IUserPrompt<T>
    {

        // Exposed to derived prompts as protected read-only properties (preferred over protected fields per analyzers CA1051/SA1401)

        /// <summary>
        /// Gets the console writer used for output.
        /// </summary>
        protected IConsoleWriter Writer { get; } = writer;

        /// <summary>
        /// Gets the console reader used for input.
        /// </summary>
        protected IConsoleReader Reader { get; } = reader;

        /// <summary>
        /// Gets the options controlling prompt behavior.
        /// </summary>
        protected PromptOptions Options { get; } = options ?? new PromptOptions();

        /// <summary>
        /// Gets the validator used for validating user input.
        /// </summary>
        protected IPromptValidator<T>? Validator { get; } = validator;

        /// <summary>
        /// Gets the prompt text displayed to the user.
        /// </summary>
        public string Prompt { get; } = prompt;

        /// <summary>
        /// Gets the default value used when the user provides no input.
        /// </summary>
        public T? Default { get; } = @default;

        /// <summary>
        /// Gets a value indicating whether a default value was explicitly provided.
        /// </summary>
        public bool HasDefault { get; } = hasDefault;

        /// <summary>
        /// Prompts the user for input and returns the validated value.
        /// </summary>
        /// <returns>The validated value provided by the user.</returns>
        /// <exception cref="InvalidOperationException">Thrown when in non-interactive mode and no default value is available.</exception>
        public T GetValue()
        {
            // Handle non-interactive mode
            if (Options.NonInteractive)
            {
                if (HasDefault)
                {
                    return Default!;
                }

                // No default available in non-interactive mode - fail with clear error
                throw new InvalidOperationException(
                    $"Cannot prompt for '{Prompt}' in non-interactive mode. " +
                    "Use --no-input only when all prompts have default values, or provide values via command-line arguments.");
            }

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
                    if (HasDefault)
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
                if (HasDefault)
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

            if (HasDefault)
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
