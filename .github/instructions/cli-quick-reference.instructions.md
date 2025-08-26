# CLI Development Quick Reference

Quick reference for implementing CLI best practices with EasyCLI. See `cli-best-practices.instructions.md` for detailed examples.

## Essential Command Template

```csharp
public class YourCommand : ICliCommand
{
    public string Name => "your-command";
    public string Description => "Brief description of what this command does";
    
    public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken ct)
    {
        // 1. Parse arguments and flags
        var options = ParseArgs(args);
        
        // 2. Show help if requested
        if (options.ShowHelp)
        {
            ShowHelp(context);
            return Task.FromResult(0);
        }
        
        // 3. Validate inputs
        if (!ValidateInputs(options, context))
            return Task.FromResult(1);
        
        // 4. Execute with proper error handling
        try
        {
            var result = ExecuteCommand(options, context);
            context.Writer.WriteSuccessLine("Operation completed successfully");
            return Task.FromResult(0);
        }
        catch (Exception ex)
        {
            context.Writer.WriteErrorLine($"Error: {ex.Message}");
            return Task.FromResult(1);
        }
    }
}
```

## Required Standards Checklist

- [ ] **Command structure**: Clear verb-noun naming pattern
- [ ] **Help support**: Respond to `--help` and `-h` flags
- [ ] **Exit codes**: Return 0 for success, non-zero for errors
- [ ] **Error messages**: Clear, actionable error descriptions
- [ ] **Input validation**: Validate all user inputs before processing

## Common Flag Patterns

```csharp
// Standard CLI flags to support
--help, -h          // Show help
--verbose, -v       // Increase output detail
--quiet, -q         // Reduce output
--dry-run, -n       // Show what would happen without executing
--force, -f         // Skip confirmations
--output, -o        // Specify output file/format
--config, -c        // Specify config file
--yes, -y           // Assume yes to prompts
```

## Output Best Practices

```csharp
// Use themed output for consistency
var theme = ConsoleThemes.Dark; // or Light, HighContrast

// Success states
context.Writer.WriteSuccessLine("✓ Operation completed", theme);

// Warnings
context.Writer.WriteWarningLine("⚠ Configuration file not found, using defaults", theme);

// Errors  
context.Writer.WriteErrorLine("✗ Failed to connect to service", theme);

// Information
context.Writer.WriteInfoLine("Processing 142 files...", theme);

// Structured data
context.Writer.WriteKeyValues(new[]
{
    ("Status", "Running"),
    ("Uptime", "2h 35m"),
    ("Memory", "512MB")
});
```

## Environment Integration

```csharp
// Respect standard environment variables
var isCI = Environment.GetEnvironmentVariable("CI") != null;
var isQuiet = args.Contains("--quiet") || isCI;

// EasyCLI automatically respects:
// NO_COLOR=1        // Disable colors
// FORCE_COLOR=1     // Force colors even when redirected
```

## Interactive vs Automated Detection

```csharp
public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken ct)
{
    var isAutomated = !context.Reader.IsInteractive || 
                     Environment.GetEnvironmentVariable("CI") != null ||
                     args.Contains("--non-interactive");
    
    if (isAutomated)
    {
        // Machine-readable output, no prompts
        var result = new { status = "success", data = GetData() };
        context.Writer.WriteLine(JsonSerializer.Serialize(result));
    }
    else
    {
        // Human-friendly output, can use prompts
        context.Writer.WriteSuccessLine("Welcome! Let's get started...");
        var name = new StringPrompt("Your name", context.Writer, context.Reader).GetValue();
        // ... continue with interactive flow
    }
    
    return Task.FromResult(0);
}
```

## Error Handling Patterns

```csharp
// Specific exit codes for different error types
public static class ExitCodes
{
    public const int Success = 0;
    public const int GeneralError = 1;
    public const int FileNotFound = 2;
    public const int PermissionDenied = 3;
    public const int InvalidArguments = 4;
    public const int ServiceUnavailable = 5;
}

// Exception handling with helpful messages
try
{
    await DoSomething();
    return ExitCodes.Success;
}
catch (FileNotFoundException ex)
{
    context.Writer.WriteErrorLine($"File not found: {ex.FileName}");
    context.Writer.WriteHintLine("Make sure the file exists and you have read permissions");
    return ExitCodes.FileNotFound;
}
catch (UnauthorizedAccessException)
{
    context.Writer.WriteErrorLine("Permission denied");
    context.Writer.WriteHintLine("Try running with elevated permissions or check file ownership");
    return ExitCodes.PermissionDenied;
}
```

## Configuration Pattern

```csharp
public class AppConfig
{
    public string ApiUrl { get; set; } = "https://api.example.com";
    public int Timeout { get; set; } = 30;
    public bool EnableLogging { get; set; } = true;
    
    public static AppConfig Load()
    {
        var globalPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".myapp", "config.json");
        var localPath = Path.Combine(Directory.GetCurrentDirectory(), ".myapp.json");
        
        var config = new AppConfig();
        
        // Load global config first
        if (File.Exists(globalPath))
        {
            var globalJson = File.ReadAllText(globalPath);
            var globalConfig = JsonSerializer.Deserialize<AppConfig>(globalJson);
            config = MergeConfigs(config, globalConfig);
        }
        
        // Override with local config
        if (File.Exists(localPath))
        {
            var localJson = File.ReadAllText(localPath);
            var localConfig = JsonSerializer.Deserialize<AppConfig>(localJson);
            config = MergeConfigs(config, localConfig);
        }
        
        return config;
    }
}
```

## Shell Integration Example

```csharp
// Program.cs - Setting up a professional CLI shell
using EasyCLI.Console;
using EasyCLI.Shell;

var reader = new ConsoleReader();
var writer = new ConsoleWriter();

var shell = new CliShell(reader, writer, new ShellOptions
{
    Prompt = "mycli>",
    PromptStyle = ConsoleThemes.Dark.Info,
    HistoryLimit = 100
});

// Register your commands
await shell.RegisterAsync(new DeployCommand());
await shell.RegisterAsync(new StatusCommand()); 
await shell.RegisterAsync(new ConfigCommand());

// Start interactive mode
await shell.RunAsync();
```

## Testing Your CLI

```bash
# Test required functionality
mycli --help                    # Should show help
mycli deploy --help            # Should show command-specific help
mycli invalid-command          # Should show error with suggestions
echo "test" | mycli process    # Should handle stdin
mycli status && echo "Success" # Should set proper exit codes

# Test environment integration
NO_COLOR=1 mycli status        # Should work without colors
CI=1 mycli deploy              # Should work in automated mode
```

## Common Mistakes to Avoid

❌ **Don't**: Hardcode file paths
✅ **Do**: Use `Path.Combine()` and environment variables

❌ **Don't**: Ignore exit codes
✅ **Do**: Return meaningful exit codes for automation

❌ **Don't**: Write directly to Console
✅ **Do**: Use EasyCLI's `ConsoleWriter` for themed, testable output

❌ **Don't**: Block on user input in CI environments
✅ **Do**: Detect automation and provide non-interactive fallbacks

❌ **Don't**: Swallow exceptions silently
✅ **Do**: Provide clear error messages with actionable guidance