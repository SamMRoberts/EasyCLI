using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Xunit;

namespace EasyCLI.Tests
{
    public class CmdletTests
    {
        private PowerShell CreatePowerShell()
        {
            // Load the EasyCLI assembly into a runspace so the cmdlet is available.
            var iss = InitialSessionState.CreateDefault();
            iss.Commands.Add(new SessionStateCmdletEntry(
                "Write-EasyMessage",
                typeof(EasyCLI.Cmdlets.ShowEasyMessageCommand),
                null));

            var rs = RunspaceFactory.CreateRunspace(iss);
            rs.Open();
            return PowerShell.Create(rs);
        }

        [Fact]
        public void Cmdlet_Writes_Object_And_Text_Success()
        {
            using var ps = CreatePowerShell();
            ps.AddCommand("Write-EasyMessage").AddParameter("Message", "Hello").AddParameter("Success");
            var results = ps.Invoke();

            Assert.Single(results);
            Assert.Equal("Hello", results[0].BaseObject);
            Assert.False(ps.HadErrors, "PowerShell reported errors");
        }

        [Theory]
        [InlineData("Success")]
        [InlineData("Warning")]
        [InlineData("Error")]
        [InlineData("Info")]
        [InlineData("Hint")]
        public void Cmdlet_Accepts_Style_Parameters(string flag)
        {
            using var capture = new ConsoleCapture();
            using var ps = CreatePowerShell();
            ps.AddCommand("Write-EasyMessage").AddParameter("Message", flag).AddParameter(flag);
            var results = ps.Invoke();
            Assert.Single(results);
            Assert.Equal(flag, results[0].BaseObject);
            var text = capture.GetOutput();
            // Expect at least one ANSI escape ("\u001b[") when color not disabled
            Assert.Contains("\u001b[", text);
        }

        [Fact]
        public void Cmdlet_NoColor_Does_NotThrow()
        {
            using var capture = new ConsoleCapture();
            using var ps = CreatePowerShell();
            ps.AddCommand("Write-EasyMessage").AddParameter("Message", "Plain").AddParameter("NoColor");
            var results = ps.Invoke();
            Assert.Single(results);
            Assert.Equal("Plain", results[0].BaseObject);
            var text = capture.GetOutput();
            Assert.DoesNotContain("\u001b[", text); // no ANSI sequences
        }
    }
}
