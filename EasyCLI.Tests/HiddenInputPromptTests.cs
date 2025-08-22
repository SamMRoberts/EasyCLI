using EasyCLI.Prompts;
using EasyCLI.Styling;

namespace EasyCLI.Tests
{
    public class HiddenInputPromptTests
    {
        private sealed class CapturingHidden : IHiddenInputSource
        {
            private readonly string _value;
            public char? LastMask { get; private set; }
            public CapturingHidden(string value) => _value = value;
            public string ReadHidden(char? mask = null)
            {
                LastMask = mask;
                return _value;
            }
        }

        private sealed class DummyWriter : IConsoleWriter
        {
            public void Write(string message) { }
            public void WriteLine(string message) { }
            public void Write(string message, ConsoleStyle style) { }
            public void WriteLine(string message, ConsoleStyle style) { }
        }

        private sealed class DummyReader : IConsoleReader
        {
            public string ReadLine() => string.Empty; // not used for hidden input path
        }

        [Fact]
        public void HiddenInputPrompt_Passes_Default_Mask()
        {
            var hidden = new CapturingHidden("secret");
            var prompt = new HiddenInputPrompt("Password", new DummyWriter(), new DummyReader(), hiddenSource: hidden, mask: '*');
            var v = prompt.Get();
            Assert.Equal("secret", v);
            Assert.Equal('*', hidden.LastMask);
        }

        [Fact]
        public void HiddenInputPrompt_Passes_Custom_Mask()
        {
            var hidden = new CapturingHidden("token");
            var prompt = new HiddenInputPrompt("Token", new DummyWriter(), new DummyReader(), hiddenSource: hidden, mask: '#');
            var v = prompt.Get();
            Assert.Equal("token", v);
            Assert.Equal('#', hidden.LastMask);
        }

        [Fact]
        public void HiddenInputPrompt_Uses_Default_Value_When_Empty()
        {
            var hidden = new CapturingHidden("");
            var prompt = new HiddenInputPrompt("Password", new DummyWriter(), new DummyReader(), hiddenSource: hidden, @default: "fallback", mask: '*');
            var v = prompt.Get();
            Assert.Equal("fallback", v);
            Assert.Equal('*', hidden.LastMask);
        }

        private sealed class EscHidden : IHiddenInputSource
        {
            public string ReadHidden(char? mask = null) => "\u001b"; // simulate ESC
        }

        [Fact]
        public void HiddenInputPrompt_Cancel_ReturnsDefault()
        {
            var prompt = new HiddenInputPrompt("Password", new DummyWriter(), new DummyReader(), hiddenSource: new EscHidden(), @default: "fallback", mask: '*');
            var v = prompt.Get();
            Assert.Equal("fallback", v);
        }
    }
}
