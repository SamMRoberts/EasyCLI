# CLI Enhancement Features

EasyCLI v0.2.0+ introduces professional CLI best practices and enterprise-ready features for building production-grade command-line applications.

## Enhanced Command Framework

The `EnhancedCliCommand` base class provides professional CLI features with minimal boilerplate code.

### Basic Enhanced Command

```csharp
using EasyCLI.Shell;

public class DeployCommand : EnhancedCliCommand
{
    public override string Name => "deploy";
    public override string Description => "Deploy application to specified environment";
    public override string Category => "Deployment"; // Optional categorization

    protected override void ConfigureHelp(CommandHelp help)
    {
        help.Usage = "deploy [options] <environment>";
        help.Description = "Deploys the application with optional configuration.";

        // Arguments
        help.Arguments.Add(new CommandArgument("environment", 
            "Target environment (dev, staging, production)", required: true));

        // Options
        help.Options.Add(new CommandOption("dry-run", "n", 
            "Show what would be deployed without executing"));
        help.Options.Add(new CommandOption("verbose", "v", 
            "Enable verbose output"));
        help.Options.Add(new CommandOption("config", "c", 
            "Custom configuration file path", hasValue: true));
        help.Options.Add(new CommandOption("timeout", "t", 
            "Deployment timeout in seconds", hasValue: true));

        // Examples
        help.Examples.Add(new CommandExample(
            "deploy staging --dry-run",
            "Preview staging deployment"));
        help.Examples.Add(new CommandExample(
            "deploy production --config prod.json --verbose",
            "Deploy to production with custom config"));
    }

    protected override async Task<int> ExecuteCommand(CommandLineArgs args, 
        ShellExecutionContext context, CancellationToken cancellationToken)
    {
        // Enhanced commands automatically provide:
        // - Logger with appropriate verbosity
        // - ConfigManager for configuration
        // - Environment detection
        // - Proper error handling and exit codes

        var environment = args.Arguments[0];
        var configFile = args.GetOption("config");
        var timeout = args.GetOptionAsInt("timeout", 300);

        if (args.IsDryRun)
        {
            Logger?.LogWarning("[DRY RUN] No changes will be made");
            Logger?.LogInfo($"Would deploy to: {environment}");
            Logger?.LogInfo($"Would use config: {configFile ?? "default"}");
            Logger?.LogInfo($"Would timeout after: {timeout}s");
            return ExitCodes.Success;
        }

        Logger?.LogInfo($"Deploying to {environment}...");
        
        // Actual deployment logic here
        await DeployToEnvironment(environment, configFile, timeout, cancellationToken);
        
        Logger?.LogSuccess("Deployment completed successfully!");
        return ExitCodes.Success;
    }

    private async Task DeployToEnvironment(string environment, string? configFile, 
        int timeout, CancellationToken cancellationToken)
    {
        // Implementation details...
    }
}
```

### Standard CLI Flags

Enhanced commands automatically support standard CLI flags:

```bash
# Standard flags supported out of the box
mycli deploy --help          # Show help
mycli deploy --verbose       # Enable verbose logging  
mycli deploy --quiet         # Suppress output
mycli deploy --debug         # Enable debug logging
mycli deploy --dry-run       # Preview mode
mycli deploy --plain         # Plain text output (no colors/styling)
mycli deploy --yes           # Skip confirmations
mycli deploy --force         # Override safety checks
```

## Configuration Management

Hierarchical JSON configuration system with global and local support.

### Configuration Structure

```csharp
public class AppConfig
{
    public string ApiUrl { get; set; } = "https://api.example.com";
    public int Timeout { get; set; } = 30;
    public bool EnableLogging { get; set; } = true;
    public Dictionary<string, string> Environments { get; set; } = new();
}
```

### Using Configuration in Commands

