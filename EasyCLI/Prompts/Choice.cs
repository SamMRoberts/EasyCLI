namespace EasyCLI.Prompts
{
    /// <summary>
    /// Represents a choice option with a display label and associated value.
    /// </summary>
    /// <typeparam name="T">The type of the value associated with this choice.</typeparam>
    /// <remarks>
    /// Initializes a new instance of the <see cref="Choice{T}"/> class.
    /// </remarks>
    /// <param name="label">The display label for this choice.</param>
    /// <param name="value">The value associated with this choice.</param>
    public sealed class Choice<T>(string label, T value)
    {

        /// <summary>
        /// Gets the display label for this choice.
        /// </summary>
        public string Label { get; } = label;

        /// <summary>
        /// Gets the value associated with this choice.
        /// </summary>
        public T Value { get; } = value;

        /// <summary>
        /// Returns the display label for this choice.
        /// </summary>
        /// <returns>The display label.</returns>
        public override string ToString()
        {
            return Label;
        }
    }
}
