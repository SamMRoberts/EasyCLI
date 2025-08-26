# CLI Best Practices Guide for EasyCLI

This guide maps EasyCLI's capabilities to modern CLI best practices, organized by the three-tier framework of Required, Desired, and Recommended features.

## Overview

EasyCLI provides a comprehensive foundation for building professional-grade command-line tools that follow modern CLI conventions. This guide shows how to leverage EasyCLI's features to implement the complete spectrum of CLI best practices.

## Required Features (The Essentials)

These are non-negotiable features that every CLI must have. EasyCLI provides robust support for all of these.

### ✅ Clear Command Structure

**EasyCLI Support**: Shell framework with `ICliCommand` interface supports predictable verb-noun patterns.

```csharp
// Example: mycli deploy start
public class DeployCommand : ICliCommand
{
    public string Name => "deploy";
    public string Description => "Deploy application components";
    
    public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken ct)
    {
        if (args.Length == 0 || args[0] != "start")
        {
            context.Writer.WriteErrorLine("Usage: deploy start [options]");
            return Task.FromResult(1);
        }
        
        // Implement deployment logic
        context.Writer.WriteSuccessLine("Deployment started");
        return Task.FromResult(0);
    }
}
```

**Pattern**: Use consistent verb-noun structure (init, deploy, status, stop) and register commands with descriptive names.

### ✅ Help System

**EasyCLI Support**: Built-in help commands, rich error messages, and PowerShell cmdlet documentation.

```csharp
// Built-in shell help: automatically lists all registered commands
// "help" - shows all commands
// "help deploy" - shows specific command details

public class MyCommand : ICliCommand
{
    public string Description => "Deploy application with optional environment [staging|production]";
    
    public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken ct)
    {
        if (args.Contains("--help") || args.Contains("-h"))
        {
            ShowHelp(context);
            return Task.FromResult(0);
        }
        
        // Command logic
    }
    
    private void ShowHelp(ShellExecutionContext context)
    {
        context.Writer.WriteHeadingLine("Deploy Command", ConsoleThemes.Dark);
        context.Writer.WriteLine("Usage: deploy [environment] [options]");
        context.Writer.WriteLine();
        context.Writer.WriteInfoLine("Arguments:");
        context.Writer.WriteLine("  environment  Target environment (staging, production)");
        context.Writer.WriteLine();
        context.Writer.WriteInfoLine("Options:");
        context.Writer.WriteLine("  --dry-run    Show what would be deployed");
        context.Writer.WriteLine("  --verbose    Enable detailed output");
        context.Writer.WriteLine("  --help, -h   Show this help message");
    }
}
```

### ✅ Consistent Flags/Options

**EasyCLI Support**: Framework allows standardized flag parsing with consistent short/long forms.

```csharp
public class ConfigurableCommand : ICliCommand
{
    public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken ct)
    {
        var options = ParseOptions(args);
        
        if (options.Verbose)
        {
            context.Writer.WriteInfoLine("Verbose mode enabled");
        }
        
        // Use EasyCLI's styling for consistent output
        if (options.DryRun)
        {
            context.Writer.WriteWarningLine("[DRY RUN] Would execute command");
            return Task.FromResult(0);
        }
        
        return Task.FromResult(0);
    }
    
    private CommandOptions ParseOptions(string[] args)
    {
        var options = new CommandOptions();
        
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--verbose":
                case "-v":
                    options.Verbose = true;
                    break;
                case "--dry-run":
                case "-n":
                    options.DryRun = true;
                    break;
                case "--quiet":
                case "-q":
                    options.Quiet = true;
                    break;
            }
        }
        
        return options;
    }
}

public class CommandOptions
{
    public bool Verbose { get; set; }
    public bool DryRun { get; set; }
    public bool Quiet { get; set; }
}
```

### ✅ Exit Codes

**EasyCLI Support**: Shell commands return meaningful exit codes (0 = success, non-zero = error).

```csharp
public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken ct)
{
    try
    {
        // Success path
        context.Writer.WriteSuccessLine("Operation completed successfully");
        return Task.FromResult(0);
    }
    catch (FileNotFoundException)
    {
        context.Writer.WriteErrorLine("Configuration file not found");
        return Task.FromResult(2); // File not found
    }
    catch (UnauthorizedAccessException)
    {
        context.Writer.WriteErrorLine("Insufficient permissions");
        return Task.FromResult(3); // Permission denied
    }
    catch (Exception ex)
    {
        context.Writer.WriteErrorLine($"Unexpected error: {ex.Message}");
        return Task.FromResult(1); // General error
    }
}
```

