using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Xunit;

namespace EasyCLI.Tests
{
    public class ReadChoiceCmdletTests
    {
  private static PowerShell CreatePowerShell()
        {
            var iss = InitialSessionState.CreateDefault();
            iss.Commands.Add(new SessionStateCmdletEntry(
                "Read-Choice",
                typeof(EasyCLI.Cmdlets.ReadChoiceCommand),
                null));
      // Add alias explicitly since module manifest metadata isn't auto-loaded in this isolated runspace.
      iss.Commands.Add(new SessionStateAliasEntry(
        "Select-EasyChoice",
        "Read-Choice",
        string.Empty,
        ScopedItemOptions.None));
            // Alias test also relies on metadata but we add explicit entry for primary name.
            var rs = RunspaceFactory.CreateRunspace(iss);
            rs.Open();
            return PowerShell.Create(rs);
        }

        [Fact]
        public void ReadChoice_Select_By_Number()
        {
            using var ps = CreatePowerShell();
            ps.AddCommand("Read-Choice")
              .AddParameter("Options", new[] { "Alpha", "Beta", "Gamma" })
              .AddParameter("Select", "2");
            var results = ps.Invoke();
            Assert.Single(results);
            Assert.Equal("Beta", Assert.IsType<string>(results[0].BaseObject));
            Assert.False(ps.HadErrors);
        }

        [Fact]
        public void ReadChoice_Select_By_Label_Prefix()
        {
            using var ps = CreatePowerShell();
            ps.AddCommand("Read-Choice")
              .AddParameter("Options", new[] { "Start", "Status", "Stop" })
              .AddParameter("Select", "Sta"); // ambiguous prefix resolves to first exact or prefix (Start)
            var results = ps.Invoke();
            Assert.Single(results);
            Assert.Equal("Start", results[0].BaseObject);
        }

        [Fact]
        public void ReadChoice_PassThruIndex()
        {
            using var ps = CreatePowerShell();
            ps.AddCommand("Read-Choice")
              .AddParameter("Options", new[] { "Red", "Green", "Blue" })
              .AddParameter("Select", "Blue")
              .AddParameter("PassThruIndex");
            var results = ps.Invoke();
            Assert.Single(results);
            Assert.Equal(2, (int)results[0].BaseObject);
        }

        [Fact]
        public void ReadChoice_PassThruObject()
        {
            using var ps = CreatePowerShell();
            ps.AddCommand("Read-Choice")
              .AddParameter("Options", new[] { "One", "Two", "Three" })
              .AddParameter("Select", "3")
              .AddParameter("PassThruObject");
            var results = ps.Invoke();
            Assert.Single(results);
            var obj = Assert.IsType<EasyCLI.Cmdlets.ChoiceSelection>(results[0].BaseObject);
            Assert.Equal(2, obj.Index);
            Assert.Equal("Three", obj.Value);
        }

        [Fact]
        public void ReadChoice_InvalidSelection_ReportsError()
        {
            using var ps = CreatePowerShell();
            ps.AddCommand("Read-Choice")
              .AddParameter("Options", new[] { "One", "Two" })
              .AddParameter("Select", "99");
            var results = ps.Invoke();
            Assert.Empty(results);
            Assert.True(ps.HadErrors);
        }

        [Fact]
        public void ReadChoice_InvalidLabel_ReportsError()
        {
            using var ps = CreatePowerShell();
            ps.AddCommand("Read-Choice")
              .AddParameter("Options", new[] { "Apple", "Banana" })
              .AddParameter("Select", "Cherry");
            var results = ps.Invoke();
            Assert.Empty(results);
            Assert.True(ps.HadErrors);
        }

  [Fact]
  public void ReadChoice_EmptyOptions_ReportsError()
        {
            using var ps = CreatePowerShell();
            ps.AddCommand("Read-Choice")
              .AddParameter("Options", Array.Empty<string>())
              .AddParameter("Select", "1");
            var results = ps.Invoke();
            Assert.Empty(results);
            Assert.True(ps.HadErrors);
        }

