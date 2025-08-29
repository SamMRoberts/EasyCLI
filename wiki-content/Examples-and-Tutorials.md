# Examples and Tutorials

This page provides practical examples and step-by-step tutorials for building applications with EasyCLI.

## Tutorial 1: Basic Console Application

Let's build a simple file processing tool that demonstrates core EasyCLI features.

### Step 1: Project Setup

```bash
# Create new console application
dotnet new console -n FileProcessor
cd FileProcessor

# Add EasyCLI package
dotnet add package SamMRoberts.EasyCLI
```

### Step 2: Basic Styling

```csharp
using EasyCLI.Console;

var writer = new ConsoleWriter();
var theme = ConsoleThemes.Dark;

// Welcome message
writer.WriteTitledBox("File Processor v1.0", new[]
{
    "A simple file processing tool",
    "Built with EasyCLI"
}, theme);

// Show status
writer.WriteSuccessLine("✓ Application started", theme);
writer.WriteInfoLine("Ready to process files", theme);

// Display configuration
writer.WriteKeyValues(new[]
{
    ("Version", "1.0.0"),
    ("Mode", "Development"),
    ("Max Files", "100")
}, keyStyle: theme.Info);
```

### Step 3: Interactive Input

```csharp
using EasyCLI.Prompts;

var reader = new ConsoleReader();

// Get input directory
var dirPrompt = new StringPrompt("Input directory", writer, reader,
    defaultValue: "./input");
var inputDir = dirPrompt.GetValue();

// Get file type
var typeChoices = new[]
{
    new Choice<string>("Text Files", "*.txt"),
    new Choice<string>("JSON Files", "*.json"),
    new Choice<string>("All Files", "*.*")
};
var typePrompt = new ChoicePrompt<string>("File type", typeChoices, writer, reader);
var filePattern = typePrompt.GetValue();

// Confirm processing
var confirmPrompt = new YesNoPrompt($"Process {filePattern} files in {inputDir}?", 
    writer, reader, defaultValue: true);
var shouldProcess = confirmPrompt.GetValue();

if (!shouldProcess)
{
    writer.WriteWarningLine("Processing cancelled", theme);
    return;
}
```

### Step 4: File Processing with Progress

```csharp
// Find files
var files = Directory.GetFiles(inputDir, filePattern);
writer.WriteInfoLine($"Found {files.Length} files to process", theme);

if (files.Length == 0)
{
    writer.WriteWarningLine("No files found matching pattern", theme);
    return;
}

// Process files with progress
int processed = 0;
int failed = 0;

foreach (var file in files)
{
    try
    {
        writer.Write($"Processing {Path.GetFileName(file)}... ");
        
        // Simulate processing
        await Task.Delay(500);
        var size = new FileInfo(file).Length;
        
        writer.WriteWithStyle("✓ DONE", theme.Success);
        writer.WriteLine($" ({size:N0} bytes)");
        
        processed++;
    }
    catch (Exception ex)
    {
        writer.WriteWithStyle("✗ FAILED", theme.Error);
        writer.WriteLine($" - {ex.Message}");
        failed++;
    }
}

// Summary
writer.WriteRule("Processing Complete", theme.Heading);
writer.WriteKeyValues(new[]
{
    ("Total Files", files.Length.ToString()),
    ("Processed", processed.ToString()),
    ("Failed", failed.ToString()),
    ("Success Rate", $"{(processed * 100.0 / files.Length):F1}%")
}, keyStyle: theme.Info);

if (failed == 0)
{
    writer.WriteSuccessLine("All files processed successfully!", theme);
}
else
{
    writer.WriteWarningLine($"{failed} files failed to process", theme);
}
```

## Tutorial 2: Interactive Shell Application

Build a configuration management shell with multiple commands.

### Step 1: Basic Shell Setup

```csharp
using EasyCLI.Console;
using EasyCLI.Shell;

var reader = new ConsoleReader();
var writer = new ConsoleWriter();

var shell = new CliShell(reader, writer, new ShellOptions
{
    Prompt = "config>",
    PromptStyle = ConsoleThemes.Dark.Info,
    WelcomeMessage = @"
╔══════════════════════════════════╗
║    Configuration Manager v1.0    ║
║                                  ║
║  Type 'help' to see commands     ║
║  Type 'exit' to quit             ║
╚══════════════════════════════════╝"
});

// Register commands
await shell.RegisterAsync(new ConfigShowCommand());
await shell.RegisterAsync(new ConfigSetCommand());
await shell.RegisterAsync(new ConfigDeleteCommand());
await shell.RegisterAsync(new StatusCommand());

// Start the shell
await shell.RunAsync();
```

