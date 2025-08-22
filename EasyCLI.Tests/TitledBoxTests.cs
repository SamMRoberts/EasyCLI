using System.Linq;
using Xunit;

namespace EasyCLI.Tests
{
    public class TitledBoxTests
    {
        [Fact]
        public void TitledBox_HasTitleInTopBorder()
        {
            var lines = EasyCLI.Formatting.ConsoleFormatting.BuildTitledBox(new[] { "a", "b" }, "Title").ToList();
            Assert.Contains(" Title ", lines[0]);
            Assert.StartsWith("┌", lines[0]);
            Assert.EndsWith("┐", lines[0]);
        }
    }
}
