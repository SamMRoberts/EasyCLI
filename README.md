# EasyCLI 

<img src="https://img.shields.io/github/actions/workflow/status/SamMRoberts/EasyCLI/ci.yml?branch=main" alt="CI"/>
<img src="https://img.shields.io/github/actions/workflow/status/SamMRoberts/EasyCLI/status-checks.yml?branch=main" alt="Status Checks"/>
<img src="https://img.shields.io/github/actions/workflow/status/SamMRoberts/EasyCLI/version-tag-publish.yml?branch=main" alt="Version/Tag/Publish"/>
<a href="https://www.nuget.org/packages/SamMRoberts.EasyCLI"><img src="https://img.shields.io/nuget/v/SamMRoberts.EasyCLI.svg" alt="NuGet"></a>
<a href="https://www.nuget.org/packages/SamMRoberts.EasyCLI"><img src="https://img.shields.io/nuget/dt/SamMRoberts.EasyCLI.svg" alt="NuGet Downloads"></a>
<a href="https://github.com/SamMRoberts/EasyCLI/releases"><img src="https://img.shields.io/github/v/release/SamMRoberts/EasyCLI?include_prereleases&sort=semver" alt="GitHub Release"></a>
<img src="https://img.shields.io/badge/GitHub%20Packages-active-brightgreen" alt="GitHub Packages"/>