### Step 2: Configuration Show Command

```csharp
public class ConfigShowCommand : BaseCliCommand
{
    public override string Name => "show";
    public override string Description => "Display configuration settings";
    public override string Category => "Configuration";

    protected override void ConfigureHelp(CommandHelp help)
    {
        help.Usage = "show [setting-name]";
        help.Description = "Display all configuration settings or a specific setting.";
        
        help.Arguments.Add(new CommandArgument("setting-name", 
            "Name of specific setting to display", required: false));
        
        help.Options.Add(new CommandOption("json", "j", 
            "Output in JSON format"));
        help.Options.Add(new CommandOption("plain", "p", 
            "Output in plain text format"));
        
        help.Examples.Add(new CommandExample("show", 
            "Display all settings"));
        help.Examples.Add(new CommandExample("show api-url", 
            "Display specific setting"));
        help.Examples.Add(new CommandExample("show --json", 
            "Display settings in JSON format"));
    }

    protected override Task<int> ExecuteCommand(CommandLineArgs args, 
        ShellExecutionContext context, CancellationToken cancellationToken)
    {
        var theme = ConsoleThemes.Dark;
        var config = LoadConfiguration();

        if (args.HasOption("json"))
        {
            return ShowJsonFormat(config, args, context);
        }
        
        if (args.HasOption("plain"))
        {
            return ShowPlainFormat(config, args, context);
        }
        
        return ShowTableFormat(config, args, context, theme);
    }

    private Task<int> ShowTableFormat(Dictionary<string, object> config, 
        CommandLineArgs args, ShellExecutionContext context, ConsoleTheme theme)
    {
        var settingName = args.Arguments.FirstOrDefault();
        
        if (settingName != null)
        {
            // Show specific setting
            if (config.TryGetValue(settingName, out var value))
            {
                context.Writer.WriteInfoLine($"Setting: {settingName}", theme);
                context.Writer.WriteKeyValues(new[]
                {
                    ("Value", value?.ToString() ?? "null"),
                    ("Type", value?.GetType().Name ?? "null")
                });
            }
            else
            {
                context.Writer.WriteErrorLine($"Setting '{settingName}' not found", theme);
                return Task.FromResult(1);
            }
        }
        else
        {
            // Show all settings
            context.Writer.WriteRule("Configuration Settings", theme.Heading);
            
            if (config.Any())
            {
                var rows = config.Select(kvp => new[] 
                { 
                    kvp.Key, 
                    kvp.Value?.ToString() ?? "null",
                    kvp.Value?.GetType().Name ?? "null"
                }).ToArray();
                
                context.Writer.WriteTableSimple(
                    new[] { "Setting", "Value", "Type" },
                    rows,
                    headerStyle: theme.Info,
                    borderStyle: theme.Hint);
            }
            else
            {
                context.Writer.WriteWarningLine("No configuration settings found", theme);
                context.Writer.WriteHintLine("Use 'set <name> <value>' to add settings", theme);
            }
        }
        
        return Task.FromResult(0);
    }

    private Task<int> ShowJsonFormat(Dictionary<string, object> config, 
        CommandLineArgs args, ShellExecutionContext context)
    {
        var settingName = args.Arguments.FirstOrDefault();
        
        if (settingName != null)
        {
            if (config.TryGetValue(settingName, out var value))
            {
                var result = new { name = settingName, value = value };
                context.Writer.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                }));
            }
            else
            {
                var error = new { error = "Setting not found", setting = settingName };
                context.Writer.WriteLine(JsonSerializer.Serialize(error));
                return Task.FromResult(1);
            }
        }
        else
        {
            context.Writer.WriteLine(JsonSerializer.Serialize(config, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            }));
        }
        
        return Task.FromResult(0);
    }

    private Task<int> ShowPlainFormat(Dictionary<string, object> config, 
        CommandLineArgs args, ShellExecutionContext context)
    {
        var settingName = args.Arguments.FirstOrDefault();
        
        if (settingName != null)
        {
            if (config.TryGetValue(settingName, out var value))
            {
                context.Writer.WriteLine($"{settingName}: {value}");
            }
            else
            {
                context.Writer.WriteLine($"Error: Setting '{settingName}' not found");
                return Task.FromResult(1);
            }
        }
        else
        {
            foreach (var kvp in config)
            {
                context.Writer.WriteLine($"{kvp.Key}: {kvp.Value}");
            }
        }
        
        return Task.FromResult(0);
    }

    private Dictionary<string, object> LoadConfiguration()
    {
        // In a real application, load from file/database
        return new Dictionary<string, object>
        {
            ["api-url"] = "https://api.example.com",
            ["timeout"] = 30,
            ["debug"] = false,
            ["max-retries"] = 3
        };
    }
}
```

