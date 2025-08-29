# EasyCLI Wiki

Welcome to the **EasyCLI** documentation wiki! EasyCLI is a comprehensive .NET 9.0 class library for building modern command-line interfaces, PowerShell cmdlets, and interactive console applications.

## Quick Links

- **[Getting Started](Getting-Started)** - Installation and first steps
- **[Core Features](Core-Features)** - ANSI styling, prompts, and basic functionality  
- **[CLI Enhancement Features](CLI-Enhancement-Features)** - Professional CLI patterns and enterprise features
- **[Shell Framework](Shell-Framework)** - Interactive shell and custom commands
- **[Output and Scripting](Output-and-Scripting)** - Output formats and automation support
- **[API Reference](API-Reference)** - Core classes and interfaces
- **[Examples and Tutorials](Examples-and-Tutorials)** - Practical usage examples
- **[Contributing](Contributing)** - Development guidelines and contribution process

## What's New in v0.2.0

**Phase 2 introduces professional CLI best practices and enterprise-ready features:**

- 🔧 **Enhanced CLI Framework**: `EnhancedCliCommand` base class with built-in CLI patterns
- ⚠️ **Dangerous Operation Confirmation**: Safety framework for destructive operations  
- 📋 **Configuration Management**: Hierarchical JSON configuration system
- 🌍 **Environment Detection**: Automatic Git, Docker, CI/CD detection
- 📊 **Structured Logging**: Professional logging with verbosity levels
- ⚡ **PowerShell Module v0.2.0**: Enhanced cmdlets with pipeline support
- 🏗️ **Professional CLI Patterns**: Dry-run mode, smart errors, standardized arguments

## Key Features at a Glance

### Core Framework
- **🎨 ANSI Styling**: Rich console output with colors and themes
- **💬 Interactive Prompts**: String, choice, hidden input prompts with validation
- **⚡ PowerShell Integration**: Ready-to-use cmdlets (`Write-Message`, `Read-Choice`)
- **🐚 Persistent Shell**: Interactive shell with custom command support
- **📊 Rich Formatting**: Tables, boxes, rules, key-value pairs
- **🌗 Theme Support**: Dark, Light, HighContrast themes
- **🌍 Environment Aware**: Respects `NO_COLOR`, `FORCE_COLOR`, output redirection

### CLI Enhancement Features (v0.2.0+)
- **⚙️ Configuration Management**: Global and local JSON configuration
- **🔍 Environment Detection**: Git repos, Docker containers, CI/CD environments
- **📝 Structured Logging**: Verbosity levels with automatic CLI flag integration
- **🛠️ Enhanced Commands**: Built-in help system and argument validation
- **🏃‍♂️ Dry-Run Support**: Safe operation previews
- **🛡️ Dangerous Operation Confirmation**: Safety framework with automation detection
- **⚠️ Smart Error Handling**: Intelligent suggestions and recovery

## Repository Information

- **Repository**: [SamMRoberts/EasyCLI](https://github.com/SamMRoberts/EasyCLI)
- **NuGet Package**: [SamMRoberts.EasyCLI](https://www.nuget.org/packages/SamMRoberts.EasyCLI)
- **License**: MIT License
- **Target Framework**: .NET 9.0
- **PowerShell Module**: Available on PowerShell Gallery

## Quick Start

```bash
# Install the NuGet package
dotnet add package SamMRoberts.EasyCLI

# Or install PowerShell module
Install-Module -Name EasyCLI
```

```csharp
// Basic usage example
using EasyCLI.Console;

var writer = new ConsoleWriter();
var theme = ConsoleThemes.Dark;

writer.WriteSuccessLine("✓ Welcome to EasyCLI!", theme);
writer.WriteInfoLine("Building beautiful CLIs made easy.", theme);
```

## Need Help?

- 📖 **Documentation**: Browse this wiki for comprehensive guides
- 🐛 **Issues**: [GitHub Issues](https://github.com/SamMRoberts/EasyCLI/issues) for bug reports
- 💬 **Discussions**: [GitHub Discussions](https://github.com/SamMRoberts/EasyCLI/discussions) for questions
- 📋 **Output Contract**: [Scripting Guide](Output-and-Scripting) for automation stability guarantees

---

**Made with ❤️ by the EasyCLI contributors**