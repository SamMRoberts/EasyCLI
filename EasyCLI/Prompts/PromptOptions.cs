using System;
using System.Collections.Generic;

namespace EasyCLI.Prompts
{
    /// <summary>
    /// Common options controlling prompt rendering & behavior.
    /// </summary>
    public sealed class PromptOptions
    {
        public string? Suffix { get; set; } = ": ";
        public ConsoleStyle? LabelStyle { get; set; } = ConsoleStyles.Heading;
        public ConsoleStyle? DefaultStyle { get; set; } = ConsoleStyles.Hint;
        public ConsoleStyle? ErrorStyle { get; set; } = ConsoleStyles.Error;
        public bool EchoInput { get; set; } = true; // future secure input scenario could set false
    public bool EnableEscapeCancel { get; set; } = true;
    public PromptCancelBehavior CancelBehavior { get; set; } = PromptCancelBehavior.ReturnDefault;
    }
}