```csharp
protected override async Task<int> ExecuteCommand(CommandLineArgs args, 
    ShellExecutionContext context, CancellationToken cancellationToken)
{
    // Config automatically loaded from:
    // 1. Global: ~/.myapp/config.json
    // 2. Local: ./.myapp.json
    // 3. Command line options override config values

    var apiUrl = Config?.ApiUrl ?? "https://api.example.com";
    var timeout = Config?.Timeout ?? 30;

    Logger?.LogVerbose($"Using API URL: {apiUrl}");
    Logger?.LogVerbose($"Using timeout: {timeout}s");

    // Override from command line
    if (args.HasOption("api-url"))
    {
        apiUrl = args.GetOption("api-url")!;
        Logger?.LogVerbose($"API URL overridden: {apiUrl}");
    }

    return ExitCodes.Success;
}
```

### Configuration Management

```csharp
// Save global configuration
await ConfigManager.SaveConfigAsync(new AppConfig 
{ 
    ApiUrl = "https://prod-api.example.com",
    Timeout = 60,
    EnableLogging = false
}, global: true);

// Save local project configuration
await ConfigManager.SaveConfigAsync(new AppConfig 
{ 
    ApiUrl = "https://dev-api.example.com",
    Environments = new Dictionary<string, string>
    {
        ["dev"] = "development",
        ["test"] = "testing",
        ["prod"] = "production"
    }
}, global: false);

// Load configuration with sources
var config = await ConfigManager.LoadConfigAsync<AppConfig>();
Logger?.LogInfo($"API URL from: {config.Source?.ApiUrlSource}");
```

## Environment Detection

Automatic detection of Git repositories, Docker environments, CI/CD systems, and platform information.

### Environment Information

```csharp
protected override async Task<int> ExecuteCommand(CommandLineArgs args, 
    ShellExecutionContext context, CancellationToken cancellationToken)
{
    // Environment automatically detected and available
    Logger?.LogVerbose($"Platform: {Environment?.Platform}");
    Logger?.LogVerbose($"Interactive: {Environment?.IsInteractive}");
    Logger?.LogVerbose($"CI Environment: {Environment?.IsContinuousIntegration}");
    Logger?.LogVerbose($"CI Provider: {Environment?.CiProvider}");
    Logger?.LogVerbose($"Git Repository: {Environment?.IsGitRepository}");
    Logger?.LogVerbose($"Git Branch: {Environment?.GitBranch}");
    Logger?.LogVerbose($"Docker: {Environment?.IsDockerEnvironment}");

    // Adapt behavior based on environment
    if (Environment?.IsContinuousIntegration == true)
    {
        // Non-interactive mode for CI/CD
        Logger?.LogInfo("CI environment detected - using non-interactive mode");
        return await ExecuteAutomated(args, cancellationToken);
    }

    if (Environment?.IsGitRepository == true)
    {
        Logger?.LogInfo($"Git repository detected on branch: {Environment.GitBranch}");
        // Git-specific features available
    }

    return ExitCodes.Success;
}
```

### Environment-Aware Behavior

```csharp
// Different behavior based on environment
if (Environment?.IsDockerEnvironment == true)
{
    // Adjust for container environment
    Logger?.LogInfo("Docker environment detected");
    // Use container-specific paths and permissions
}

if (Environment?.Platform == Platform.Windows)
{
    // Windows-specific behavior
    Logger?.LogVerbose("Using Windows-specific configuration");
}
```

## Structured Logging

Professional logging framework with verbosity levels and automatic CLI flag integration.

### Logging Levels

```csharp
protected override async Task<int> ExecuteCommand(CommandLineArgs args, 
    ShellExecutionContext context, CancellationToken cancellationToken)
{
    // Logger automatically configured based on CLI flags:
    // --quiet: Only errors
    // --verbose: Info, warnings, errors  
    // --debug: Everything including debug messages
    // Default: Normal level (warnings and errors)

    Logger?.LogDebug("Starting command execution");
    Logger?.LogVerbose("Loading configuration files");
    Logger?.LogInfo("Processing request...");
    Logger?.LogSuccess("Operation completed successfully");
    Logger?.LogWarning("Cache is 90% full");
    Logger?.LogError("Failed to connect to service");

    return ExitCodes.Success;
}
```

