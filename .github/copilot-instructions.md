# EasyCLI
EasyCLI is a .NET 9.0 class library for building PowerShell Cmdlets and reusable CLI tooling. It includes a lightweight ANSI styling layer for console output, interactive prompts, and PowerShell integration.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Prerequisites and Setup
- **CRITICAL**: Requires .NET 9.0 SDK. The repo targets `net9.0` and will NOT build with .NET 8.0 or earlier.
- Install .NET 9.0 SDK if not available (auto-installs to `$HOME/.dotnet`):
  ```bash
  curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version 9.0.101 --install-dir $HOME/.dotnet
  export PATH="$HOME/.dotnet:$PATH"
  ```
- Set `PATH="$HOME/.dotnet:$PATH"` in your session to use the correct .NET version.

### Build and Test Process
- Build the project:
  ```bash
  dotnet build
  ```
  - **Timing**: First build takes ~50 seconds (includes package restore). Subsequent builds take ~2 seconds.
  - **NEVER CANCEL**: Set timeout to 120+ seconds for builds.

- Run all tests:
  ```bash
  dotnet test --verbosity minimal
  ```
  - **Timing**: Test suite takes ~15 seconds to run 95 tests.
  - **NEVER CANCEL**: Set timeout to 60+ seconds for tests.

- Run the demo application:
  ```bash
  dotnet run --project EasyCLI.Demo/EasyCLI.Demo.csproj
  ```
  - **Timing**: Runs in ~5 seconds. 
  - **Note**: Demo will be interactive if input is not redirected. Use `echo "" | dotnet run --project EasyCLI.Demo/EasyCLI.Demo.csproj` for non-interactive execution.

### Code Formatting (CRITICAL)
- **ALWAYS** run formatting before committing or CI will fail:
  ```bash
  dotnet format
  ```
- Verify formatting compliance:
  ```bash
  dotnet format --verify-no-changes
  ```
- **Timing**: Format check/fix takes ~14-15 seconds.

## Manual Validation Scenarios

**ALWAYS** run through these scenarios after making changes to ensure functionality:

### 1. Basic Library Functionality
```bash
# Build and run demo to verify ANSI styling
dotnet run --project EasyCLI.Demo/EasyCLI.Demo.csproj
```
Expected output: Styled console output with colors, tables, themes (Dark/Light/HighContrast), and formatted text.

### 2. PowerShell Cmdlet Integration
```bash
# Run cmdlet tests to verify PowerShell integration
dotnet test EasyCLI.Tests/EasyCLI.Tests.csproj -v minimal --filter "CmdletTests"
```
Expected: All 38 PowerShell cmdlet tests pass, verifying `Write-Message` cmdlet (alias `Show-Message`) works correctly.

### 3. ANSI Styling and Environment Variables
Test that color output respects environment variables:
```bash
# Test NO_COLOR support
NO_COLOR=1 dotnet run --project EasyCLI.Demo/EasyCLI.Demo.csproj
# Should show plain text without ANSI escape sequences
```

### 4. Interactive Prompts (Manual Testing Only)
```bash
# Test interactive prompts (requires TTY)
dotnet run --project EasyCLI.Demo/EasyCLI.Demo.csproj
# Manually interact with prompts for name, age, choices, etc.
```

## Project Structure and Navigation

### Key Directories
- `EasyCLI/` - Main class library source code
  - `ConsoleStyle.cs`, `ConsoleStyles.cs` - ANSI styling definitions
  - `ConsoleWriter.cs`, `ConsoleWriterExtensions.cs` - Main console output classes
  - `ConsoleThemes.cs` - Built-in color themes (Dark, Light, HighContrast)
  - `ConsoleFormatting.cs` - Advanced formatting helpers (tables, boxes, rules)
  - `Cmdlets/WriteMessageCommand.cs` - PowerShell cmdlet implementation (`Write-Message` with alias `Show-Message`)
  - `Prompts/` - Interactive prompt framework (string, int, choice, multi-select, hidden)
- `EasyCLI.Tests/` - Unit tests (95 tests covering ANSI behavior, cmdlets, prompts)
- `EasyCLI.Demo/` - Demonstration application showing all library features
- `.github/workflows/ci.yml` - CI pipeline (build, test, formatting validation)

### Frequently Modified Files
- When adding ANSI styles: `ConsoleStyles.cs`, `ConsoleThemes.cs`
- When adding console helpers: `ConsoleWriter.cs`, `ConsoleWriterExtensions.cs`
- When adding prompts: `Prompts/` directory
- When modifying PowerShell integration: `Cmdlets/` directory
- Always check `EasyCLI.Tests/` for corresponding test files

## Common Development Tasks

### Adding New Console Styles
1. Add style constants in `ConsoleStyles.cs`
2. Update theme definitions in `ConsoleThemes.cs` if needed
3. Add extension methods in `ConsoleWriterExtensions.cs`
4. Create tests in `EasyCLI.Tests/ConsoleWriterTests.cs`

### Adding New Interactive Prompts
1. Create prompt class in `Prompts/` inheriting from `BasePrompt<T>`
2. Add validation support if needed in `Prompts/Validators/`
3. Add tests in `EasyCLI.Tests/PromptTests.cs`
4. Update demo in `EasyCLI.Demo/Program.cs`

### Modifying PowerShell Cmdlets
1. Edit cmdlet in `Cmdlets/WriteMessageCommand.cs` or other cmdlet files
2. Update `EasyCLI.psd1` if adding new cmdlets
3. Always run cmdlet tests: `dotnet test --filter "CmdletTests"`

## Build Pipeline Validation
The CI pipeline (`.github/workflows/ci.yml`) runs:
1. Restore packages for all projects
2. Build library and demo in Release mode
3. Run all tests with minimal verbosity

Always run these locally before committing:
```bash
dotnet restore EasyCLI/EasyCLI.csproj --locked-mode
dotnet restore EasyCLI.Tests/EasyCLI.Tests.csproj --locked-mode  
dotnet restore EasyCLI.Demo/EasyCLI.Demo.csproj --locked-mode
dotnet build EasyCLI/EasyCLI.csproj --configuration Release --no-restore
dotnet build EasyCLI.Demo/EasyCLI.Demo.csproj --configuration Release --no-restore
dotnet test EasyCLI.Tests/EasyCLI.Tests.csproj --configuration Release --no-restore --verbosity minimal
dotnet format --verify-no-changes
```

**Quick validation pipeline** (all commands in ~18 seconds):
```bash
dotnet build && dotnet test --verbosity minimal && dotnet format --verify-no-changes
```

## Known Issues and Workarounds
- **Requires .NET 9.0**: Will fail with earlier .NET versions. Always install .NET 9.0 SDK first.
- **Formatting Required**: Code must pass `dotnet format --verify-no-changes` or CI fails.
- **Interactive Demo**: Demo application requires TTY for interactive prompts. Use input redirection for automated testing.
- **PowerShell Module**: Module loading tested through unit tests, not direct PowerShell import in build environment.