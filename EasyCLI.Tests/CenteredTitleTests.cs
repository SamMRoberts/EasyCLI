using Xunit;

namespace EasyCLI.Tests
{
    public class CenteredTitleTests
    {
        [Fact]
        public void CenterTitleRule_ProducesCenteredTitle()
        {
            var s = EasyCLI.Formatting.ConsoleFormatting.CenterTitleRule("Center", width: 40);
            Assert.Equal(40, s.Length);
            Assert.Contains(" Center ", s);
        }
    }
}