### Custom Logging

```csharp
// Logger with custom configuration
var logger = new Logger(LogLevel.Verbose, context.Writer);

// Conditional logging
if (Logger?.IsVerboseEnabled == true)
{
    Logger.LogVerbose("Detailed processing information");
}

// Structured logging with context
Logger?.LogInfo("Processing file", new { 
    FileName = "data.json", 
    Size = "1.2MB",
    Type = "JSON" 
});
```

## Dangerous Operation Confirmation

Safety framework for destructive operations with automation detection.

### Confirmation Prompts

```csharp
public class DeleteCommand : EnhancedCliCommand
{
    public override string Name => "delete";
    public override string Description => "Delete files or directories";

    protected override async Task<int> ExecuteCommand(CommandLineArgs args, 
        ShellExecutionContext context, CancellationToken cancellationToken)
    {
        var path = args.Arguments[0];
        
        // Automatic confirmation for dangerous operations
        if (!await ConfirmDangerousOperation(
            $"Delete '{path}' and all its contents?",
            "This action cannot be undone.",
            args, context))
        {
            Logger?.LogInfo("Operation cancelled");
            return ExitCodes.UserCancelled;
        }

        // Perform deletion
        Directory.Delete(path, recursive: true);
        Logger?.LogSuccess($"Deleted: {path}");
        
        return ExitCodes.Success;
    }
}
```

### Automation Detection

```csharp
// Automatically handles:
// - Interactive mode: Shows confirmation prompt
// - CI/CD mode: Requires --yes or --force flags
// - Non-TTY mode: Requires explicit confirmation flags

if (args.HasFlag("yes") || args.HasFlag("force"))
{
    // Skip confirmation
}
else if (!Environment?.IsInteractive == true)
{
    Logger?.LogError("Cannot confirm dangerous operation in non-interactive mode");
    Logger?.LogHint("Use --yes or --force to confirm in automated environments");
    return ExitCodes.InvalidArguments;
}
```

## Dry-Run Support

Built-in dry-run mode for safe operation previews.

### Implementing Dry-Run

```csharp
protected override async Task<int> ExecuteCommand(CommandLineArgs args, 
    ShellExecutionContext context, CancellationToken cancellationToken)
{
    var files = GetFilesToProcess(args.Arguments[0]);
    
    if (args.IsDryRun)
    {
        Logger?.LogWarning("[DRY RUN] No changes will be made");
        
        // Preview what would happen
        foreach (var file in files)
        {
            Logger?.LogInfo($"Would process: {file}");
        }
        
        Logger?.LogInfo($"Would process {files.Count} files total");
        Logger?.LogHint("Run without --dry-run to execute these changes");
        
        return ExitCodes.Success;
    }

    // Actually execute
    foreach (var file in files)
    {
        await ProcessFile(file, cancellationToken);
        Logger?.LogVerbose($"Processed: {file}");
    }
    
    Logger?.LogSuccess($"Processed {files.Count} files successfully");
    return ExitCodes.Success;
}
```

## Smart Error Handling

Intelligent error suggestions and recovery with proper exit codes.

### Exit Codes

```csharp
public static class ExitCodes
{
    public const int Success = 0;
    public const int GeneralError = 1;
    public const int FileNotFound = 2;
    public const int PermissionDenied = 3;
    public const int InvalidArguments = 4;
    public const int ServiceUnavailable = 5;
    public const int UserCancelled = 130;
}
```

### Error Recovery

