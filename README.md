# EasyCLI 

<img src="https://img.shields.io/github/actions/workflow/status/SamMRoberts/EasyCLI/ci.yml?branch=main" alt="CI"/>
<img src="https://img.shields.io/github/actions/workflow/status/SamMRoberts/EasyCLI/status-checks.yml?branch=main" alt="Status Checks"/>
<img src="https://img.shields.io/github/actions/workflow/status/SamMRoberts/EasyCLI/version-tag-publish.yml?branch=main" alt="Version/Tag/Publish"/>
<a href="https://www.nuget.org/packages/SamMRoberts.EasyCLI"><img src="https://img.shields.io/nuget/v/SamMRoberts.EasyCLI.svg" alt="NuGet"></a>
<a href="https://www.nuget.org/packages/SamMRoberts.EasyCLI"><img src="https://img.shields.io/nuget/dt/SamMRoberts.EasyCLI.svg" alt="NuGet Downloads"></a>
<a href="https://github.com/SamMRoberts/EasyCLI/releases"><img src="https://img.shields.io/github/v/release/SamMRoberts/EasyCLI?include_prereleases&sort=semver" alt="GitHub Release"></a>
<img src="https://img.shields.io/badge/GitHub%20Packages-active-brightgreen" alt="GitHub Packages"/>

EasyCLI is a comprehensive .NET 9.0 class library for building modern command-line interfaces, PowerShell cmdlets, and interactive console applications. It provides professional-grade ANSI styling, interactive prompts, and an experimental persistent shell framework.

## ✨ Features

- **🎨 ANSI Styling**: Rich console output with colors, themes, and environment-aware formatting
- **💬 Interactive Prompts**: String, integer, yes/no, hidden input, choice, and multi-select prompts with validation
- **⚡ PowerShell Integration**: Ready-to-use cmdlets (`Write-Message`, `Write-Rule`, `Write-TitledBox`, `Read-Choice`)
- **🐚 Persistent Shell**: Experimental interactive shell with custom command support and external process delegation
- **📊 Rich Formatting**: Tables, titled boxes, rules, key-value pairs, and wrapped text
- **🌗 Theme Support**: Built-in themes (Dark, Light, HighContrast) with customizable styles
- **🌍 Environment Aware**: Respects `NO_COLOR`, `FORCE_COLOR`, and output redirection
- **🔧 Extensible**: Plugin-friendly architecture with `ICliCommand` interface

## 📚 Table of Contents