### Step 3: Configuration Set Command

```csharp
public class ConfigSetCommand : BaseCliCommand
{
    public override string Name => "set";
    public override string Description => "Set a configuration value";
    public override string Category => "Configuration";

    protected override void ConfigureHelp(CommandHelp help)
    {
        help.Usage = "set <name> <value>";
        help.Description = "Set a configuration setting to a specific value.";
        
        help.Arguments.Add(new CommandArgument("name", 
            "Name of the setting", required: true));
        help.Arguments.Add(new CommandArgument("value", 
            "Value to set", required: true));
        
        help.Options.Add(new CommandOption("type", "t", 
            "Specify value type (string, int, bool)", hasValue: true));
        help.Options.Add(new CommandOption("global", "g", 
            "Set as global configuration"));
        
        help.Examples.Add(new CommandExample("set api-url https://api.example.com", 
            "Set API URL"));
        help.Examples.Add(new CommandExample("set timeout 60 --type int", 
            "Set timeout as integer"));
        help.Examples.Add(new CommandExample("set debug true --type bool", 
            "Set debug flag"));
    }

    protected override Task<int> ExecuteCommand(CommandLineArgs args, 
        ShellExecutionContext context, CancellationToken cancellationToken)
    {
        if (args.Arguments.Count < 2)
        {
            ShowConciseHelp(context);
            return Task.FromResult(1);
        }

        var name = args.Arguments[0];
        var valueStr = args.Arguments[1];
        var type = args.GetOption("type", "string");
        var isGlobal = args.HasOption("global");
        var theme = ConsoleThemes.Dark;

        // Parse value based on type
        object value;
        try
        {
            value = ParseValue(valueStr, type);
        }
        catch (Exception ex)
        {
            context.Writer.WriteErrorLine($"Invalid value for type '{type}': {ex.Message}", theme);
            return Task.FromResult(1);
        }

        // Save configuration
        SaveConfiguration(name, value, isGlobal);
        
        var scope = isGlobal ? "global" : "local";
        context.Writer.WriteSuccessLine($"✓ Set {scope} setting '{name}' = {value}", theme);
        
        return Task.FromResult(0);
    }

    private object ParseValue(string valueStr, string type)
    {
        return type.ToLowerInvariant() switch
        {
            "string" => valueStr,
            "int" => int.Parse(valueStr),
            "bool" => bool.Parse(valueStr),
            "double" => double.Parse(valueStr),
            _ => throw new ArgumentException($"Unsupported type: {type}")
        };
    }

    private void SaveConfiguration(string name, object value, bool isGlobal)
    {
        // In a real application, save to file/database
        Console.WriteLine($"Saved {name} = {value} (global: {isGlobal})");
    }
}
```

### Step 4: Status Command

```csharp
public class StatusCommand : BaseCliCommand
{
    public override string Name => "status";
    public override string Description => "Show system status";
    public override string Category => "System";

    protected override Task<int> ExecuteCommand(CommandLineArgs args, 
        ShellExecutionContext context, CancellationToken cancellationToken)
    {
        var theme = ConsoleThemes.Dark;
        
        context.Writer.WriteTitledBox("System Status", new[]
        {
            $"Application: Configuration Manager",
            $"Version: 1.0.0",
            $"Uptime: {TimeSpan.FromMilliseconds(Environment.TickCount64):hh\\:mm\\:ss}",
            $"Working Directory: {context.WorkingDirectory}",
            $"Platform: {Environment.OSVersion.Platform}",
            $"CLR Version: {Environment.Version}"
        }, theme);

        // Memory usage
        var memoryUsage = GC.GetTotalMemory(false);
        context.Writer.WriteKeyValues(new[]
        {
            ("Memory Usage", $"{memoryUsage:N0} bytes"),
            ("Gen 0 Collections", GC.CollectionCount(0).ToString()),
            ("Gen 1 Collections", GC.CollectionCount(1).ToString()),
            ("Gen 2 Collections", GC.CollectionCount(2).ToString())
        }, keyStyle: theme.Info);

        return Task.FromResult(0);
    }
}
```