        [Fact]
        public void ReadChoice_Alias_Works()
        {
            using var ps = CreatePowerShell();
            // Alias should behave the same; we add the command by canonical name, alias metadata resolves.
            ps.AddCommand("Select-EasyChoice")
              .AddParameter("Options", new[] { "Alpha", "Beta" })
              .AddParameter("Select", "Beta");
            var results = ps.Invoke();
            Assert.Single(results);
            Assert.Equal("Beta", results[0].BaseObject);
        }

        [Fact]
        public void ReadChoice_Cancel_On_Escape_Simulated()
        {
            using var ps = CreatePowerShell();
            // Simulate ESC then Enter (so user cancels before finalizing)
            ps.AddCommand("Read-Choice")
              .AddParameter("Options", new[] { "A", "B" })
              .AddParameter("CancelOnEscape")
              .AddParameter("SimulateKeys", "\u001b\n");
            var results = ps.Invoke();
            Assert.Empty(results); // cancelled => no output
            // PowerShell treats cancellation as no error (we didn't write one), ensure no errors flagged.
            Assert.False(ps.HadErrors);
        }

        [Fact]
        public void ReadChoice_DefaultSelection_Path()
        {
            using var ps = CreatePowerShell();
            // Simulate just Enter; Default should be returned
            ps.AddCommand("Read-Choice")
              .AddParameter("Options", new[] { "One", "Two" })
              .AddParameter("Default", "Two")
              .AddParameter("SimulateKeys", "\n");
            var results = ps.Invoke();
            Assert.Single(results);
            Assert.Equal("Two", results[0].BaseObject);
        }

        [Fact]
        public void ReadChoice_NoColor_HasNoAnsi()
        {
            using var capture = new ConsoleCapture();
            using var ps = CreatePowerShell();
            ps.AddCommand("Read-Choice")
              .AddParameter("Options", new[] { "Alpha" })
              .AddParameter("Select", "1")
              .AddParameter("NoColor");
            var results = ps.Invoke();
            Assert.Single(results);
            var text = capture.GetOutput();
            Assert.DoesNotContain("\u001b[", text); // No ANSI escapes
        }

        private class NamedThing { public string Name { get; set; } = string.Empty; }

  [Fact]
	public void ReadChoice_PipelineObjects_UsesNameProperty()
        {
            using var ps = CreatePowerShell();
            // Pipeline supplies objects with Name property; no explicit -Options needed.
      ps.AddScript(@"[PsCustomObject]@{ Name='Alpha' }, [PsCustomObject]@{ Name='Beta' } | Read-Choice -Select 2");
            var results = ps.Invoke();
            Assert.Single(results);
      Assert.Equal("Beta", Assert.IsType<string>(results[0].BaseObject));
        }

  [Fact]
  public void ReadChoice_PipelineObjects_PassThruObject()
  {
      using var ps = CreatePowerShell();
      ps.AddScript(@"[PsCustomObject]@{ Name='Zero' }, [PsCustomObject]@{ Name='One' } | Read-Choice -Select 2 -PassThruObject");
      var results = ps.Invoke();
      Assert.Single(results);
      var obj = Assert.IsType<EasyCLI.Cmdlets.ChoiceSelection>(results[0].BaseObject);
      Assert.Equal(1, obj.Index);
      Assert.Equal("One", obj.Value);
  }

    [Fact]
    public void ReadChoice_PipelineObjects_PassThruIndex()
    {
      using var ps = CreatePowerShell();
      ps.AddScript(@"[PsCustomObject]@{ Name='First' }, [PsCustomObject]@{ Name='Second' } | Read-Choice -Select Second -PassThruIndex");
      var results = ps.Invoke();
      Assert.Single(results);
      Assert.Equal(1, Assert.IsType<int>(results[0].BaseObject));
    }

        [Fact]
        public void ReadChoice_AmbiguousPrefix_PicksFirstMatch()
        {
            using var ps = CreatePowerShell();
            ps.AddCommand("Read-Choice")
              .AddParameter("Options", new[] { "Start", "Status", "Stop" })
              .AddParameter("Select", "Sta");
            var results = ps.Invoke();
            Assert.Single(results);
            Assert.Equal("Start", results[0].BaseObject);
        }
    }
}
