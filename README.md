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

## Project Structure

- `EasyCLI/` - Contains the class library source code.
  - `ConsoleStyle`, `ConsoleStyles`, `ConsoleWriter`, `ConsoleWriterExtensions`, `ConsoleReader`
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
