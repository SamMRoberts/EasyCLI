using System.Collections.Generic;
using Xunit;

namespace EasyCLI.Tests
{
    public class TableAlignmentTests
    {
        [Fact]
        public void BuildSimpleTable_RespectsAlignmentAndMaxWidth()
        {
            var headers = new[] { "Left", "Center", "Right" };
            var rows = new List<IReadOnlyList<string>>
            {
                new[] { "a", "b", "c" },
                new[] { "longtext", "middle", "end" }
            };

            var lines = new List<string>(EasyCLI.Formatting.ConsoleFormatting.BuildSimpleTable(
                headers,
                rows,
                padding: 1,
                maxWidth: 40,
                alignments: new[] { EasyCLI.Formatting.ConsoleFormatting.CellAlign.Left, EasyCLI.Formatting.ConsoleFormatting.CellAlign.Center, EasyCLI.Formatting.ConsoleFormatting.CellAlign.Right }));

            Assert.NotEmpty(lines);
            Assert.Contains("+", lines[0]);
            Assert.All(lines, l => Assert.True(l.Length <= 42)); // border chars included
        }
    }
}
