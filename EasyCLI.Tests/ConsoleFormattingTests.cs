using System.Linq;
using Xunit;

namespace EasyCLI.Tests
{
    public class ConsoleFormattingTests
    {
        [Fact]
        public void Rule_DefaultsTo80Chars()
        {
            var line = EasyCLI.ConsoleFormatting.Rule();
            Assert.Equal(80, line.Length);
        }

        [Fact]
        public void HeadingUnderline_MatchesTextLength()
        {
            var u = EasyCLI.ConsoleFormatting.HeadingUnderline("Hello");
            Assert.Equal(5, u.Length);
        }

        [Fact]
        public void Wrap_SplitsLongText()
        {
            var text = string.Join(" ", Enumerable.Range(0, 20).Select(i => "word" + i));
            var lines = EasyCLI.ConsoleFormatting.Wrap(text, 20).ToList();
            Assert.True(lines.Count > 1);
        }

        [Fact]
        public void Wrap_UsesConsoleWidth_WhenZero()
        {
            var lines = EasyCLI.ConsoleFormatting.Wrap("hello world", 0).ToList();
            Assert.True(lines.Count >= 1);
        }
    }
}
