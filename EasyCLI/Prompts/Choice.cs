namespace EasyCLI.Prompts
{
    /// <summary>
    /// Represents a choice option with a display label and associated value.
    /// </summary>
    /// <typeparam name="T">The type of the value associated with this choice.</typeparam>
    public sealed class Choice<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Choice{T}"/> class.
        /// </summary>
        /// <param name="label">The display label for this choice.</param>
        /// <param name="value">The value associated with this choice.</param>
        public Choice(string label, T value)
        {
            Label = label;
            Value = value;
        }

        /// <summary>
        /// Gets the display label for this choice.
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// Gets the value associated with this choice.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Returns the display label for this choice.
        /// </summary>
        /// <returns>The display label.</returns>
        public override string ToString() => Label;
    }
}
