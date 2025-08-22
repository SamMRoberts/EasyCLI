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
            Assert.Contains("─", results[0].BaseObject.ToString());
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
    }
}