### ✅ Input/Output Handling

**EasyCLI Support**: `ConsoleReader`/`ConsoleWriter` with stdin/stdout/stderr support.

```csharp
public async Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken ct)
{
    // Read from stdin if no file specified
    string input;
    if (args.Length == 0)
    {
        context.Writer.WriteInfoLine("Reading from stdin...", ConsoleThemes.Dark);
        input = await context.Reader.ReadLineAsync();
    }
    else
    {
        input = await File.ReadAllTextAsync(args[0], ct);
    }
    
    // Process and write to stdout
    var result = ProcessData(input);
    context.Writer.WriteLine(result);
    
    // Errors go to stderr (automatically handled by ConsoleWriter)
    if (string.IsNullOrEmpty(result))
    {
        context.Writer.WriteErrorLine("No data to process");
        return 1;
    }
    
    return 0;
}
```

### ✅ Cross-Platform Compatibility

**EasyCLI Support**: .NET 9.0 provides excellent cross-platform support, and EasyCLI respects platform conventions.

```csharp
// EasyCLI automatically handles:
// - Path separators (/ vs \)
// - Console capabilities detection
// - Terminal color support
// - Environment variable conventions (NO_COLOR, FORCE_COLOR)

public class CrossPlatformCommand : ICliCommand
{
    public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken ct)
    {
        // Use Path.Combine for cross-platform paths
        var configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".myapp", "config.json");
        
        // EasyCLI handles color detection automatically
        context.Writer.WriteSuccessLine($"Config location: {configPath}");
        
        return Task.FromResult(0);
    }
}
```

## Desired Features (Productivity Boosters)

These features make CLIs pleasant to use. EasyCLI provides excellent foundation support.

### ✅ Colorized Output

**EasyCLI Support**: Comprehensive ANSI styling with built-in themes and environment respect.

```csharp
public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken ct)
{
    var theme = ConsoleThemes.Dark; // or Light, HighContrast
    
    // Status indicators
    context.Writer.WriteSuccessLine("✓ Database connection established", theme);
    context.Writer.WriteWarningLine("⚠ Cache is 90% full", theme);
    context.Writer.WriteErrorLine("✗ Service unavailable", theme);
    
    // Structured output
    context.Writer.WriteHeadingLine("System Status", theme);
    context.Writer.WriteKeyValues(new[]
    {
        ("CPU Usage", "45%"),
        ("Memory", "2.1GB / 8GB"),
        ("Disk Space", "120GB available")
    }, keyStyle: theme.Info);
    
    return Task.FromResult(0);
}
```

### 🔶 Autocomplete

**EasyCLI Support**: Basic completion via `ICliCommand.GetCompletions()` method.

```csharp
public class FileCommand : ICliCommand
{
    public string[] GetCompletions(ShellExecutionContext context, string prefix)
    {
        // Provide file/directory completions
        if (Directory.Exists(prefix))
        {
            return Directory.GetDirectories(prefix)
                .Concat(Directory.GetFiles(prefix))
                .Select(Path.GetFileName)
                .Where(name => name.StartsWith(Path.GetFileName(prefix)))
                .ToArray();
        }
        
        // Provide command-specific completions
        return new[] { "list", "create", "delete", "rename" }
            .Where(cmd => cmd.StartsWith(prefix))
            .ToArray();
    }
}
```

**Enhancement Needed**: Consider implementing more sophisticated tab completion with system integration.

### 🔶 Config Management

**Implementation Pattern**: Use EasyCLI's input/output capabilities with JSON/YAML config files.

