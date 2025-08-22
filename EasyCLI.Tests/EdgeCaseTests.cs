using Xunit;

namespace EasyCLI.Tests
{
    public class EdgeCaseTests
    {
        [Fact]
        public void CenterTitleRule_Truncates_LongTitle()
        {
            var title = new string('X', 200);
            var s = EasyCLI.Formatting.ConsoleFormatting.CenterTitleRule(title, width: 20);
            Assert.Equal(20, s.Length);
        }

        [Fact]
        public void TitledBox_Handles_EmptyTitle_And_EmptyLines()
        {
            var lines = EasyCLI.Formatting.ConsoleFormatting.BuildTitledBox(System.Array.Empty<string>(), "");
            Assert.NotEmpty(lines);
        }
    }
}
