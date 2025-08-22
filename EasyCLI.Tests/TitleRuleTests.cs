using Xunit;

namespace EasyCLI.Tests
{
    public class TitleRuleTests
    {
        [Fact]
        public void TitleRule_FillsToWidth()
        {
            var s = EasyCLI.Formatting.ConsoleFormatting.TitleRule("Title", width: 40);
            Assert.Equal(40, s.Length);
            Assert.StartsWith("Title ", s); // includes a gap
        }
    }
}