```csharp
public class ConfigManager
{
    private readonly string _globalConfigPath;
    private readonly string _localConfigPath;
    
    public ConfigManager()
    {
        _globalConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".myapp", "config.json");
        _localConfigPath = Path.Combine(Directory.GetCurrentDirectory(), ".myapp.json");
    }
    
    public async Task<Config> LoadConfigAsync(IConsoleWriter writer)
    {
        var config = new Config();
        
        // Load global config
        if (File.Exists(_globalConfigPath))
        {
            writer.WriteHintLine($"Loading global config from {_globalConfigPath}");
            var globalJson = await File.ReadAllTextAsync(_globalConfigPath);
            var globalConfig = JsonSerializer.Deserialize<Config>(globalJson);
            config = MergeConfigs(config, globalConfig);
        }
        
        // Load local config (overrides global)
        if (File.Exists(_localConfigPath))
        {
            writer.WriteHintLine($"Loading local config from {_localConfigPath}");
            var localJson = await File.ReadAllTextAsync(_localConfigPath);
            var localConfig = JsonSerializer.Deserialize<Config>(localJson);
            config = MergeConfigs(config, localConfig);
        }
        
        return config;
    }
}

public class ConfigCommand : ICliCommand
{
    public string Name => "config";
    public string Description => "Manage application configuration";
    
    public async Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken ct)
    {
        var configManager = new ConfigManager();
        
        if (args.Length == 0)
        {
            // Show current config
            var config = await configManager.LoadConfigAsync(context.Writer);
            context.Writer.WriteTableSimple(
                new[] { "Setting", "Value", "Source" },
                new[]
                {
                    new[] { "api_url", config.ApiUrl, config.ApiUrlSource },
                    new[] { "timeout", config.Timeout.ToString(), config.TimeoutSource }
                },
                headerStyle: ConsoleThemes.Dark.Heading
            );
            return 0;
        }
        
        // Handle config set/get operations
        return 0;
    }
}
```

### ✅ Subcommands & Aliases

**EasyCLI Support**: Shell framework naturally supports this pattern.

```csharp
public class GitLikeCommand : ICliCommand
{
    public string Name => "git";
    public string Description => "Git-like command with subcommands";
    
    public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken ct)
    {
        if (args.Length == 0)
        {
            ShowHelp(context);
            return Task.FromResult(1);
        }
        
        var subcommand = args[0];
        var subArgs = args.Skip(1).ToArray();
        
        return subcommand switch
        {
            "status" or "st" => ShowStatus(context, subArgs),
            "commit" or "ci" => Commit(context, subArgs),
            "push" => Push(context, subArgs),
            "pull" => Pull(context, subArgs),
            _ => UnknownSubcommand(context, subcommand)
        };
    }
    
    private Task<int> ShowStatus(ShellExecutionContext context, string[] args)
    {
        context.Writer.WriteHeadingLine("Repository Status", ConsoleThemes.Dark);
        context.Writer.WriteSuccessLine("✓ Working directory clean");
        return Task.FromResult(0);
    }
}
```

### 🔶 Logging & Verbosity Levels

**Implementation Pattern**: Use EasyCLI's styling with verbosity control.

```csharp
public enum LogLevel { Quiet, Normal, Verbose, Debug }

public class Logger
{
    private readonly IConsoleWriter _writer;
    private readonly LogLevel _level;
    
    public Logger(IConsoleWriter writer, LogLevel level)
    {
        _writer = writer;
        _level = level;
    }
    
    public void LogDebug(string message)
    {
        if (_level >= LogLevel.Debug)
            _writer.WriteHintLine($"[DEBUG] {message}", ConsoleThemes.Dark);
    }
    
    public void LogVerbose(string message)
    {
        if (_level >= LogLevel.Verbose)
            _writer.WriteInfoLine($"[VERBOSE] {message}", ConsoleThemes.Dark);
    }
    
    public void LogNormal(string message)
    {
        if (_level >= LogLevel.Normal)
            _writer.WriteLine(message);
    }
    
    public void LogWarning(string message)
    {
        if (_level >= LogLevel.Normal)
            _writer.WriteWarningLine($"Warning: {message}", ConsoleThemes.Dark);
    }
    
    public void LogError(string message)
    {
        _writer.WriteErrorLine($"Error: {message}", ConsoleThemes.Dark);
    }
}

public class VerboseCommand : ICliCommand
{
    public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken ct)
    {
        var logLevel = DetermineLogLevel(args);
        var logger = new Logger(context.Writer, logLevel);
        
        logger.LogDebug("Starting command execution");
        logger.LogVerbose("Processing configuration");
        logger.LogNormal("Operation completed");
        
        return Task.FromResult(0);
    }
    
    private LogLevel DetermineLogLevel(string[] args)
    {
        if (args.Contains("--quiet") || args.Contains("-q")) return LogLevel.Quiet;
        if (args.Contains("--debug")) return LogLevel.Debug;
        if (args.Contains("--verbose") || args.Contains("-v")) return LogLevel.Verbose;
        return LogLevel.Normal;
    }
}
```