## Tutorial 3: Enhanced CLI Application

Build a professional deployment tool using enhanced CLI features.

### Step 1: Enhanced Command Structure

```csharp
using EasyCLI.Shell;

public class DeployCommand : EnhancedCliCommand
{
    public override string Name => "deploy";
    public override string Description => "Deploy application to specified environment";
    public override string Category => "Deployment";

    protected override void ConfigureHelp(CommandHelp help)
    {
        help.Usage = "deploy [options] <environment>";
        help.Description = "Deploy the application to the specified environment with optional configuration.";

        help.Arguments.Add(new CommandArgument("environment", 
            "Target environment (dev, staging, production)", required: true));

        help.Options.Add(new CommandOption("config", "c", 
            "Custom configuration file path", hasValue: true));
        help.Options.Add(new CommandOption("timeout", "t", 
            "Deployment timeout in seconds", hasValue: true));
        help.Options.Add(new CommandOption("rollback", "r", 
            "Enable automatic rollback on failure"));

        help.Examples.Add(new CommandExample("deploy staging", 
            "Deploy to staging environment"));
        help.Examples.Add(new CommandExample("deploy production --config prod.json --timeout 300", 
            "Deploy to production with custom config and timeout"));
        help.Examples.Add(new CommandExample("deploy dev --dry-run --verbose", 
            "Preview deployment to dev environment with verbose output"));
    }

    protected override async Task<int> ExecuteCommand(CommandLineArgs args, 
        ShellExecutionContext context, CancellationToken cancellationToken)
    {
        var environment = args.Arguments[0];
        var configFile = args.GetOption("config");
        var timeout = args.GetOptionAsInt("timeout", 300);
        var enableRollback = args.HasFlag("rollback");

        // Validate environment
        var validEnvironments = new[] { "dev", "staging", "production" };
        if (!validEnvironments.Contains(environment.ToLowerInvariant()))
        {
            Logger?.LogError($"Invalid environment: {environment}");
            Logger?.LogHint($"Valid environments: {string.Join(", ", validEnvironments)}");
            return ExitCodes.InvalidArguments;
        }

        // Load configuration
        await LoadDeploymentConfig(configFile);

        // Check if this is a dangerous environment
        if (environment.ToLowerInvariant() == "production")
        {
            if (!await ConfirmDangerousOperation(
                $"Deploy to PRODUCTION environment?",
                "This will affect live users and cannot be easily undone.",
                args, context))
            {
                Logger?.LogInfo("Deployment cancelled");
                return ExitCodes.UserCancelled;
            }
        }

        if (args.IsDryRun)
        {
            return await PreviewDeployment(environment, configFile, timeout, enableRollback);
        }

        return await ExecuteDeployment(environment, configFile, timeout, enableRollback, cancellationToken);
    }

    private async Task<int> PreviewDeployment(string environment, string? configFile, 
        int timeout, bool enableRollback)
    {
        Logger?.LogWarning("[DRY RUN] No changes will be made");
        Logger?.LogInfo($"Would deploy to: {environment}");
        Logger?.LogInfo($"Would use config: {configFile ?? "default"}");
        Logger?.LogInfo($"Would timeout after: {timeout}s");
        Logger?.LogInfo($"Rollback enabled: {enableRollback}");

        // Simulate deployment steps
        var steps = new[]
        {
            "Validate configuration",
            "Build application",
            "Run tests", 
            "Package artifacts",
            "Upload to server",
            "Deploy application",
            "Run health checks",
            "Update load balancer"
        };

        foreach (var step in steps)
        {
            Logger?.LogInfo($"Would execute: {step}");
            await Task.Delay(200); // Simulate processing time
        }

        Logger?.LogSuccess("Dry run completed successfully");
        Logger?.LogHint("Run without --dry-run to execute deployment");

        return ExitCodes.Success;
    }

    private async Task<int> ExecuteDeployment(string environment, string? configFile, 
        int timeout, bool enableRollback, CancellationToken cancellationToken)
    {
        Logger?.LogInfo($"Starting deployment to {environment}...");

        try
        {
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
            using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken, timeoutCts.Token);

            await DeployApplication(environment, configFile, enableRollback, combinedCts.Token);
            
            Logger?.LogSuccess("✓ Deployment completed successfully!");
            return ExitCodes.Success;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            Logger?.LogWarning("Deployment cancelled by user");
            return ExitCodes.UserCancelled;
        }
        catch (OperationCanceledException)
        {
            Logger?.LogError($"Deployment timed out after {timeout} seconds");
            
            if (enableRollback)
            {
                Logger?.LogInfo("Attempting automatic rollback...");
                await RollbackDeployment(environment);
            }
            
            return ExitCodes.ServiceUnavailable;
        }
        catch (Exception ex)
        {
            Logger?.LogError($"Deployment failed: {ex.Message}");
            
            if (enableRollback)
            {
                Logger?.LogInfo("Attempting automatic rollback...");
                await RollbackDeployment(environment);
            }
            
            return ExitCodes.GeneralError;
        }
    }

    private async Task DeployApplication(string environment, string? configFile, 
        bool enableRollback, CancellationToken cancellationToken)
    {
        var steps = new[]
        {
            ("Validating configuration", () => ValidateConfiguration(configFile)),
            ("Building application", () => BuildApplication(cancellationToken)),
            ("Running tests", () => RunTests(cancellationToken)),
            ("Packaging artifacts", () => PackageArtifacts(cancellationToken)),
            ("Uploading to server", () => UploadArtifacts(environment, cancellationToken)),
            ("Deploying application", () => DeployToServer(environment, cancellationToken)),
            ("Running health checks", () => RunHealthChecks(environment, cancellationToken)),
            ("Updating load balancer", () => UpdateLoadBalancer(environment, cancellationToken))
        };

        foreach (var (stepName, stepAction) in steps)
        {
            Logger?.LogInfo($"Executing: {stepName}...");
            await stepAction();
            Logger?.LogVerbose($"Completed: {stepName}");
        }
    }

    private async Task LoadDeploymentConfig(string? configFile)
    {
        if (configFile != null)
        {
            Logger?.LogVerbose($"Loading custom configuration: {configFile}");
            // Load custom config
        }
        else
        {
            Logger?.LogVerbose("Using default configuration");
            // Use Config from base class
        }
    }

    // Implementation methods...
    private Task ValidateConfiguration(string? configFile) => Task.Delay(500);
    private Task BuildApplication(CancellationToken ct) => Task.Delay(2000, ct);
    private Task RunTests(CancellationToken ct) => Task.Delay(3000, ct);
    private Task PackageArtifacts(CancellationToken ct) => Task.Delay(1000, ct);
    private Task UploadArtifacts(string env, CancellationToken ct) => Task.Delay(2000, ct);
    private Task DeployToServer(string env, CancellationToken ct) => Task.Delay(1500, ct);
    private Task RunHealthChecks(string env, CancellationToken ct) => Task.Delay(1000, ct);
    private Task UpdateLoadBalancer(string env, CancellationToken ct) => Task.Delay(500, ct);
    private Task RollbackDeployment(string env) => Task.Delay(1000);
}
```

