using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Xunit;
using System.Linq;

namespace EasyCLI.Tests
{
    public class NewCmdletTests
    {
        private PowerShell CreatePowerShell()
        {
            var iss = InitialSessionState.CreateDefault();
            iss.Commands.Add(new SessionStateCmdletEntry(
                "Write-Rule",
                typeof(EasyCLI.Cmdlets.WriteRuleCommand),
                null));
            iss.Commands.Add(new SessionStateCmdletEntry(
                "Write-TitledBox",
                typeof(EasyCLI.Cmdlets.WriteTitledBoxCommand),
                null));
            var rs = RunspaceFactory.CreateRunspace(iss);
            rs.Open();
            return PowerShell.Create(rs);
        }

        [Fact]
        public void WriteEasyRule_Basic()
        {
            using var capture = new ConsoleCapture();
            using var ps = CreatePowerShell();
            ps.AddCommand("Write-Rule");
            var results = ps.Invoke();
            var text = capture.GetOutput();
            Assert.Contains("─", text);
            Assert.Single(results);
            var lineObj = results[0].BaseObject?.ToString();
            Assert.False(string.IsNullOrEmpty(lineObj));
            Assert.Contains("─", lineObj!);
        }

                [Fact]
                public void WriteEasyRule_PassThruObject()
                {
                        using var ps = CreatePowerShell();
                        ps.AddCommand("Write-Rule")
                            .AddParameter("Title", "MyTitle")
                            .AddParameter("Center")
                            .AddParameter("PassThruObject");
                        var results = ps.Invoke();
                        Assert.Single(results);
                        var obj = Assert.IsType<EasyCLI.Cmdlets.RuleInfo>(results[0].BaseObject);
                        Assert.Equal("MyTitle", obj.Title);
                        Assert.True(obj.Center);
                        Assert.Contains("MyTitle", obj.Line);
                }

        [Fact]
        public void WriteEasyRule_Title()
        {
            using var capture = new ConsoleCapture();
            using var ps = CreatePowerShell();
            ps.AddCommand("Write-Rule").AddParameter("Title", "TITLE");
            ps.Invoke();
            var text = capture.GetOutput();
            Assert.Contains("TITLE", text);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(1)]
        public void WriteEasyRule_WidthEdgeCases(int width)
        {
            using var ps = CreatePowerShell();
            ps.AddCommand("Write-Rule").AddParameter("Width", width);
            var results = ps.Invoke();
            Assert.Single(results);
            var line = results[0].BaseObject?.ToString();
            Assert.False(string.IsNullOrEmpty(line));
            Assert.Equal(width <= 0 ? line!.Length : width, line!.Length); // if width>0 should match; width never 0 here
        }

        [Fact]
        public void WriteEasyTitledBox_LongLines_WrapNotAppliedButWidthAdapts()
        {
            using var capture = new ConsoleCapture();
            using var ps = CreatePowerShell();
            var longLine = new string('X', 120);
            ps.AddScript($"'{longLine}' | Write-TitledBox -Title Wide");
            ps.Invoke();
            var outText = capture.GetOutput();
            // Ensure the long content appears fully (no truncation since box auto-expands)
            Assert.Contains(longLine, outText);
        }

        [Fact]
        public void WriteEasyTitledBox_CollectsPipeline()
        {
            using var capture = new ConsoleCapture();
            using var ps = CreatePowerShell();
            ps.AddScript("'a','b' | Write-TitledBox -Title MyBox");
            var results = ps.Invoke();
            // Box returns multiple lines (top,border lines,content,bottom)
            Assert.True(results.Count >= 4);
            var outText = capture.GetOutput();
            Assert.Contains("MyBox", outText);
            Assert.Contains("a", outText);
            Assert.Contains("b", outText);
        }

        [Fact]
        public void WriteEasyTitledBox_PassThruObject()
        {
            using var ps = CreatePowerShell();
            ps.AddScript("'x','y' | Write-TitledBox -Title Demo -PassThruObject");
            var results = ps.Invoke();
            Assert.Single(results);
            var obj = Assert.IsType<EasyCLI.Cmdlets.TitledBoxInfo>(results[0].BaseObject);
            Assert.Equal("Demo", obj.Title);
            Assert.Equal(new[] { "x", "y" }, obj.Lines);
            Assert.True(obj.BoxLines.Count >= 4);
        }

        [Fact]
        public void WriteEasyRule_NoColor_HasNoAnsi()
        {
            using var capture = new ConsoleCapture();
            using var ps = CreatePowerShell();
            ps.AddCommand("Write-Rule").AddParameter("NoColor").AddParameter("Title", "NC");
            var results = ps.Invoke();
            Assert.Single(results);
            var output = capture.GetOutput();
            Assert.DoesNotContain("\u001b[", output);
        }

        [Fact]
        public void WriteEasyTitledBox_NoColor_HasNoAnsi()
        {
            using var capture = new ConsoleCapture();
            using var ps = CreatePowerShell();
            ps.AddScript("'only' | Write-TitledBox -Title Demo -NoColor");
            var results = ps.Invoke();
            Assert.True(results.Count >= 2); // at least border + content
            var output = capture.GetOutput();
            Assert.DoesNotContain("\u001b[", output);
        }

        [Fact]
        public void WriteEasyRule_AutoWidth_WidthZero()
        {
            using var ps = CreatePowerShell();
            // Width default (0) -> auto, expect > 0 length, likely 80 fallback
            ps.AddCommand("Write-Rule").AddParameter("Title", "Auto");
            var results = ps.Invoke();
            Assert.Single(results);
            var line = results[0].BaseObject?.ToString();
            Assert.NotNull(line);
            Assert.True(line!.Length > 10); // simple sanity; environment width or fallback >= 80 typically
            Assert.Contains("Auto", line);
        }
    }
}