### 🔶 Dry-Run Mode

**Implementation Pattern**: Standard flag parsing with EasyCLI's warning styling.

```csharp
public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken ct)
{
    var dryRun = args.Contains("--dry-run") || args.Contains("-n");
    
    if (dryRun)
    {
        context.Writer.WriteWarningLine("[DRY RUN] No changes will be made", ConsoleThemes.Dark);
        context.Writer.WriteLine();
    }
    
    // Show what would happen
    context.Writer.WriteInfoLine($"Would delete 5 files", ConsoleThemes.Dark);
    context.Writer.WriteInfoLine($"Would create directory: ./output", ConsoleThemes.Dark);
    
    if (dryRun)
    {
        context.Writer.WriteLine();
        context.Writer.WriteHintLine("Run without --dry-run to execute these changes", ConsoleThemes.Dark);
        return Task.FromResult(0);
    }
    
    // Actually execute
    context.Writer.WriteSuccessLine("Changes applied successfully", ConsoleThemes.Dark);
    return Task.FromResult(0);
}
```

### ✅ Cross-Environment Awareness

**EasyCLI Support**: Respects NO_COLOR, FORCE_COLOR, and other environment conventions.

```csharp
public class EnvironmentAwareCommand : ICliCommand
{
    public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken ct)
    {
        // EasyCLI automatically respects NO_COLOR, FORCE_COLOR
        // Your command can also check environment-specific settings
        
        var environment = Environment.GetEnvironmentVariable("APP_ENV") ?? "development";
        var apiUrl = Environment.GetEnvironmentVariable("API_URL") ?? "http://localhost:3000";
        
        context.Writer.WriteKeyValues(new[]
        {
            ("Environment", environment),
            ("API URL", apiUrl),
            ("Colors Enabled", context.Writer.SupportsColor.ToString())
        }, keyStyle: ConsoleThemes.Dark.Info);
        
        return Task.FromResult(0);
    }
}
```

## Recommended Features (The "Chef's Kiss")

These features elevate your CLI into a well-designed developer tool.

### ✅ Interactive Mode

**EasyCLI Support**: Comprehensive prompt framework for guided user interaction.

```csharp
public class InteractiveSetupCommand : ICliCommand
{
    public async Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken ct)
    {
        context.Writer.WriteHeadingLine("Interactive Setup", ConsoleThemes.Dark);
        
        // String input with validation
        var namePrompt = new StringPrompt("Project name", context.Writer, context.Reader,
            validator: new RequiredValidator());
        var projectName = namePrompt.GetValue();
        
        // Choice selection
        var frameworkChoices = new[]
        {
            new Choice<string>("ASP.NET Core", "aspnet"),
            new Choice<string>("Console App", "console"),
            new Choice<string>("Worker Service", "worker")
        };
        var frameworkPrompt = new ChoicePrompt<string>("Framework", frameworkChoices, 
            context.Writer, context.Reader);
        var framework = frameworkPrompt.GetValue();
        
        // Yes/No confirmation
        var confirmPrompt = new YesNoPrompt("Create project now?", context.Writer, context.Reader, 
            defaultValue: true);
        var shouldCreate = confirmPrompt.GetValue();
        
        if (shouldCreate)
        {
            context.Writer.WriteSuccessLine($"Creating {framework} project: {projectName}");
            return 0;
        }
        
        context.Writer.WriteHintLine("Setup cancelled");
        return 1;
    }
}
```

### ✅ Rich Output Formats

**EasyCLI Support**: Tables, boxes, rules, and structured formatting.

