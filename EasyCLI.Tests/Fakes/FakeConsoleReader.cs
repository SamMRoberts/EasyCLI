using EasyCLI.Console;

namespace EasyCLI.Tests.Fakes
{
    internal sealed class FakeConsoleReader(IEnumerable<string> lines) : IConsoleReader
    {
        private readonly Queue<string> _lines = new Queue<string>(lines);

        public string ReadLine() => _lines.Count == 0 ? string.Empty : _lines.Dequeue();
    }
}
