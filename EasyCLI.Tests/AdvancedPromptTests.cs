using System.Collections.Generic;
using EasyCLI.Prompts;
using EasyCLI.Tests.Fakes;
using Xunit;

namespace EasyCLI.Tests
{
    public class AdvancedPromptTests
    {
        private sealed class FakeHidden : IHiddenInputSource
        {
            private readonly string _value; private readonly char? _mask; public string CapturedMaskOutput = string.Empty;
            public FakeHidden(string value){ _value = value; }
            public string ReadHidden(char? mask = null) => _value; // mask ignored for test
        }

        [Fact]
        public void HiddenInputPrompt_ReturnsValue()
        {
            var writer = new FakeConsoleWriter();
            var reader = new FakeConsoleReader(new string[0]);
            var hidden = new FakeHidden("secret");
            var p = new HiddenInputPrompt("Password", writer, reader, hiddenSource: hidden, @default: null);
            var v = p.Get();
            Assert.Equal("secret", v);
        }

        [Fact]
        public void ChoicePrompt_SelectByIndex()
        {
            var writer = new FakeConsoleWriter();
            var reader = new FakeConsoleReader(new[] { "2" });
            var choices = new[] { new Choice<string>("Apple","A"), new Choice<string>("Banana","B") };
            var p = new ChoicePrompt<string>("Pick fruit", choices, writer, reader);
            var v = p.Get();
            Assert.Equal("B", v);
        }

        [Fact]
        public void ChoicePrompt_SelectByLabelPrefix()
        {
            var writer = new FakeConsoleWriter();
            var reader = new FakeConsoleReader(new[] { "Ban" });
            var choices = new[] { new Choice<string>("Apple","A"), new Choice<string>("Banana","B") };
            var p = new ChoicePrompt<string>("Pick fruit", choices, writer, reader);
            var v = p.Get();
            Assert.Equal("B", v);
        }

        [Fact]
        public void MultiSelectPrompt_CommaAndRange()
        {
            var writer = new FakeConsoleWriter();
            var reader = new FakeConsoleReader(new[] { "1,3-4" });
            var choices = new[] { new Choice<int>("One",1), new Choice<int>("Two",2), new Choice<int>("Three",3), new Choice<int>("Four",4) };
            var p = new MultiSelectPrompt<int>("Pick numbers", choices, writer, reader);
            var v = p.Get();
            Assert.Equal(new List<int>{1,3,4}, v);
        }
    }
}
