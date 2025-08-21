using System.Linq;
using Xunit;

namespace EasyCLI.Tests
{
    public class TitledBoxTests
    {
        [Fact]
        public void TitledBox_HasTitleInTopBorder()
        {
            var lines = EasyCLI.ConsoleFormatting.BuildTitledBox(new[] { "a", "b" }, "Title").ToList();
            Assert.True(lines[0].Contains(" Title "));
            Assert.StartsWith("┌", lines[0]);
            Assert.EndsWith("┐", lines[0]);
        }
    }
}
