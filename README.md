# EasyCLI

This is a .NET (C#) class library project intended for building a PowerShell Cmdlet as a CLI tool.

## Getting Started

- The main code for your Cmdlet should be placed in the `EasyCLI` project folder.
- Build the project using:
  
  ```sh
  dotnet build
  ```
- To use your Cmdlet, you will need to register the resulting DLL with PowerShell.

## Project Structure

- `EasyCLI/` - Contains the class library source code.
- `.github/copilot-instructions.md` - Workspace-specific Copilot instructions.

## Next Steps

- Implement your Cmdlet class by inheriting from `System.Management.Automation.Cmdlet`.
- Add the necessary PowerShell attributes and logic.

---

For more details on authoring PowerShell Cmdlets in C#, see the official Microsoft documentation.
