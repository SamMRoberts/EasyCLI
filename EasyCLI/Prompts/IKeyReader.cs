using System;

namespace EasyCLI.Prompts
{
    public interface IKeyReader
    {
        ConsoleKeyInfo ReadKey(bool intercept = true);
    }

    public sealed class ConsoleKeyReader : IKeyReader
    {
        public ConsoleKeyInfo ReadKey(bool intercept = true) => Console.ReadKey(intercept);
    }
}