- [Requirements](#requirements)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [ANSI Styling](#ansi-styling)
- [Interactive Prompts](#interactive-prompts)
- [Persistent Shell](#persistent-shell)
- [PowerShell Cmdlets](#powershell-cmdlets)
- [Project Structure](#project-structure)
- [Development](#development)
- [Contributing](#contributing)
- [Versioning](#versioning)
- [License](#license)

## 📋 Requirements

- **.NET Framework**: .NET 9.0 or later
- **PowerShell** (for cmdlets): PowerShell 7.0+ recommended
- **Terminal Support**: 
  - ✅ Modern terminals with ANSI support (Windows Terminal, VS Code terminal, iTerm2, etc.)
  - ✅ Console applications and CI/CD environments
  - ✅ Output redirection and piping (colors auto-disabled unless forced)
  - ⚠️ Limited support for legacy Windows Command Prompt (colors may not display)

**Environment Variables**:
- `NO_COLOR=1` - Disables all colored output globally
- `FORCE_COLOR=1` - Forces colored output even when redirected (overrides `NO_COLOR`)
- `CI=1` - Detected automatically for CI environments

## 📦 Installation

### .NET CLI
```sh
dotnet add package SamMRoberts.EasyCLI
```

### Package Reference
```xml
<ItemGroup>
  <PackageReference Include="SamMRoberts.EasyCLI" Version="*" />
</ItemGroup>
```

### PowerShell
```powershell
Install-Package SamMRoberts.EasyCLI
```

### GitHub Packages (Alternative)
Add to your `nuget.config`:
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget" value="https://api.nuget.org/v3/index.json" />
    <add key="github" value="https://nuget.pkg.github.com/SamMRoberts/index.json" />
  </packageSources>
</configuration>
```

Authenticate once:
```sh
dotnet nuget add source https://nuget.pkg.github.com/SamMRoberts/index.json \
  --name github --username "SamMRoberts" --password "$GITHUB_TOKEN" --store-password-in-clear-text
```

## 🚀 Quick Start

### Console Styling
```csharp
using EasyCLI.Console;
using EasyCLI.Styling;

var writer = new ConsoleWriter();

// Basic styled output
writer.WriteSuccessLine("✓ Operation completed successfully");
writer.WriteWarningLine("⚠ Proceed with caution");
writer.WriteErrorLine("✗ Something went wrong");
writer.WriteInfoLine("ℹ Processing 142 files...");
writer.WriteHintLine("💡 Tip: Use --verbose for detailed output");

// Themed output
var theme = ConsoleThemes.Dark; // or Light, HighContrast
writer.WriteHeadingLine("System Status", theme);
writer.WriteKeyValues(new[]
{
    ("CPU Usage", "45%"),
    ("Memory", "2.1GB / 8GB"),
    ("Status", "Healthy")
}, keyStyle: theme.Info);
```

### Interactive Prompts
```csharp
using EasyCLI.Console;
using EasyCLI.Prompts;
using EasyCLI.Prompts.Validators;

var reader = new ConsoleReader();
var writer = new ConsoleWriter();

// Simple prompts
var name = new StringPrompt("Your name", writer, reader, @default: "Anonymous").GetValue();
var age = new IntPrompt("Your age", writer, reader, validator: new IntRangeValidator(1, 120)).GetValue();
var proceed = new YesNoPrompt("Continue?", writer, reader, @default: true).GetValue();

// Choice selection
var choices = new[]
{
    new Choice<string>("Start Server", "start"),
    new Choice<string>("Stop Server", "stop"),
    new Choice<string>("Restart Server", "restart")
};
var action = new ChoicePrompt<string>("Action", choices, writer, reader).GetValue();

writer.WriteInfoLine($"Hello {name} (age {age}), action: {action}, proceed: {proceed}");
```

### Building a Simple CLI
```csharp
using EasyCLI.Shell;

// Create and configure shell
var reader = new ConsoleReader();
var writer = new ConsoleWriter();
var shell = new CliShell(reader, writer, new ShellOptions
{
    Prompt = "myapp>",
    PromptStyle = ConsoleThemes.Dark.Info
});

// Register custom commands
shell.Register(new MyCustomCommand());

// Start interactive session
await shell.RunAsync();
```

## 🎨 ANSI Styling

EasyCLI provides comprehensive ANSI styling capabilities with automatic terminal detection and environment-aware behavior.

### Basic Styling

```csharp
using EasyCLI.Console;
using EasyCLI.Styling;

var writer = new ConsoleWriter();

// Semantic styles
writer.WriteHeadingLine("Application Status");
writer.WriteSuccessLine("✓ Database connected");
writer.WriteWarningLine("⚠ Cache at 90% capacity");
writer.WriteErrorLine("✗ Service unavailable");
writer.WriteInfoLine("ℹ Processing requests...");
writer.WriteHintLine("💡 Use --verbose for more details");

// Custom colors with TrueColor support
var purple = ConsoleStyles.TrueColor(180, 0, 200);
writer.WriteLine("Custom colored text", purple);

// Basic formatting
writer.WriteLine("Bold text", ConsoleStyles.Bold);
writer.WriteLine("Italic text", ConsoleStyles.Italic);
writer.WriteLine("Underlined text", ConsoleStyles.Underline);
```

### Built-in Themes

EasyCLI includes three built-in themes optimized for different terminal backgrounds:

```csharp
// Dark theme (default for dark terminals)
var darkTheme = ConsoleThemes.Dark;

// Light theme (optimized for light backgrounds)
var lightTheme = ConsoleThemes.Light;

// High contrast theme (accessibility focused)
var highContrastTheme = ConsoleThemes.HighContrast;

// Use with any styled output
writer.WriteSuccessLine("Themed success message", darkTheme);
writer.WriteErrorLine("Themed error message", lightTheme);
```

### Custom Themes

Create your own theme by combining existing styles:

```csharp
var customTheme = new ConsoleTheme
{
    Success = ConsoleStyles.FgBrightGreen,
    Warning = ConsoleStyles.FgYellow,
    Error = ConsoleStyles.FgBrightRed,
    Heading = ConsoleStyles.Bold + ConsoleStyles.FgCyan,
    Info = ConsoleStyles.FgBlue,
    Hint = ConsoleStyles.Dim
};

writer.WriteHeadingLine("Custom Themed Heading", customTheme);
```

### Rich Formatting

```csharp
// Tables
writer.WriteTableSimple(
    new[] { "Name", "Status", "Uptime" },
    new[]
    {
        new[] { "Web Server", "Running", "5d 12h" },
        new[] { "Database", "Running", "12d 3h" },
        new[] { "Cache", "Warning", "1d 8h" }
    },
    headerStyle: ConsoleStyles.Heading
);

// Key-value pairs
writer.WriteKeyValues(new[]
{
    ("Version", "1.2.3"),
    ("Build", "2024.01.15"),
    ("Environment", "Production")
}, keyStyle: ConsoleStyles.Info);

// Titled boxes
writer.WriteTitledBox(
    new[] { "Important information goes here", "Multiple lines supported" },
    title: "Notice",
    borderStyle: ConsoleStyles.Warning
);

// Rules and dividers
writer.WriteRule("Section Title", filler: '=', titleStyle: ConsoleStyles.Heading);
```

### Environment Control

EasyCLI automatically respects standard environment variables and output conditions:

| Condition | Behavior |
|-----------|----------|
| `NO_COLOR=1` | Disables all ANSI styling globally |
| `FORCE_COLOR=1` | Forces colors even when output is redirected |
| Output redirected | Colors disabled automatically (unless `FORCE_COLOR=1`) |
| CI environment | Colors disabled automatically (unless `FORCE_COLOR=1`) |
| Unsupported terminal | Colors disabled automatically |

```csharp
// Check color support programmatically
if (writer.SupportsColor)
{
    writer.WriteSuccessLine("Colors are supported!");
}
else
{
    writer.WriteLine("Running without colors");
}
```

## 💬 Interactive Prompts

EasyCLI provides a comprehensive prompting framework for building interactive console applications with built-in validation, theming, and user-friendly error handling.

### Basic Prompts

#### String Input
```csharp
using EasyCLI.Console;
using EasyCLI.Prompts;
using EasyCLI.Prompts.Validators;

var writer = new ConsoleWriter();
var reader = new ConsoleReader();

// Simple string prompt
var name = new StringPrompt("Enter your name", writer, reader).GetValue();

// With default value
var username = new StringPrompt("Username", writer, reader, @default: "anonymous").GetValue();

// With validation
var email = new StringPrompt("Email address", writer, reader,
    validator: new RegexValidator(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", "Please enter a valid email address"))
    .GetValue();
```

#### Numeric Input
```csharp
// Integer input
var age = new IntPrompt("Age", writer, reader).GetValue();

// With range validation
var percentage = new IntPrompt("Completion percentage", writer, reader,
    validator: new IntRangeValidator(0, 100, "Percentage must be between 0 and 100"))
    .GetValue();

// With default value
var port = new IntPrompt("Port number", writer, reader, @default: 8080).GetValue();
```

#### Yes/No Confirmation
```csharp
// Simple yes/no
var proceed = new YesNoPrompt("Continue with operation?", writer, reader).GetValue();

// With default value
var overwrite = new YesNoPrompt("Overwrite existing file?", writer, reader, @default: false).GetValue();
```

#### Hidden Input (Passwords)
```csharp
using EasyCLI.Prompts;

// Secure password input
var password = new HiddenInputPrompt("Password", writer, reader,
    hiddenSource: new ConsoleHiddenInputSource()).GetValue();

// With default for testing
var secret = new HiddenInputPrompt("API Key", writer, reader,
    hiddenSource: new ConsoleHiddenInputSource(),
    @default: "test-key-123").GetValue();
```

### Choice Prompts

#### Single Choice Selection
```csharp
// Simple choice prompt
var environments = new[]
{
    new Choice<string>("Development", "dev"),
    new Choice<string>("Staging", "staging"), 
    new Choice<string>("Production", "prod")
};

var environment = new ChoicePrompt<string>("Target environment", environments, writer, reader)
    .GetValue();

// With filtering and paging for large lists
var fruits = new[]
{
    new Choice<string>("Apple", "apple"),
    new Choice<string>("Banana", "banana"),
    new Choice<string>("Cherry", "cherry"),
    // ... many more items
};

var options = new PromptOptions 
{ 
    PageSize = 10, 
    EnablePaging = true, 
    EnableFiltering = true 
};

var fruit = new ChoicePrompt<string>("Pick a fruit", fruits, writer, reader,
    options: options, keyReader: new ConsoleKeyReader()).GetValue();
```

#### Multi-Select Prompts
```csharp
// Multiple selection with checkboxes
var features = new[]
{
    new Choice<string>("Authentication", "auth"),
    new Choice<string>("Logging", "logging"),
    new Choice<string>("Caching", "caching"),
    new Choice<string>("Monitoring", "monitoring")
};

var selectedFeatures = new MultiSelectPrompt<string>("Select features to enable", features, writer, reader)
    .GetValue();

writer.WriteInfoLine($"Selected: {string.Join(", ", selectedFeatures)}");
```

### Advanced Validation

#### Built-in Validators
```csharp
using EasyCLI.Prompts.Validators;

// Required field validation
var required = new StringPrompt("Project name", writer, reader,
    validator: new RequiredValidator("Project name is required")).GetValue();

// Numeric range validation
var score = new IntPrompt("Score", writer, reader,
    validator: new IntRangeValidator(0, 100)).GetValue();

// Regular expression validation
var phoneNumber = new StringPrompt("Phone number", writer, reader,
    validator: new RegexValidator(@"^\+?[\d\s\-\(\)]+$", "Invalid phone number format"))
    .GetValue();

// File path validation
var configFile = new StringPrompt("Config file path", writer, reader,
    validator: new FileExistsValidator("Configuration file must exist")).GetValue();
```

#### Custom Validators
```csharp
// Create custom validator
public class UniqueUsernameValidator : IPromptValidator<string>
{
    private readonly string[] _existingUsernames;
    
    public UniqueUsernameValidator(string[] existingUsernames)
    {
        _existingUsernames = existingUsernames;
    }
    
    public PromptValidationResult Validate(string value)
    {
        if (_existingUsernames.Contains(value, StringComparer.OrdinalIgnoreCase))
        {
            return new PromptValidationResult(false, "Username already exists");
        }
        return new PromptValidationResult(true);
    }
}

// Use custom validator
var existingUsers = new[] { "admin", "user1", "guest" };
var newUsername = new StringPrompt("New username", writer, reader,
    validator: new UniqueUsernameValidator(existingUsers)).GetValue();
```

### Themed Prompts

```csharp
// Apply consistent theming to prompts
var theme = ConsoleThemes.Dark;

// Prompts automatically inherit the writer's theme support
var styledName = new StringPrompt("Enter name", writer, reader).GetValue();

// Error messages and validation feedback use theme colors
var validatedInput = new StringPrompt("Email", writer, reader,
    validator: new RegexValidator(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", "Invalid email"))
    .GetValue();
```

### Prompt Options and Behavior

```csharp
// Configure prompt behavior
var options = new PromptOptions
{
    PageSize = 15,           // Items per page for choice prompts
    EnablePaging = true,     // Allow paging through large lists
    EnableFiltering = true,  // Allow filtering choices by typing
    CancelBehavior = PromptCancelBehavior.ReturnDefault // or ThrowException
};

var choice = new ChoicePrompt<string>("Select option", choices, writer, reader, options: options)
    .GetValue();
```

### Error Handling

```csharp
try
{
    var result = new StringPrompt("Required input", writer, reader,
        validator: new RequiredValidator()).GetValue();
}
catch (PromptCanceledException)
{
    writer.WriteWarningLine("Operation cancelled by user");
    return ExitCodes.UserCanceled;
}
```

## 🐚 Persistent Shell

> **⚠️ Experimental Feature**: The shell API is currently experimental and may evolve before 1.0 stabilization. Feedback is welcome!

EasyCLI includes an experimental persistent shell that allows users to enter an interactive session and execute commands without repeatedly prefixing with an executable name. The shell provides built-in commands, external process delegation, and extensible custom commands.

### Built-in Commands

The shell includes essential commands out of the box:

| Command | Aliases | Description |
|---------|---------|-------------|
| `help` | `?` | List commands or show command details |
| `history` | `hist` | Display recent command history |
| `pwd` | | Print current working directory |
| `cd <dir>` | | Change working directory |
| `clear` | `cls` | Clear the screen |
| `complete <prefix>` | | List command name completions |
| `exit` / `quit` | | Leave the shell (Ctrl+D also works) |

### Quick Start

```csharp
using EasyCLI.Console;
using EasyCLI.Shell;

var reader = new ConsoleReader();
var writer = new ConsoleWriter();

var shell = new CliShell(reader, writer, new ShellOptions
{
    Prompt = "myapp>",
    PromptStyle = ConsoleThemes.Dark.Info,
    HistoryLimit = 1000
});

// Register custom commands
shell.Register(new DeployCommand());
shell.Register(new StatusCommand());

// Start interactive session (runs until exit/quit)
await shell.RunAsync();
```

### Custom Command Example

Create sophisticated commands using the `BaseCliCommand` class for enhanced features:

```csharp
using EasyCLI.Shell;

public class DeployCommand : BaseCliCommand
{
    public override string Name => "deploy";
    public override string Description => "Deploy application to specified environment";

    protected override void ConfigureHelp(CommandHelp help)
    {
        help.Usage = "deploy [options] <environment>";
        help.Description = "Deploys the application to the specified environment with optional configuration.";

        help.Arguments.Add(new CommandArgument("environment", 
            "Target environment (dev, staging, production)", required: true));

        help.Options.Add(new CommandOption("dry-run", "n", 
            "Show what would be deployed without executing"));
        help.Options.Add(new CommandOption("verbose", "v", 
            "Enable verbose output"));
        help.Options.Add(new CommandOption("config", "c", 
            "Custom configuration file path", hasValue: true));
        help.Options.Add(new CommandOption("timeout", "t", 
            "Deployment timeout in minutes", hasValue: true, defaultValue: "30"));

        help.Examples.Add(new CommandExample("deploy staging", 
            "Deploy to staging environment"));
        help.Examples.Add(new CommandExample("deploy production --dry-run", 
            "Preview production deployment"));
        help.Examples.Add(new CommandExample("deploy dev --config custom.json --verbose", 
            "Deploy to dev with custom config and verbose output"));
    }

    protected override int ValidateArguments(CommandLineArgs args, ShellExecutionContext context)
    {
        if (args.Arguments.Count == 0)
        {
            context.Writer.WriteErrorLine("Error: Environment argument is required");
            ShowSuggestion(context, "Usage: deploy <environment>");
            return ExitCodes.InvalidArguments;
        }

        string environment = args.Arguments[0];
        string[] validEnvironments = ["dev", "staging", "production"];
        
        if (!validEnvironments.Contains(environment.ToLowerInvariant()))
        {
            context.Writer.WriteErrorLine($"Error: Invalid environment '{environment}'");
            context.Writer.WriteHintLine($"Valid environments: {string.Join(", ", validEnvironments)}");
            return ExitCodes.InvalidArguments;
        }

        return ExitCodes.Success;
    }

    protected override async Task<int> ExecuteCommand(CommandLineArgs args, ShellExecutionContext context, CancellationToken cancellationToken)
    {
        string environment = args.Arguments[0];
        var theme = GetTheme(context);

        context.Writer.WriteHeadingLine($"Deploying to {environment.ToUpperInvariant()}", theme);

        if (args.IsDryRun)
        {
            context.Writer.WriteWarningLine("[DRY RUN] Deployment simulation", theme);
        }

        // Simulate deployment steps
        string[] steps = ["Building application", "Running tests", "Creating package", "Uploading artifacts", "Updating configuration"];
        
        for (int i = 0; i < steps.Length; i++)
        {
            string step = steps[i];
            context.Writer.WriteInfoLine($"[{i + 1}/{steps.Length}] {step}...", theme);
            
            if (args.IsVerbose)
            {
                context.Writer.WriteHintLine($"  Executing step: {step}", theme);
            }

            // Simulate work
            await Task.Delay(args.IsDryRun ? 100 : 1000, cancellationToken);
            
            context.Writer.WriteSuccessLine($"  ✓ {step} completed", theme);
        }

        if (args.IsDryRun)
        {
            context.Writer.WriteWarningLine("DRY RUN: No actual deployment performed", theme);
        }
        else
        {
            context.Writer.WriteSuccessLine($"🚀 Successfully deployed to {environment}!", theme);
        }

        return ExitCodes.Success;
    }

    protected override string[] GetCustomCompletions(ShellExecutionContext context, string prefix)
    {
        string[] environments = ["dev", "staging", "production"];
        string[] options = ["--dry-run", "--verbose", "--config", "--timeout"];
        
        return environments.Concat(options)
            .Where(item => item.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }
}
```

### Simple Command Interface

For simpler commands, implement `ICliCommand` directly:

```csharp
public class StatusCommand : ICliCommand
{
    public string Name => "status";
    public string Description => "Show application status";

    public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken ct)
    {
        var theme = ConsoleThemes.Dark;
        
        context.Writer.WriteHeadingLine("Application Status", theme);
        context.Writer.WriteKeyValues(new[]
        {
            ("Status", "Running"),
            ("Uptime", "2h 35m"),
            ("Memory Usage", "512MB"),
            ("Active Connections", "127")
        }, keyStyle: theme.Info);

        context.Writer.WriteSuccessLine("✓ All systems operational", theme);
        return Task.FromResult(0);
    }
}
```

### External Process Integration

The shell automatically executes external programs when no matching command is found:

```
myapp> git status
myapp> ls -la
myapp> dotnet --version
myapp> curl -s https://api.github.com/users/octocat
```

Output and error streams are captured and styled appropriately.

### Shell Configuration

```csharp
var options = new ShellOptions
{
    Prompt = "myapp>",                          // Custom prompt text
    PromptStyle = ConsoleThemes.Dark.Info,      // Prompt styling
    HistoryLimit = 1000,                        // Command history size
    WorkingDirectory = "/path/to/working/dir"   // Initial directory
};

var shell = new CliShell(reader, writer, options);
```

### Dynamic Command Registration

```csharp
// Register commands at runtime
await shell.RegisterAsync(new CustomCommand());

// Register simple delegate-based commands
shell.Register(new DelegateCommand("hello", "Say hello", async (ctx, args, ct) =>
{
    ctx.Writer.WriteSuccessLine("Hello from dynamic command!");
    return 0;
}));
```

### Future Roadmap

- **Command History Navigation**: Arrow key navigation and reverse search
- **Tab Completion**: Enhanced completion for commands and file paths
- **PowerShell Integration**: Direct cmdlet invocation within the shell
- **Configuration Files**: Startup scripts and user preferences (`~/.myapprc`)
- **Scripting Mode**: Multi-line input and basic scripting capabilities

## ⚡ PowerShell Cmdlets

EasyCLI ships with a PowerShell module (`EasyCLI.psd1`) that provides high-level cmdlets for styled console output and interactive prompts. The module is compatible with PowerShell 7.0+ and follows PowerShell best practices.

### Cmdlet Summary

| Cmdlet | Aliases | Purpose | Key Parameters |
|--------|---------|---------|----------------|
| `Write-Message` | `Show-Message` | Styled console output | `-Message`, `-Success`, `-Warning`, `-Error`, `-Info`, `-Hint` |
| `Write-Rule` | | Horizontal divider with optional title | `-Title`, `-Center`, `-PassThruObject` |
| `Write-TitledBox` | | Framed box around content | `-Title`, `-PassThruObject`, pipeline input |
| `Read-Choice` | `Select-EasyChoice` | Interactive choice selection | `-Options`, `-Select`, `-PassThruIndex`, `-PassThruObject` |

### Write-Message

Provides styled console output with semantic meaning and theme support.

```powershell
# Basic usage
Write-Message "Operation completed" -Success
Write-Message "Proceed with caution" -Warning  
Write-Message "Critical error occurred" -Error
Write-Message "Processing files..." -Info
Write-Message "Tip: Use -Verbose for details" -Hint

# Using alias for backward compatibility
Show-Message "Legacy alias still works" -Success

# Pipeline support
"Multiple", "Messages" | Write-Message -Info

# Disable colors for specific output
Write-Message "Plain text output" -Success -NoColor
```

**Parameters:**
- `-Message` (mandatory): The text to display
- `-Success`: Style as success message (green)
- `-Warning`: Style as warning message (yellow)  
- `-Error`: Style as error message (red)
- `-Info`: Style as informational message (cyan)
- `-Hint`: Style as hint/tip message (dim)
- `-NoColor`: Disable colored output for this message

### Write-Rule

Creates horizontal divider rules with optional centered titles.

```powershell
# Simple rule
Write-Rule

# Rule with title
Write-Rule -Title "Configuration Section"

# Centered title
Write-Rule -Title "Main Content" -Center

# Get structured output
$rule = Write-Rule -Title "Build Results" -PassThruObject
$rule | Format-List *
# Output:
# Title        : Build Results
# TotalWidth   : 80
# PaddingLeft  : 32
# PaddingRight : 32
# FillerChar   : -
```

**Parameters:**
- `-Title`: Optional title text to display in the rule
- `-Center`: Center the title within the rule
- `-PassThruObject`: Return a `RuleInfo` object instead of just displaying

### Write-TitledBox

Creates framed boxes around content with optional titles.

```powershell
# Simple box from pipeline
@("Line one", "Line two", "Line three") | Write-TitledBox -Title "Demo"

# Multiple inputs
"Important", "Information" | Write-TitledBox -Title "Notice"

# Get structured output
$box = @("Alpha", "Beta") | Write-TitledBox -Title "Data" -PassThruObject
$box | Format-List *
# Output:
# Title     : Data
# Lines     : {Alpha, Beta}
# Width     : 12
# HasTitle  : True
```

**Parameters:**
- `-Title`: Title to display above the box
- `-PassThruObject`: Return a `TitledBoxInfo` object with metadata
- Pipeline input: Content lines for the box

### Read-Choice

Interactive choice selection with support for objects, indices, and non-interactive operation.

```powershell
# Basic choice selection
$result = Read-Choice -Options "Alpha", "Beta", "Gamma" -Select 2
# Returns: "Beta"

# Non-interactive selection
$choice = Read-Choice -Options "Red", "Green", "Blue" -Select "Green"
# Returns: "Green"

# Return index instead of value  
$index = Read-Choice -Options "One", "Two", "Three" -Select "Two" -PassThruIndex
# Returns: 1

# Pipeline with objects (uses Name property)
$items = @(
    [PSCustomObject]@{Name="Development"; Url="dev.example.com"},
    [PSCustomObject]@{Name="Production"; Url="prod.example.com"}
)
$selected = $items | Read-Choice -Select 2
# Returns: "Production"

# Structured output with metadata
$choice = Read-Choice -Options "X", "Y", "Z" -Select "Y" -PassThruObject
$choice | Format-List *
# Output:
# SelectedValue : Y
# SelectedIndex : 1
# Options       : {X, Y, Z}
# WasInteractive: False

# Disable colors
Read-Choice -Options "Item1", "Item2" -Select 1 -NoColor
```

**Parameters:**
- `-Options`: Array of choice options (strings)
- `-Select`: Non-interactive selection (by index number or value)
- `-PassThruIndex`: Return the selected index instead of the value
- `-PassThruObject`: Return a `ChoiceSelection` object with metadata
- `-NoColor`: Disable colored output
- Pipeline input: Objects with `Name` property become options

### Environment Integration

All cmdlets respect standard environment variables:

```powershell
# Disable colors globally
$env:NO_COLOR = "1"
Write-Message "No colors" -Success

# Force colors even when redirected
$env:FORCE_COLOR = "1" 
Write-Message "Forced colors" -Warning | Out-File log.txt
```

### Error Handling

```powershell
# Cmdlets follow PowerShell error handling conventions
try {
    $result = Read-Choice -Options @() -Select 1  # Empty options
} catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Parameter validation
Write-Message -Success -Warning  # Error: Multiple style switches not allowed
```

### Module Installation in PowerShell

```powershell
# Install the NuGet package (makes module available)
Install-Package SamMRoberts.EasyCLI

# Import the module  
Import-Module EasyCLI

# List available commands
Get-Command -Module EasyCLI

# Get help for specific cmdlets
Get-Help Write-Message -Full
Get-Help Read-Choice -Examples
```

### Integration Examples

```powershell
# Build script with styled output
Write-Rule -Title "Build Process" -Center
Write-Message "Starting build..." -Info

try {
    # Build steps here
    Write-Message "✓ Build completed successfully" -Success
} catch {
    Write-Message "✗ Build failed: $($_.Exception.Message)" -Error
    exit 1
}

# Interactive deployment script
$environments = "Development", "Staging", "Production"
$env = Read-Choice -Options $environments -PassThruObject

if ($env.SelectedValue -eq "Production") {
    $confirm = Read-YesNo "Deploy to PRODUCTION? This cannot be undone."
    if (-not $confirm) {
        Write-Message "Deployment cancelled" -Warning
        exit 0
    }
}

Write-Message "Deploying to $($env.SelectedValue)..." -Info
# Deployment logic here
```

## 📁 Project Structure

```
EasyCLI/
├── EasyCLI/                          # Main class library
│   ├── Console/                      # Core console I/O classes
│   │   ├── ConsoleReader.cs          # Console input handling
│   │   └── ConsoleWriter.cs          # Console output with ANSI support
│   ├── Styling/                      # ANSI styling and themes
│   │   ├── ConsoleStyle.cs           # ANSI style definitions
│   │   ├── ConsoleStyles.cs          # Predefined style constants
│   │   └── ConsoleThemes.cs          # Built-in color themes
│   ├── Prompts/                      # Interactive prompt framework
│   │   ├── BasePrompt.cs             # Base prompt implementation
│   │   ├── StringPrompt.cs           # Text input prompts
│   │   ├── IntPrompt.cs              # Numeric input prompts
│   │   ├── YesNoPrompt.cs            # Boolean confirmation prompts
│   │   ├── HiddenInputPrompt.cs      # Password/secret input
│   │   ├── ChoicePrompt.cs           # Single-choice selection
│   │   ├── MultiSelectPrompt.cs      # Multiple-choice selection
│   │   └── Validators/               # Input validation framework
│   │       ├── RequiredValidator.cs  # Required field validation
│   │       ├── RegexValidator.cs     # Pattern matching validation
│   │       └── IntRangeValidator.cs  # Numeric range validation
│   ├── Shell/                        # Experimental persistent shell
│   │   ├── CliShell.cs               # Main shell implementation
│   │   ├── ICliCommand.cs            # Command interface
│   │   ├── BaseCliCommand.cs         # Enhanced command base class
│   │   ├── ShellOptions.cs           # Shell configuration
│   │   └── Commands/                 # Built-in command implementations
│   ├── Cmdlets/                      # PowerShell cmdlet implementations
│   │   ├── WriteMessageCommand.cs    # Write-Message cmdlet
│   │   ├── WriteRuleCommand.cs       # Write-Rule cmdlet
│   │   ├── WriteTitledBoxCommand.cs  # Write-TitledBox cmdlet
│   │   └── ReadChoiceCommand.cs      # Read-Choice cmdlet
│   ├── Formatting/                   # Advanced formatting utilities
│   ├── Extensions/                   # Extension methods
│   └── EasyCLI.psd1                  # PowerShell module manifest
├── EasyCLI.Tests/                    # Unit test suite (100 tests)
│   ├── ConsoleWriterTests.cs         # ANSI output testing
│   ├── PromptTests.cs                # Interactive prompt testing
│   ├── CmdletTests.cs                # PowerShell cmdlet testing
│   └── ShellTests.cs                 # Shell functionality testing
├── EasyCLI.Demo/                     # Demonstration application
│   └── Program.cs                    # Feature showcase and examples
├── .github/                          # GitHub configuration
│   ├── workflows/ci.yml              # Continuous integration pipeline
│   └── copilot-instructions.md       # Development guidelines
└── README.md                         # This documentation
```

### Key Components

- **Console I/O**: Thread-safe console reading and writing with ANSI support
- **Styling System**: Comprehensive ANSI styling with environment detection
- **Prompt Framework**: Extensible interactive input system with validation
- **Shell Framework**: Experimental command-line shell with custom command support
- **PowerShell Integration**: Native cmdlets following PowerShell conventions
- **Rich Formatting**: Tables, boxes, rules, and structured output helpers

## 🔧 Development

### Prerequisites

- **.NET 9.0 SDK** (required for building)
- **PowerShell 7.0+** (for testing cmdlets)
- **Git** (for source control)

### Build Process

```bash
# Clone the repository
git clone https://github.com/SamMRoberts/EasyCLI.git
cd EasyCLI

# Build the solution
dotnet build

# Run all tests
dotnet test --verbosity minimal

# Run the demo application
dotnet run --project EasyCLI.Demo/EasyCLI.Demo.csproj

# Format code (required before committing)
dotnet format
```

### Testing

The project includes comprehensive test coverage:

```bash
# Run all tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test categories
dotnet test --filter "Category=Console"
dotnet test --filter "Category=Prompts"  
dotnet test --filter "Category=Cmdlets"
dotnet test --filter "Category=Shell"
```

### Code Quality

The project enforces code quality through:

- **StyleCop Analyzers**: Consistent code formatting
- **NET Analyzers**: Code quality and performance rules
- **CI Pipeline**: Automated build, test, and formatting checks

```bash
# Check code formatting
dotnet format --verify-no-changes

# Run static analysis
dotnet build --configuration Release
```

## 🤝 Contributing

We welcome contributions! Please see our contributing guidelines for details.

### How to Contribute

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Make** your changes following our coding standards
4. **Test** your changes thoroughly
5. **Format** your code (`dotnet format`)
6. **Commit** your changes (`git commit -m 'Add amazing feature'`)
7. **Push** to the branch (`git push origin feature/amazing-feature`)
8. **Open** a Pull Request

### Development Guidelines

- Follow existing code patterns and conventions
- Add tests for new functionality
- Update documentation for user-facing changes
- Ensure all tests pass and code is properly formatted
- Keep changes focused and atomic

### Reporting Issues

When reporting issues, please include:

- .NET version and target framework
- Operating system and terminal details
- Clear reproduction steps
- Expected vs actual behavior
- Relevant code samples or error messages

## 📋 Versioning

This project follows [Semantic Versioning](https://semver.org/) (SemVer):

- **MAJOR**: Incompatible API changes
- **MINOR**: New functionality (backward compatible)
- **PATCH**: Bug fixes (backward compatible)

### Release Process

> 🔧 **Maintainer Information**: _This section should be updated by maintainers with specific release procedures_

1. Version numbers are managed automatically via CI/CD pipeline
2. Releases are published to NuGet.org and GitHub Packages
3. Release notes are generated from commit messages and PR descriptions
4. Pre-release versions use semantic versioning with preview tags

### Stability Status

- **Core APIs** (Console, Styling, Prompts): ✅ **Stable** - Following semantic versioning
- **PowerShell Cmdlets**: ✅ **Stable** - API considered stable for 1.0+
- **Shell Framework**: ⚠️ **Experimental** - API may change before 1.0 stabilization

## 📄 License

> 🔧 **Maintainer Information**: _Verify license terms and update as needed_

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

### MIT License Summary

- ✅ Commercial use permitted
- ✅ Modification permitted  
- ✅ Distribution permitted
- ✅ Private use permitted
- ❌ No warranty provided
- ❌ No liability assumed

### Third-Party Licenses

This project depends on:

- **Microsoft.PowerShell.SDK** - MIT License
- **Microsoft.SourceLink.GitHub** - MIT License  
- **StyleCop.Analyzers** - MIT License
- **Microsoft.CodeAnalysis.NetAnalyzers** - MIT License

---

**Made with ❤️ by the EasyCLI contributors**

For questions, suggestions, or support, please [open an issue](https://github.com/SamMRoberts/EasyCLI/issues) or start a [discussion](https://github.com/SamMRoberts/EasyCLI/discussions).