```csharp
public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken ct)
{
    var format = args.Contains("--json") ? "json" : 
                 args.Contains("--yaml") ? "yaml" : "table";
    
    var data = GetSystemInfo();
    
    switch (format)
    {
        case "json":
            context.Writer.WriteLine(JsonSerializer.Serialize(data, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            }));
            break;
            
        case "yaml":
            // Use YamlDotNet or similar
            context.Writer.WriteLine(ConvertToYaml(data));
            break;
            
        default:
            // Use EasyCLI's table formatting
            context.Writer.WriteTableSimple(
                new[] { "Property", "Value" },
                data.Select(kv => new[] { kv.Key, kv.Value }).ToArray(),
                headerStyle: ConsoleThemes.Dark.Heading,
                borderStyle: ConsoleThemes.Dark.Hint
            );
            break;
    }
    
    return Task.FromResult(0);
}
```

### ✅ Progress Indicators

**Implementation Pattern**: Use EasyCLI's styling for progress feedback.

```csharp
public async Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken ct)
{
    var steps = new[] { "Validating", "Processing", "Uploading", "Finalizing" };
    
    for (int i = 0; i < steps.Length; i++)
    {
        var percentage = (i + 1) * 100 / steps.Length;
        var progressBar = new string('█', percentage / 5) + new string('░', 20 - percentage / 5);
        
        context.Writer.Write($"\r{steps[i]}... [{progressBar}] {percentage}%");
        
        // Simulate work
        await Task.Delay(1000, ct);
    }
    
    context.Writer.WriteLine();
    context.Writer.WriteSuccessLine("✓ All steps completed successfully");
    
    return 0;
}
```

### 🔶 Smart Error Recovery

**Implementation Pattern**: Use EasyCLI's styling with helpful suggestions.

```csharp
public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken ct)
{
    if (args.Length == 0)
    {
        context.Writer.WriteErrorLine("No command specified");
        context.Writer.WriteLine();
        context.Writer.WriteHintLine("Did you mean one of these?");
        
        var suggestions = new[] { "deploy", "status", "config" };
        foreach (var suggestion in suggestions)
        {
            context.Writer.WriteLine($"  {suggestion}");
        }
        
        return Task.FromResult(1);
    }
    
    var command = args[0];
    if (!IsValidCommand(command))
    {
        context.Writer.WriteErrorLine($"Unknown command: {command}");
        
        var closestMatch = FindClosestMatch(command, GetValidCommands());
        if (closestMatch != null)
        {
            context.Writer.WriteHintLine($"Did you mean '{closestMatch}'?");
        }
        
        return Task.FromResult(1);
    }
    
    return Task.FromResult(0);
}

private string? FindClosestMatch(string input, string[] options)
{
    return options
        .Where(opt => LevenshteinDistance(input, opt) <= 2)
        .OrderBy(opt => LevenshteinDistance(input, opt))
        .FirstOrDefault();
}
```

### ✅ Pluggable Architecture

**EasyCLI Support**: `ICliCommand` interface enables clean plugin architecture.

```csharp
public class PluginManager
{
    private readonly CliShell _shell;
    
    public PluginManager(CliShell shell)
    {
        _shell = shell;
    }
    
    public async Task LoadPluginsAsync(string pluginsDirectory)
    {
        if (!Directory.Exists(pluginsDirectory)) return;
        
        foreach (var dllFile in Directory.GetFiles(pluginsDirectory, "*.dll"))
        {
            try
            {
                var assembly = Assembly.LoadFrom(dllFile);
                var commandTypes = assembly.GetTypes()
                    .Where(t => typeof(ICliCommand).IsAssignableFrom(t) && !t.IsAbstract);
                
                foreach (var commandType in commandTypes)
                {
                    if (Activator.CreateInstance(commandType) is ICliCommand command)
                    {
                        await _shell.RegisterAsync(command);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log plugin loading error
                Console.WriteLine($"Failed to load plugin {dllFile}: {ex.Message}");
            }
        }
    }
}
```

### 🔶 Context Awareness

**Implementation Pattern**: Detect and adapt to environment context.

```csharp
public class ContextAwareCommand : ICliCommand
{
    public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken ct)
    {
        var contextInfo = DetectContext();
        
        // Adapt behavior based on context
        if (contextInfo.IsGitRepository)
        {
            context.Writer.WriteInfoLine($"Git branch: {contextInfo.GitBranch}");
        }
        
        if (contextInfo.IsDockerEnvironment)
        {
            context.Writer.WriteInfoLine("Running in Docker container");
        }
        
        if (contextInfo.HasConfigFile)
        {
            context.Writer.WriteInfoLine($"Using config: {contextInfo.ConfigFile}");
        }
        
        return Task.FromResult(0);
    }
    
    private ContextInfo DetectContext()
    {
        return new ContextInfo
        {
            IsGitRepository = Directory.Exists(".git"),
            GitBranch = GetGitBranch(),
            IsDockerEnvironment = Environment.GetEnvironmentVariable("DOCKER_CONTAINER") != null,
            HasConfigFile = File.Exists("app.config.json"),
            ConfigFile = FindConfigFile()
        };
    }
}
```

