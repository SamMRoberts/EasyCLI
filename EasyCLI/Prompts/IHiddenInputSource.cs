namespace EasyCLI.Prompts
{
    /// <summary>
    /// Abstraction for collecting hidden (non-echoed or masked) input.
    /// </summary>
    public interface IHiddenInputSource
    {
        /// <summary>
        /// Reads hidden input from the user (password-style input).
        /// </summary>
        /// <param name="mask">Optional character to display instead of the typed characters. If null, nothing is displayed.</param>
        /// <returns>The input string provided by the user, or a sentinel value for cancellation.</returns>
        string ReadHidden(char? mask = null);
    }

    /// <summary>
    /// Default implementation using Console.ReadKey for interactive terminals.
    /// </summary>
    public sealed class ConsoleHiddenInputSource : IHiddenInputSource
    {
        /// <summary>
        /// Reads hidden input from the console using individual key presses.
        /// </summary>
        /// <param name="mask">Optional character to display instead of the typed characters. If null, nothing is displayed.</param>
        /// <returns>The input string provided by the user, or a sentinel value for cancellation.</returns>
        public string ReadHidden(char? mask = null)
        {
            StringBuilder sb = new();
            while (true)
            {
                ConsoleKeyInfo key = System.Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Escape)
                {
                    return "\u001b"; // sentinel for cancel
                }
                if (key.Key == ConsoleKey.Enter)
                {
                    System.Console.WriteLine();
                    break;
                }
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        sb.Length--;
                        if (mask.HasValue)
                        {
                            System.Console.Write('\b');
                            System.Console.Write(' ');
                            System.Console.Write('\b');
                        }
                    }
                    continue;
                }
                _ = sb.Append(key.KeyChar);
                if (mask.HasValue)
                {
                    System.Console.Write(mask.Value);
                }
            }
            return sb.ToString();
        }
    }
}