### Step 2: Application Entry Point

```csharp
using EasyCLI.Console;
using EasyCLI.Shell;

class Program
{
    static async Task Main(string[] args)
    {
        var reader = new ConsoleReader();
        var writer = new ConsoleWriter();
        var theme = ConsoleThemes.Dark;

        // Show welcome banner
        writer.WriteTitledBox("Deployment Manager v2.0", new[]
        {
            "Professional deployment tool built with EasyCLI",
            "Supports multiple environments and rollback",
            "",
            "Type 'help' for available commands"
        }, theme);

        // Create and configure shell
        var shell = new CliShell(reader, writer, new ShellOptions
        {
            Prompt = "deploy>",
            PromptStyle = theme.Info,
            HistoryLimit = 50,
            ShowHelpOnStart = false
        });

        // Register enhanced commands
        await shell.RegisterAsync(new DeployCommand());
        await shell.RegisterAsync(new StatusCommand());
        await shell.RegisterAsync(new ConfigCommand());

        // Start interactive mode
        await shell.RunAsync();
    }
}
```

## Example 4: PowerShell Integration

Using EasyCLI cmdlets in PowerShell scripts.

### Installation

```powershell
# Install the PowerShell module
Install-Module -Name EasyCLI

# Import the module
Import-Module EasyCLI
```

