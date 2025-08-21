using System.Collections.Generic;

namespace EasyCLI.Tests.Fakes
{
    internal sealed class FakeConsoleReader : IConsoleReader
    {
        private readonly Queue<string> _lines;
        public FakeConsoleReader(IEnumerable<string> lines) => _lines = new Queue<string>(lines);
        public string ReadLine() => _lines.Count == 0 ? string.Empty : _lines.Dequeue();
    }
}
