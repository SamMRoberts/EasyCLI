namespace EasyCLI.Tests;

public class EnhancedCliTests
{
    [Fact]
    public void CommandLineArgs_ParsesFlags_Correctly()
    {
        var args = new CommandLineArgs(new[] { "--help", "--verbose", "-q" });

        Assert.True(args.IsHelpRequested);
        Assert.True(args.IsVerbose);
        Assert.True(args.IsQuiet);
        Assert.False(args.IsDryRun);
    }

    [Fact]
    public void CommandLineArgs_ParsesOptions_Correctly()
    {
        var args = new CommandLineArgs(new[] { "--output", "file.txt", "--repeat=3", "-r", "5" });

        Assert.Equal("file.txt", args.GetOption("output"));
        Assert.Equal("3", args.GetOption("repeat"));
        Assert.Equal("5", args.GetOption("r"));
    }

    [Fact]
    public void CommandLineArgs_ParsesPositionalArguments_Correctly()
    {
        var args = new CommandLineArgs(new[] { "command", "arg1", "arg2", "--flag" });

        Assert.Equal(3, args.Arguments.Count);
        Assert.Equal("command", args.GetArgument(0));
        Assert.Equal("arg1", args.GetArgument(1));
        Assert.Equal("arg2", args.GetArgument(2));
        Assert.True(args.HasFlag("flag"));
    }

    [Fact]
    public void ExitCodes_HasStandardValues()
    {
        Assert.Equal(0, ExitCodes.Success);
        Assert.Equal(1, ExitCodes.GeneralError);
        Assert.Equal(2, ExitCodes.FileNotFound);
        Assert.Equal(4, ExitCodes.InvalidArguments);
        Assert.Equal(127, ExitCodes.CommandNotFound);
    }
}