### Basic Usage

```powershell
# Simple styled messages
Write-Message "Starting deployment process..." -Style Info
Write-Message "Deployment completed successfully!" -Style Success
Write-Message "Warning: Cache is 90% full" -Style Warning
Write-Message "Error: Unable to connect to server" -Style Error

# Using aliases
Show-Message "This is easier to type!" -Style Success
```

### Interactive Scripts

```powershell
# Get user input
$environment = Read-Choice "Select environment" @("Development", "Staging", "Production")
$shouldDeploy = Read-Choice "Deploy to $environment?" @("Yes", "No")

if ($shouldDeploy -eq "Yes") {
    Show-Message "Deploying to $environment..." -Style Info
    
    # Simulate deployment
    1..10 | ForEach-Object {
        Show-Message "Processing step $_/10" -Style Info
        Start-Sleep -Milliseconds 500
    }
    
    Show-Message "Deployment completed!" -Style Success
} else {
    Show-Message "Deployment cancelled" -Style Warning
}
```

### Pipeline Integration

```powershell
# Process files with pipeline
Get-ChildItem *.txt | ForEach-Object {
    $result = Process-File $_.Name
    $result | Write-Message -Style Success
}

# Configuration management
$config = @{
    "ApiUrl" = "https://api.example.com"
    "Timeout" = 30
    "Debug" = $true
}

$config.GetEnumerator() | ForEach-Object {
    "$($_.Key): $($_.Value)" | Write-Message -Style Info
}
```

## Best Practices Summary

### 1. Consistent Error Handling

```csharp
try
{
    await DoSomething();
    return ExitCodes.Success;
}
catch (FileNotFoundException ex)
{
    Logger?.LogError($"File not found: {ex.FileName}");
    ShowSuggestion(context, "Make sure the file exists and you have read permissions");
    return ExitCodes.FileNotFound;
}
catch (Exception ex)
{
    Logger?.LogError($"Unexpected error: {ex.Message}");
    Logger?.LogDebug($"Stack trace: {ex.StackTrace}");
    return ExitCodes.GeneralError;
}
```

### 2. Progressive Disclosure

```csharp
// Show concise help by default
if (args.Arguments.Count == 0)
{
    ShowConciseHelp(context);
    return ExitCodes.InvalidArguments;
}

// Detailed help when explicitly requested
if (args.ShowHelp)
{
    ShowHelp(context);
    return ExitCodes.Success;
}
```

### 3. Environment Awareness

```csharp
// Adapt behavior based on environment
if (Environment?.IsContinuousIntegration == true)
{
    // Non-interactive mode for CI/CD
    Logger?.LogInfo("CI environment detected - using automated mode");
    return await ExecuteAutomated(args, cancellationToken);
}

// Respect output preferences
if (args.IsPlain || !context.Writer.SupportsColor)
{
    // Plain text output
    context.Writer.WriteLine("Status: OK");
}
else
{
    // Rich formatted output
    context.Writer.WriteSuccessLine("✓ Status: OK", theme);
}
```

### 4. Comprehensive Testing

```csharp
[Test]
public async Task DeployCommand_ProductionEnvironment_RequiresConfirmation()
{
    // Arrange
    var input = new StringReader("n\n"); // User says no
    var output = new StringWriter();
    var context = CreateTestContext(input, output);
    var command = new DeployCommand();
    var args = new[] { "production" };

    // Act
    var result = await command.ExecuteAsync(context, args, CancellationToken.None);

    // Assert
    Assert.That(result, Is.EqualTo(ExitCodes.UserCancelled));
    Assert.That(output.ToString(), Contains.Substring("Deploy to PRODUCTION"));
}
```

## Next Steps

- **[Contributing](Contributing)** - Learn how to contribute to EasyCLI
- **[API Reference](API-Reference)** - Detailed API documentation
- **[Output and Scripting](Output-and-Scripting)** - Automation and scripting guide