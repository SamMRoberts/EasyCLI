using System;
using System.IO;

namespace EasyCLI.Tests
{
    internal sealed class ConsoleCapture : IDisposable
    {
        private readonly TextWriter _originalOut;
        private readonly StringWriter _stringWriter = new();

        public ConsoleCapture()
        {
            _originalOut = Console.Out;
            Console.SetOut(_stringWriter);
        }

        public string GetOutput() => _stringWriter.ToString();

        public void Dispose()
        {
            Console.SetOut(_originalOut);
            _stringWriter.Dispose();
        }
    }
}