```csharp
protected override async Task<int> ExecuteCommand(CommandLineArgs args, 
    ShellExecutionContext context, CancellationToken cancellationToken)
{
    try
    {
        await DoSomething();
        return ExitCodes.Success;
    }
    catch (FileNotFoundException ex)
    {
        Logger?.LogError($"File not found: {ex.FileName}");
        ShowSuggestion(context, "Make sure the file exists and you have read permissions");
        
        // Suggest similar files
        var suggestions = FindSimilarFiles(ex.FileName);
        if (suggestions.Any())
        {
            Logger?.LogHint("Did you mean one of these?");
            foreach (var suggestion in suggestions)
            {
                Logger?.LogHint($"  {suggestion}");
            }
        }
        
        return ExitCodes.FileNotFound;
    }
    catch (UnauthorizedAccessException)
    {
        Logger?.LogError("Permission denied");
        ShowSuggestion(context, "Try running with elevated permissions or check file ownership");
        return ExitCodes.PermissionDenied;
    }
    catch (HttpRequestException ex) when (ex.Message.Contains("timeout"))
    {
        Logger?.LogError("Service request timed out");
        ShowSuggestion(context, "Check your network connection and try again");
        ShowSuggestion(context, "Use --timeout to increase the timeout value");
        return ExitCodes.ServiceUnavailable;
    }
}
```

### Smart Suggestions

```csharp
// Command suggestion for typos
if (!IsValidCommand(commandName))
{
    Logger?.LogError($"Unknown command: {commandName}");
    
    var suggestions = GetCommandSuggestions(commandName);
    if (suggestions.Any())
    {
        Logger?.LogHint("Did you mean:");
        foreach (var suggestion in suggestions)
        {
            Logger?.LogHint($"  {suggestion}");
        }
    }
    
    return ExitCodes.InvalidArguments;
}
```

## Professional CLI Patterns

### Help System

```csharp
// Enhanced help with categorization
help all              # Show all commands by category
help deploy           # Show specific command help
help --examples       # Show example usage patterns
```

### Standard Argument Patterns

```csharp
// Consistent flag patterns across all enhanced commands
--help, -h           # Show help
--verbose, -v        # Increase output detail  
--quiet, -q          # Reduce output
--debug              # Enable debug output
--dry-run, -n        # Preview mode
--yes, -y            # Skip confirmations
--force, -f          # Override safety checks
--plain, -p          # Plain text output
--json, -j           # JSON output format
--config, -c FILE    # Specify config file
--timeout, -t SECS   # Specify timeout
```

### Output Consistency

```csharp
// Consistent output patterns
Logger?.LogSuccess("✓ Operation completed successfully");
Logger?.LogWarning("⚠ Cache is 90% full");  
Logger?.LogError("✗ Service unavailable");
Logger?.LogInfo("ℹ Processing 142 files...");
Logger?.LogHint("💡 Use --verbose for detailed output");
```

## Best Practices

### Command Design

```csharp
// Follow verb-noun pattern
public class UserCreateCommand : EnhancedCliCommand { }
public class UserDeleteCommand : EnhancedCliCommand { }
public class ConfigShowCommand : EnhancedCliCommand { }
public class ConfigSetCommand : EnhancedCliCommand { }
```

### Error Messages

```csharp
// Provide actionable error messages
Logger?.LogError("Configuration file not found");
Logger?.LogHint("Run 'config init' to create a default configuration");

// Include context in error messages
Logger?.LogError($"Failed to connect to {apiUrl}");
Logger?.LogHint("Check your network connection and API URL configuration");
```

### Progressive Disclosure

```csharp
// Show basic help by default
if (args.Arguments.Count == 0)
{
    ShowConciseHelp(context);
    return ExitCodes.InvalidArguments;
}

// Detailed help only when requested
if (args.HasFlag("help"))
{
    ShowHelp(context);
    return ExitCodes.Success;
}
```

## Next Steps

- **[Shell Framework](Shell-Framework)** - Building interactive shells and command registration
- **[Output and Scripting](Output-and-Scripting)** - Automation-friendly output formats
- **[Examples and Tutorials](Examples-and-Tutorials)** - Real-world usage examples