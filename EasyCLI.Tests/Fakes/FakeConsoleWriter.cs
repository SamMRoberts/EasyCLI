using System.Text;

namespace EasyCLI.Tests.Fakes
{
    internal sealed class FakeConsoleWriter : IConsoleWriter
    {
        private readonly StringBuilder _sb = new();
        public bool EnableStyles { get; set; } = true;
        public string Output => _sb.ToString();

        public void Write(string message) => _sb.Append(message);
        public void WriteLine(string message) => _sb.AppendLine(message);
        public void Write(string message, ConsoleStyle style)
        {
            if (EnableStyles)
                _sb.Append(style.Apply(message));
            else
                _sb.Append(message);
        }
        public void WriteLine(string message, ConsoleStyle style)
        {
            if (EnableStyles)
                _sb.AppendLine(style.Apply(message));
            else
                _sb.AppendLine(message);
        }
    }
}
