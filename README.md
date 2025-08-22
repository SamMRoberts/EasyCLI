# EasyCLI <img src="https://img.shields.io/github/actions/workflow/status/SamMRoberts/EasyCLI/ci.yml?branch=main" alt="CI"/>

This is a .NET (C#) class library intended for building PowerShell Cmdlets and reusable CLI tooling. It includes a lightweight ANSI styling layer for console output.

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
