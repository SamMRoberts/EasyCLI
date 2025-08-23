namespace EasyCLI.Prompts
{
    /// <summary>
    /// Basic contract for prompting a user and retrieving a value of type T.
    /// </summary>
    /// <typeparam name="T">The type of value returned by the prompt.</typeparam>
    public interface IUserPrompt<T>
    {
        /// <summary>Gets prompt text shown to the user (without trailing colon / adornments).</summary>
        string Prompt { get; }

        /// <summary>Gets optional default value rendered when the user presses enter with no input.</summary>
        T? Default { get; }

        /// <summary>Execute the prompt and return a value.</summary>
        /// <returns>The value entered or selected by the user.</returns>
        T GetValue();
    }
}
