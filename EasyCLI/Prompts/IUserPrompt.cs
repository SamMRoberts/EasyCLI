using System;
using System.Collections.Generic;

namespace EasyCLI.Prompts
{
    /// <summary>
    /// Basic contract for prompting a user and retrieving a value of type T.
    /// </summary>
    public interface IUserPrompt<T>
    {
        /// <summary>Prompt text shown to the user (without trailing colon / adornments).</summary>
        string Prompt { get; }
        /// <summary>Optional default value rendered when the user presses enter with no input.</summary>
        T? Default { get; }
        /// <summary>Execute the prompt and return a value.</summary>
        T Get();
    }
}
