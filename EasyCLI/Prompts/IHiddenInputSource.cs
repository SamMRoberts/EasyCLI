using System.Text;
using System;

namespace EasyCLI.Prompts
{
    /// <summary>
    /// Abstraction for collecting hidden (non-echoed or masked) input.
    /// </summary>
    public interface IHiddenInputSource
    {
        string ReadHidden(char? mask = null);
    }

    /// <summary>
    /// Default implementation using Console.ReadKey for interactive terminals.
    /// </summary>
    public sealed class ConsoleHiddenInputSource : IHiddenInputSource
    {
        public string ReadHidden(char? mask = null)
        {
            var sb = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        sb.Length--;
                        if (mask.HasValue)
                        {
                            Console.Write('\b');
                            Console.Write(' ');
                            Console.Write('\b');
                        }
                    }
                    continue;
                }
                sb.Append(key.KeyChar);
                if (mask.HasValue)
                    Console.Write(mask.Value);
            }
            return sb.ToString();
        }
    }
}
