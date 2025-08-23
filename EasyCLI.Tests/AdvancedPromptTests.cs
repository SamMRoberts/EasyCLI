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
            private readonly string _value;
            public FakeHidden(string value) { _value = value; }
            public string ReadHidden(char? mask = null) => _value; // mask ignored for test
        }

        [Fact]
        public void HiddenInputPrompt_ReturnsValue()
        {
            var writer = new FakeConsoleWriter();
            var reader = new FakeConsoleReader(new string[0]);
            var hidden = new FakeHidden("secret");
            var p = new HiddenInputPrompt("Password", writer, reader, hiddenSource: hidden, @default: null);
            var v = p.GetValue();
            Assert.Equal("secret", v);
        }

        [Fact]
        public void ChoicePrompt_SelectByIndex()
        {
            var writer = new FakeConsoleWriter();
            var reader = new FakeConsoleReader(new[] { "2" });
            var choices = new[] { new Choice<string>("Apple", "A"), new Choice<string>("Banana", "B") };
            var p = new ChoicePrompt<string>("Pick fruit", choices, writer, reader);
            var v = p.GetValue();
            Assert.Equal("B", v);
        }

        [Fact]
        public void ChoicePrompt_SelectByLabelPrefix()
        {
            var writer = new FakeConsoleWriter();
            var reader = new FakeConsoleReader(new[] { "Ban" });
            var choices = new[] { new Choice<string>("Apple", "A"), new Choice<string>("Banana", "B") };
            var p = new ChoicePrompt<string>("Pick fruit", choices, writer, reader);
            var v = p.GetValue();
            Assert.Equal("B", v);
        }

        [Fact]
        public void MultiSelectPrompt_CommaAndRange()
        {
            var writer = new FakeConsoleWriter();
            var reader = new FakeConsoleReader(new[] { "1,3-4" });
            var choices = new[] { new Choice<int>("One", 1), new Choice<int>("Two", 2), new Choice<int>("Three", 3), new Choice<int>("Four", 4) };
            var p = new MultiSelectPrompt<int>("Pick numbers", choices, writer, reader);
            var v = p.GetValue();
            Assert.Equal(new List<int> { 1, 3, 4 }, v);
        }

        [Fact]
        public void ChoicePrompt_Paging_Navigation()
        {
            var writer = new FakeConsoleWriter();
            var choices = new List<Choice<int>>();
            for (int i = 1; i <= 15; i++) choices.Add(new Choice<int>("Item" + i, i));
            // Navigate next page then pick an item on second page
            var reader = new FakeConsoleReader(new[] { "n", "12" });
            var opts = new PromptOptions { PageSize = 10, EnablePaging = true };
            var p = new ChoicePrompt<int>("Pick", choices, writer, reader, options: opts);
            var v = p.GetValue();
            Assert.Equal(12, v);
            Assert.Contains("Page 2/2", writer.Output);
        }

        [Fact]
        public void MultiSelectPrompt_Paging_Navigation()
        {
            var writer = new FakeConsoleWriter();
            var choices = new List<Choice<int>>();
            for (int i = 1; i <= 25; i++) choices.Add(new Choice<int>("Item" + i, i));
            var reader = new FakeConsoleReader(new[] { "n", "1,12-13" }); // first page navigation, then select numbers including ones across pages allowed by indexes
            var opts = new PromptOptions { PageSize = 10, EnablePaging = true };
            var p = new MultiSelectPrompt<int>("Pick many", choices, writer, reader, options: opts);
            var v = p.GetValue();
            Assert.Equal(new List<int> { 1, 12, 13 }, v);
            Assert.Contains("Page 2/3", writer.Output);
        }
    }
}
