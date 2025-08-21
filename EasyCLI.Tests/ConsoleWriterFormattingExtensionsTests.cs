using System.Collections.Generic;
using Xunit;

namespace EasyCLI.Tests
{
    public class ConsoleWriterFormattingExtensionsTests
    {
        [Fact]
        public void WriteRule_WritesLine()
        {
            var sw = new System.IO.StringWriter();
            var w = new EasyCLI.ConsoleWriter(enableColors: false, output: sw);
            w.WriteRule(20);
            var s = sw.ToString().Replace("\r\n", "\n");
            Assert.Contains("\n", s);
            Assert.Contains("──────────", s); // subset of rule
        }

        [Fact]
        public void WriteHeadingBlock_PrintsTitleAndUnderline()
        {
            var sw = new System.IO.StringWriter();
            var w = new EasyCLI.ConsoleWriter(enableColors: false, output: sw);
            w.WriteHeadingBlock("Title");
            var s = sw.ToString().Replace("\r\n", "\n");
            Assert.Contains("Title\n", s);
            Assert.Contains("─────\n", s);
        }

        [Fact]
        public void WriteBullets_PrintsItems()
        {
            var sw = new System.IO.StringWriter();
            var w = new EasyCLI.ConsoleWriter(enableColors: false, output: sw);
            w.WriteBullets(new[] { "one", "two" }, indent: 2);
            var s = sw.ToString();
            Assert.Contains("  • one", s);
            Assert.Contains("  • two", s);
        }

        [Fact]
        public void WriteWrapped_WrapsLongText()
        {
            var sw = new System.IO.StringWriter();
            var w = new EasyCLI.ConsoleWriter(enableColors: false, output: sw);
            w.WriteWrapped("word word word word word word", width: 12);
            var s = sw.ToString().Replace("\r\n", "\n");
            Assert.Contains("\n", s);
        }

        [Fact]
        public void WriteTableSimple_PrintsBorders()
        {
            var sw = new System.IO.StringWriter();
            var w = new EasyCLI.ConsoleWriter(enableColors: false, output: sw);
            w.WriteTableSimple(new[] { "A", "B" }, new List<IReadOnlyList<string>> { new[] { "1", "2" } });
            var s = sw.ToString();
            Assert.Contains("+", s);
            Assert.Contains("|", s);
        }

        [Fact]
        public void WriteBox_PrintsBox()
        {
            var sw = new System.IO.StringWriter();
            var w = new EasyCLI.ConsoleWriter(enableColors: false, output: sw);
            w.WriteBox(new[] { "hello", "world" });
            var s = sw.ToString();
            Assert.Contains("┌", s);
            Assert.Contains("└", s);
            Assert.Contains("│ hello ", s);
            Assert.Contains("│ world ", s);
        }
    }
}
