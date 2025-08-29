# Getting Started

This guide will help you get up and running with EasyCLI quickly.

## Prerequisites

- **.NET 9.0 SDK** - Required for building and running EasyCLI applications
- **PowerShell 5.1+** or **PowerShell Core 6+** (for PowerShell module features)

### Installing .NET 9.0 SDK

If you don't have .NET 9.0 SDK installed:

```bash
# Windows (using winget)
winget install Microsoft.DotNet.SDK.9

# macOS (using Homebrew)
brew install --cask dotnet

# Linux or manual installation
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version 9.0.101
```

Verify installation:
```bash
dotnet --version
# Should show 9.0.101 or later
```

## Installation

### NuGet Package

For .NET applications, install the NuGet package:

```bash
# Using .NET CLI
dotnet add package SamMRoberts.EasyCLI

# Using Package Manager Console in Visual Studio
Install-Package SamMRoberts.EasyCLI
```

### PowerShell Module

For PowerShell scripting, install the PowerShell module:

```powershell
# Install from PowerShell Gallery
Install-Module -Name EasyCLI

# Import the module
Import-Module EasyCLI
```

## Your First EasyCLI Application

### 1. Basic Console Styling

Create a new console application and add some colorful output:

```csharp
using EasyCLI.Console;

var writer = new ConsoleWriter();
var theme = ConsoleThemes.Dark;

// Basic styled output
writer.WriteSuccessLine("✓ Application started successfully!", theme);
writer.WriteWarningLine("⚠ This is a warning message", theme);
writer.WriteErrorLine("✗ This is an error message", theme);
writer.WriteInfoLine("ℹ This is informational", theme);

// Key-value pairs
writer.WriteKeyValues(new[]
{
    ("Version", "1.0.0"),
    ("Status", "Running"),
    ("Uptime", "2 hours")
}, keyStyle: theme.Info);
```

### 2. Interactive Prompts

Add user interaction with prompts:

```csharp
using EasyCLI.Console;
using EasyCLI.Prompts;

var writer = new ConsoleWriter();
var reader = new ConsoleReader();

// String input
var namePrompt = new StringPrompt("What's your name?", writer, reader);
var name = namePrompt.GetValue();

// Yes/No prompt
var confirmPrompt = new YesNoPrompt("Continue?", writer, reader, defaultValue: true);
var shouldContinue = confirmPrompt.GetValue();

// Choice selection
var choices = new[]
{
    new Choice<string>("Development", "dev"),
    new Choice<string>("Staging", "staging"),
    new Choice<string>("Production", "prod")
};
var choicePrompt = new ChoicePrompt<string>("Select environment", choices, writer, reader);
var environment = choicePrompt.GetValue();

writer.WriteSuccessLine($"Hello {name}! Environment: {environment}");
```

### 3. Simple CLI Shell

Create an interactive shell with custom commands:

```csharp
using EasyCLI.Console;
using EasyCLI.Shell;

// Create shell
var reader = new ConsoleReader();
var writer = new ConsoleWriter();
var shell = new CliShell(reader, writer, new ShellOptions
{
    Prompt = "myapp>",
    PromptStyle = ConsoleThemes.Dark.Info
});

// Register a simple command
await shell.RegisterAsync(new GreetCommand());

// Start the shell
await shell.RunAsync();

// Simple command implementation
public class GreetCommand : ICliCommand
{
    public string Name => "greet";
    public string Description => "Greets a user";

    public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken cancellationToken)
    {
        var name = args.Length > 0 ? args[0] : "World";
        context.Writer.WriteSuccessLine($"Hello, {name}!", ConsoleThemes.Dark);
        return Task.FromResult(0);
    }
}
```

## Enhanced CLI Commands (v0.2.0+)

For production-ready CLI applications, use the enhanced command framework:

```csharp
using EasyCLI.Shell;

public class DeployCommand : EnhancedCliCommand
{
    public override string Name => "deploy";
    public override string Description => "Deploy application to specified environment";

    protected override void ConfigureHelp(CommandHelp help)
    {
        help.Usage = "deploy [options] <environment>";
        help.Arguments.Add(new CommandArgument("environment", 
            "Target environment (dev, staging, production)", required: true));
        help.Options.Add(new CommandOption("dry-run", "n", 
            "Show what would be deployed without executing"));
        help.Examples.Add(new CommandExample("deploy staging --dry-run", 
            "Preview staging deployment"));
    }

    protected override async Task<int> ExecuteCommand(CommandLineArgs args, 
        ShellExecutionContext context, CancellationToken cancellationToken)
    {
        // Enhanced commands automatically provide:
        // - Logger with appropriate verbosity
        // - ConfigManager for configuration  
        // - Environment detection
        // - Proper error handling and exit codes

        if (args.IsDryRun)
        {
            Logger?.LogWarning("[DRY RUN] No changes will be made");
            // Preview logic here
            return ExitCodes.Success;
        }

        // Actual deployment logic
        Logger?.LogInfo($"Deploying to {args.Arguments[0]}...");
        Logger?.LogSuccess("Deployment completed successfully!");
        
        return ExitCodes.Success;
    }
}
```

## PowerShell Quick Start

If you prefer PowerShell, you can use EasyCLI cmdlets directly:

```powershell
# Import the module
Import-Module EasyCLI

# Write styled messages
Write-Message "Success!" -Style Success
Write-Message "Warning!" -Style Warning  
Write-Message "Error!" -Style Error

# Create interactive prompts
$name = Read-Choice "What's your name?" @("Alice", "Bob", "Charlie")
Write-Message "Hello $name!" -Style Info

# Use aliases for brevity
Show-Message "This works too!" -Style Success
```

## Next Steps

Now that you have EasyCLI working, explore these topics:

- **[Core Features](Core-Features)** - Learn about ANSI styling, themes, and prompts
- **[CLI Enhancement Features](CLI-Enhancement-Features)** - Professional CLI patterns and configuration
- **[Shell Framework](Shell-Framework)** - Building interactive shells and custom commands  
- **[Output and Scripting](Output-and-Scripting)** - Output formats for automation and scripting
- **[Examples and Tutorials](Examples-and-Tutorials)** - More detailed examples and patterns

## Common Issues

### .NET Version Mismatch
**Error**: `NETSDK1045: The current .NET SDK does not support targeting .NET 9.0`

**Solution**: Install .NET 9.0 SDK. EasyCLI requires .NET 9.0 and will not work with earlier versions.

### Colors Not Showing
**Issue**: ANSI colors not displaying in terminal

**Solutions**:
- Check if your terminal supports ANSI colors
- Ensure `NO_COLOR` environment variable is not set
- Try setting `FORCE_COLOR=1` to force color output
- Use Windows Terminal or a modern terminal emulator

### PowerShell Module Not Found
**Error**: `The specified module 'EasyCLI' was not loaded`

**Solution**: 
```powershell
# Install from PowerShell Gallery
Install-Module -Name EasyCLI -Force

# Or install for current user only
Install-Module -Name EasyCLI -Scope CurrentUser
```

## Getting Help

- 📖 **This Wiki** - Comprehensive documentation and guides
- 🐛 **[GitHub Issues](https://github.com/SamMRoberts/EasyCLI/issues)** - Bug reports and feature requests
- 💬 **[GitHub Discussions](https://github.com/SamMRoberts/EasyCLI/discussions)** - Questions and community support
- 📧 **Support** - See repository for contact information