### 🔶 Secure Credentials Handling

**EasyCLI Support**: Hidden input prompts for secure credential collection.

```csharp
public class LoginCommand : ICliCommand
{
    public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken ct)
    {
        // Use EasyCLI's hidden prompt for passwords
        var usernamePrompt = new StringPrompt("Username", context.Writer, context.Reader);
        var passwordPrompt = new HiddenPrompt("Password", context.Writer, context.Reader);
        
        var username = usernamePrompt.GetValue();
        var password = passwordPrompt.GetValue();
        
        // Store securely (never in plain text)
        var success = AuthenticateAndStoreToken(username, password);
        
        if (success)
        {
            context.Writer.WriteSuccessLine("Authentication successful");
            context.Writer.WriteHintLine("Token stored in system keychain");
            return Task.FromResult(0);
        }
        
        context.Writer.WriteErrorLine("Authentication failed");
        return Task.FromResult(1);
    }
    
    private bool AuthenticateAndStoreToken(string username, string password)
    {
        // Implement secure authentication and token storage
        // Consider using system keychain/credential manager
        return true;
    }
}
```

### ✅ Batch & Automation Friendly

**EasyCLI Support**: Proper exit codes, environment awareness, and scriptable output.

```csharp
public class AutomationFriendlyCommand : ICliCommand
{
    public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken ct)
    {
        var isAutomated = !context.Reader.IsInteractive || 
                         Environment.GetEnvironmentVariable("CI") != null;
        
        if (isAutomated)
        {
            // Machine-readable output
            var result = new
            {
                status = "success",
                timestamp = DateTime.UtcNow,
                data = GetData()
            };
            
            context.Writer.WriteLine(JsonSerializer.Serialize(result));
        }
        else
        {
            // Human-readable output
            context.Writer.WriteSuccessLine("Operation completed successfully");
            context.Writer.WriteKeyValues(GetData().Select(kv => (kv.Key, kv.Value)).ToArray());
        }
        
        return Task.FromResult(0);
    }
}
```

## Implementation Checklist

When building a CLI with EasyCLI, ensure you implement:

**Required (Must Have)**
- [ ] Consistent command naming (verb-noun patterns)
- [ ] Help text for all commands (`--help` flag)
- [ ] Proper exit codes (0 = success, >0 = error)
- [ ] Stdin/stdout/stderr handling via ConsoleReader/Writer
- [ ] Cross-platform path handling

**Desired (Should Have)**
- [ ] Color themes using ConsoleThemes
- [ ] Interactive prompts for complex input
- [ ] Command aliases and shortcuts
- [ ] Verbosity levels (--quiet, --verbose)
- [ ] Configuration file support
- [ ] Dry-run mode for destructive operations

**Recommended (Nice to Have)**  
- [ ] Tab completion implementation
- [ ] Progress indicators for long operations
- [ ] Multiple output formats (JSON, YAML, table)
- [ ] Plugin architecture for extensibility
- [ ] Context detection (git repo, Docker, etc.)
- [ ] Smart error suggestions
- [ ] Secure credential handling

## Quick Start Template

```csharp
using EasyCLI.Console;
using EasyCLI.Shell;
using EasyCLI.Styling;

// Create a professional CLI application
var reader = new ConsoleReader();
var writer = new ConsoleWriter();
var shell = new CliShell(reader, writer, new ShellOptions
{
    Prompt = "mycli>",
    PromptStyle = ConsoleThemes.Dark.Info
});

// Register your commands
await shell.RegisterAsync(new DeployCommand());
await shell.RegisterAsync(new ConfigCommand());
await shell.RegisterAsync(new StatusCommand());

// Start interactive shell
await shell.RunAsync();
```

This template provides a foundation that follows all the CLI best practices outlined in this guide.