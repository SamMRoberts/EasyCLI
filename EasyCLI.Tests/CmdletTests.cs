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
    private static PowerShell CreatePowerShell()
        {
            // Load the EasyCLI assembly into a runspace so the cmdlet is available.
            var iss = InitialSessionState.CreateDefault();
            iss.Commands.Add(new SessionStateCmdletEntry(
                "Write-EasyMessage",
                typeof(EasyCLI.Cmdlets.WriteMessageCommand),
                null));
            // Add modern name without 'Easy' prefix
            iss.Commands.Add(new SessionStateCmdletEntry(
                "Write-Message",
                typeof(EasyCLI.Cmdlets.WriteMessageCommand),
                null));
            // Explicit alias for backward compatibility Show-Message
            iss.Commands.Add(new SessionStateAliasEntry(
                "Show-Message",
                "Write-Message",
                string.Empty,
                ScopedItemOptions.None));

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

        [Fact]
        public void PipelineInput_ProcessesAllItems()
        {
            using var capture = new ConsoleCapture();
            using var ps = CreatePowerShell();
            var script = "\"a\",\"b\",\"c\" | Write-EasyMessage -Success";
            ps.AddScript(script);
            var results = ps.Invoke();
            Assert.Equal(3, results.Count);
            var text = capture.GetOutput();
            Assert.Contains("a", text);
            Assert.Contains("b", text);
            Assert.Contains("c", text);
            // Expect success green style (32) three times
            var count = System.Text.RegularExpressions.Regex.Matches(text, "\\u001b\\[32m").Count;
            Assert.Equal(3, count);
        }

        [Fact]
        public void MultipleSwitches_FirstWins()
        {
            using var capture = new ConsoleCapture();
            using var ps = CreatePowerShell();
            ps.AddCommand("Write-EasyMessage")
              .AddParameter("Message", "Test")
              .AddParameter("Success")
              .AddParameter("Error"); // later flag should be ignored
            var results = ps.Invoke();
            Assert.Single(results);
            var text = capture.GetOutput();
            Assert.Contains("\u001b[32m", text); // success green
            Assert.DoesNotContain("\u001b[91m", text); // error bright red absent
        }

        [Fact]
        public void MissingMandatoryParameter_ShowsError()
        {
            using var ps = CreatePowerShell();
            ps.AddCommand("Write-EasyMessage").AddParameter("Success");
            // Invocation should throw ParameterBindingException due to missing mandatory Message
            var ex = Assert.Throws<System.Management.Automation.ParameterBindingException>(() => ps.Invoke());
            Assert.Contains("missing mandatory parameters", ex.Message, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void WriteMessage_Alias_ShowMessage_Works()
        {
            using var ps = CreatePowerShell();
            ps.AddCommand("Show-Message").AddParameter("Message", "Hello");
            var results = ps.Invoke();
            Assert.Single(results);
            Assert.Equal("Hello", results[0].BaseObject);
        }

        [Fact]
        public void WriteMessage_NoColor_HasNoAnsi()
        {
            using var capture = new ConsoleCapture();
            using var ps = CreatePowerShell();
            ps.AddCommand("Write-Message").AddParameter("Message", "Plain").AddParameter("NoColor");
            var results = ps.Invoke();
            Assert.Single(results);
            var text = capture.GetOutput();
            Assert.DoesNotContain("\u001b[", text);
        }
    }
}