This is a .NET (C#) class library intended for building PowerShell Cmdlets and reusable CLI tooling. It includes a lightweight ANSI styling layer for console output.

## Install

Using the .NET CLI:

```sh
dotnet add package SamMRoberts.EasyCLI
```

Or add to your project file:

```xml
<ItemGroup>
  <PackageReference Include="SamMRoberts.EasyCLI" Version="*" />
</ItemGroup>
```

PowerShell (NuGet provider):

```powershell
Install-Package SamMRoberts.EasyCLI
```

GitHub Packages (alternative source):

Add or update a `nuget.config` (next to your solution) with:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget" value="https://api.nuget.org/v3/index.json" />
    <add key="github" value="https://nuget.pkg.github.com/SamMRoberts/index.json" />
  </packageSources>
</configuration>
```

Then authenticate once (CI or local):

```sh
dotnet nuget add source https://nuget.pkg.github.com/SamMRoberts/index.json \
  --name github --username "SamMRoberts" --password "$GITHUB_TOKEN" --store-password-in-clear-text
```

Restore/install as usual.

## Getting Started

- The main code for your Cmdlet should be placed in the `EasyCLI` project folder.
- Build the project using:
  
  ```sh
  dotnet build
  ```
- To use your Cmdlet, you will need to register the resulting DLL with PowerShell.

## ANSI styling quick start

```csharp
var w = new EasyCLI.ConsoleWriter();
w.WriteHeadingLine("EasyCLI");
w.WriteSuccessLine("Completed successfully");
w.WriteWarningLine("Proceed with caution");
w.WriteErrorLine("Something went wrong");
w.WriteInfoLine("FYI: hello");
w.WriteHintLine("Tip: set NO_COLOR=1 to disable colors");

// Truecolor example
var purple = EasyCLI.ConsoleStyles.TrueColor(180, 0, 200);
w.WriteLine("Vivid text", purple);
```

### Theme presets

Use built-in presets to quickly match your terminal background. You can also override any style.

```csharp
var w = new EasyCLI.ConsoleWriter();
var theme = EasyCLI.ConsoleThemes.Dark; // or Light, HighContrast
w.WriteHeadingLine("Themed Heading", theme);
w.WriteSuccessLine("Success message", theme);

// Override a couple of styles
var custom = new EasyCLI.ConsoleTheme
{
  Success = EasyCLI.ConsoleThemes.Dark.Success,
  Warning = EasyCLI.ConsoleStyles.FgMagenta,
  Error = EasyCLI.ConsoleThemes.Dark.Error,
  Heading = EasyCLI.ConsoleThemes.Dark.Heading,
  Info = EasyCLI.ConsoleThemes.Dark.Info,
  Hint = EasyCLI.ConsoleThemes.Dark.Hint
};
w.WriteWarningLine("Magenta warning", custom);
```

Environment controls:
- NO_COLOR=1 disables colors
- FORCE_COLOR=1 forces colors (overrides NO_COLOR)
- Colors are disabled when output is redirected unless forced

## Interactive Prompts

EasyCLI provides a lightweight prompting framework.

## Interactive Shell (Persistent CLI)

EasyCLI now includes an experimental persistent shell so users can enter a session once and invoke commands without repeatedly prefixing with an executable name. The shell provides:

Built‑in commands:
- `help` – List commands or show details (e.g. `help cd`)
- `history` – Display recent commands
- `pwd` – Print current working directory
- `cd <dir>` – Change working directory (affects subsequent external processes)
- `clear` – Clear the screen
- `complete <prefix>` – List command name completions
- `exit` / `quit` – Leave the shell (Ctrl+D / EOF also exits)

Other features:
- Executes external programs found on PATH (e.g. `ls`, `git status`, `dotnet --info`)
- Command history (configurable size) – future releases may add arrow‑key navigation
- Custom prompt text & style via `ShellOptions`
- Graceful handling of unknown commands (falls back to external process execution)

### Quick start

```csharp
using EasyCLI.Console;
using EasyCLI.Shell;

var reader = new ConsoleReader();
var writer = new ConsoleWriter();
var shell = new CliShell(reader, writer, new ShellOptions
{
  Prompt = "easy>"  // choose your own, e.g. "mytool>"
});

// Register a custom command (simple example)
shell.Register(new EchoCommand());

// Start loop (runs until user types exit/quit or EOF)
await shell.RunAsync();

// Example custom command
class EchoCommand : ICliCommand
{
  public string Name => "echo";
  public string Description => "Echo arguments back to the console";
  public Task<int> ExecuteAsync(ShellExecutionContext ctx, string[] args, CancellationToken ct)
  {
    ctx.Writer.WriteLine(string.Join(' ', args));
    return Task.FromResult(0);
  }
}
```

### Customizing the prompt

```csharp
var shell = new CliShell(reader, writer, new ShellOptions
{
  Prompt = "myapp>",
  PromptStyle = EasyCLI.ConsoleStyles.FgGreen,
  HistoryLimit = 1000
});
```

### Executing external commands

If a typed name does not match a registered `ICliCommand`, the shell attempts to launch it as an external process in the current working directory (changed via `cd`). Output and error streams are captured and written through the existing `ConsoleWriter` (stderr styled red when colors are enabled).

### Registering additional commands dynamically

```csharp
shell.Register(new DelegateCommand("hello", "Say hello", (ctx, args, ct) =>
{
  ctx.Writer.WriteLine("Hello from dynamic command");
  return Task.FromResult(0);
}));
```

`DelegateCommand` is an internal helper used for built‑ins; for public APIs prefer implementing `ICliCommand` directly (a thin adaptor pattern can be added later if needed).

### Roadmap (potential future enhancements)
- Command history navigation & reverse search
- Tab completion (names + file system)
- PowerShell runspace integration (invoke existing cmdlets directly)
- Configuration file & startup scripts (`~/.easyclirc`)
- Multi‑line block input and scripting mode

> Note: The shell is currently marked experimental; public API surfaces (`CliShell`, `ShellOptions`, `ICliCommand`) may evolve before 1.0 stabilization. Feedback welcome.

## PowerShell Cmdlets

EasyCLI ships a PowerShell module (see `EasyCLI.psd1`) exposing high-level cmdlets:

### Write-Message

Styled message output (supports alias `Show-Message`).

```powershell
Write-Message "Hello world" -Info
Show-Message "Legacy alias still works" -Success
```

### Write-Rule

Render a divider rule with optional title (centered with `-Center`). Use `-PassThruObject` to get a structured `RuleInfo` object.

```powershell
Write-Rule -Title "Section" -Center
$rule = Write-Rule -Title Build -PassThruObject
$rule | Format-List *
```

### Write-TitledBox

Render a framed titled box from pipeline input. Use `-PassThruObject` for a `TitledBoxInfo` object.

```powershell
@('Line one','Line two') | Write-TitledBox -Title Demo
@('Alpha','Beta') | Write-TitledBox -Title Data -PassThruObject | Format-List *
```

### Read-Choice (alias `Select-EasyChoice`)

Display a numbered menu and return the selected value. Supports non-interactive `-Select` and new pipeline-driven options.

```powershell
# Basic (explicit options)
Read-Choice -Options Alpha,Beta,Gamma -Select 2   # returns 'Beta'

# Pipeline options (objects with a Name property)
[pscustomobject]@{Name='One'},[pscustomobject]@{Name='Two'} | Read-Choice -Select 2   # returns 'Two'

# Get index instead of value
Read-Choice -Options Red,Green,Blue -Select Green -PassThruIndex   # returns 1

# Structured output
Read-Choice -Options A,B,C -Select C -PassThruObject | Format-List *

# Suppress color
Read-Choice -Options X -Select 1 -NoColor
```

Environment variable `NO_COLOR=1` disables color; `FORCE_COLOR=1` (planned) will force-enable.

### Basic string / int / yes-no

```csharp
var writer = new ConsoleWriter();
var reader = new ConsoleReader();
var name = new EasyCLI.Prompts.StringPrompt("Name", writer, reader, @default: "Anon").Get();
var age = new EasyCLI.Prompts.IntPrompt("Age", writer, reader).Get();
var proceed = new EasyCLI.Prompts.YesNoPrompt("Continue", writer, reader, @default: true).Get();
writer.WriteInfoLine($"Name={name}, Age={age}, Proceed={proceed}");
```

### Hidden input (password)

```csharp
var secret = new EasyCLI.Prompts.HiddenInputPrompt("Password", writer, reader, hiddenSource: new EasyCLI.Prompts.ConsoleHiddenInputSource()).Get();
```

### Validators

Use built-in validators or create your own.

```csharp
using EasyCLI.Prompts.Validators;

var percent = new EasyCLI.Prompts.IntPrompt(
  "Percent", writer, reader,
  validator: new IntRangeValidator(0,100)).Get();

var email = new EasyCLI.Prompts.StringPrompt(
  "Email", writer, reader,
  validator: new RegexValidator(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", "Invalid email"))
  .Get();
```

### Choice & Multi-select

```csharp
var fruits = new [] {
  new EasyCLI.Prompts.Choice<string>("Apple", "apple"),
  new EasyCLI.Prompts.Choice<string>("Banana", "banana"),
  new EasyCLI.Prompts.Choice<string>("Cherry", "cherry"),
};
var fruit = new EasyCLI.Prompts.ChoicePrompt<string>("Pick a fruit", fruits, writer, reader).Get();

var nums = new [] {
  new EasyCLI.Prompts.Choice<int>("One",1),
  new EasyCLI.Prompts.Choice<int>("Two",2),
  new EasyCLI.Prompts.Choice<int>("Three",3),
  new EasyCLI.Prompts.Choice<int>("Four",4),
};
var selected = new EasyCLI.Prompts.MultiSelectPrompt<int>("Select numbers", nums, writer, reader).Get();
```

## Project Structure

- `EasyCLI/` - Contains the class library source code.
  - `ConsoleStyle`, `ConsoleStyles`, `ConsoleWriter`, `ConsoleWriterExtensions`, `ConsoleReader`
  - `Prompts/` prompt abstractions, validators, and implementations (string, int, yes/no, hidden, choice, multi-select)
- `.github/copilot-instructions.md` - Workspace-specific Copilot instructions.
 - `EasyCLI.Tests/` - Unit tests for ANSI behavior and console helpers.

## Next Steps

- Implement your Cmdlet class by inheriting from `System.Management.Automation.Cmdlet`.
- Add the necessary PowerShell attributes and logic.

---

For more details on authoring PowerShell Cmdlets in C#, see the official Microsoft documentation.

## Run tests

```sh
dotnet test EasyCLI.Tests/EasyCLI.Tests.csproj -v minimal
```

## Run the demo

```sh
dotnet run --project EasyCLI.Demo/EasyCLI.Demo.csproj
```
