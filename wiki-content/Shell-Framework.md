# Shell Framework

EasyCLI provides an experimental interactive shell framework for building command-line applications with persistent sessions and custom command support.

## Overview

The Shell Framework allows you to create interactive command-line applications where users can execute multiple commands in a persistent session, similar to PowerShell, bash, or database CLIs.

**⚠️ Experimental Status**: The Shell Framework is currently in experimental status and APIs may change before 1.0 stabilization.

## Basic Shell Setup

### Creating a Shell

```csharp
using EasyCLI.Console;
using EasyCLI.Shell;

var reader = new ConsoleReader();
var writer = new ConsoleWriter();

var shell = new CliShell(reader, writer, new ShellOptions
{
    Prompt = "myapp>",
    PromptStyle = ConsoleThemes.Dark.Info,
    HistoryLimit = 100,
    WelcomeMessage = "Welcome to MyApp CLI!"
});

// Register commands
await shell.RegisterAsync(new GreetCommand());
await shell.RegisterAsync(new ConfigCommand());
await shell.RegisterAsync(new StatusCommand());

// Start interactive shell
await shell.RunAsync();
```

### Shell Options

```csharp
var options = new ShellOptions
{
    Prompt = "myapp>",                          // Command prompt text
    PromptStyle = ConsoleThemes.Dark.Info,     // Prompt styling
    HistoryLimit = 100,                        // Command history size
    WelcomeMessage = "Welcome!",               // Message shown on startup
    ExitCommands = new[] { "exit", "quit" },   // Commands that exit shell
    ShowHelpOnStart = true,                    // Show help on startup
    ClearScreenOnStart = false                 // Clear screen on startup
};
```

## Command Implementation

### Basic Command Interface

```csharp
public interface ICliCommand
{
    string Name { get; }
    string Description { get; }
    string Category { get; } // Optional, defaults to "General"
    
    Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, 
        CancellationToken cancellationToken);
}
```

### Simple Command Example

```csharp
public class GreetCommand : ICliCommand
{
    public string Name => "greet";
    public string Description => "Greets a user";
    public string Category => "Utility";

    public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, 
        CancellationToken cancellationToken)
    {
        var name = args.Length > 0 ? args[0] : "World";
        var theme = ConsoleThemes.Dark;
        
        context.Writer.WriteSuccessLine($"Hello, {name}!", theme);
        
        return Task.FromResult(0);
    }
}
```

### Enhanced Command with Help

```csharp
public class ConfigCommand : BaseCliCommand
{
    public override string Name => "config";
    public override string Description => "Manage application configuration";
    public override string Category => "Configuration";

    protected override void ConfigureHelp(CommandHelp help)
    {
        help.Usage = "config <action> [options]";
        help.Description = "Manage configuration settings for the application.";

        help.Arguments.Add(new CommandArgument("action", 
            "Action to perform (show, set, get, reset)", required: true));

        help.Options.Add(new CommandOption("global", "g", 
            "Use global configuration"));
        help.Options.Add(new CommandOption("local", "l", 
            "Use local configuration"));

        help.Examples.Add(new CommandExample("config show", 
            "Display current configuration"));
        help.Examples.Add(new CommandExample("config set api-url https://api.example.com", 
            "Set API URL"));
        help.Examples.Add(new CommandExample("config get api-url", 
            "Get specific setting"));
    }

    protected override async Task<int> ExecuteCommand(CommandLineArgs args, 
        ShellExecutionContext context, CancellationToken cancellationToken)
    {
        if (args.Arguments.Count == 0)
        {
            ShowConciseHelp(context);
            return 1;
        }

        var action = args.Arguments[0].ToLowerInvariant();

        return action switch
        {
            "show" => ShowConfiguration(context),
            "set" => SetConfiguration(args, context),
            "get" => GetConfiguration(args, context),
            "reset" => ResetConfiguration(context),
            _ => ShowInvalidAction(context, action)
        };
    }

    private int ShowConfiguration(ShellExecutionContext context)
    {
        // Implementation
        context.Writer.WriteInfoLine("Current Configuration:", ConsoleThemes.Dark);
        // ... show config
        return 0;
    }

    // Other methods...
}
```

## Command Categories

Commands can be organized into categories for better help organization:

```csharp
public class DeployCommand : ICliCommand
{
    public string Category => "Deployment";
    // ... rest of implementation
}

public class LogsCommand : ICliCommand  
{
    public string Category => "Monitoring";
    // ... rest of implementation
}
```

### Built-in Categories

- **Core**: Essential shell operations (built-in: help, history, cd, clear, etc.)
- **Utility**: Text processing, file operations, system utilities
- **Configuration**: Settings and environment management
- **General**: Default category when not specified
- **Custom**: Define domain-specific categories

## Built-in Commands

The shell provides several built-in commands:

### Help System

```bash
help                    # Show categorized command overview
help all               # Show full categorized command index  
help <command>         # Show detailed help for specific command
help --examples        # Show example usage patterns
```

Example help output:
```
Available Commands

Core:
  help         Show help or detailed help for a command
  history      Show recent command history
  clear        Clear the screen
  cd           Change current directory
  ... and 4 more

Utility:
  greet        Greets a user
  echo         Print text to the console with optional styling
  ... and 2 more

Configuration:
  config       Manage application configuration
  ... and 1 more
```

### History Management

```bash
history                 # Show command history
history clear          # Clear command history
!n                     # Execute command number n from history
!!                     # Execute last command
```

### Directory Navigation

```bash
cd <path>              # Change directory
pwd                    # Show current directory
```

### Shell Control

```bash
clear                  # Clear screen
exit                   # Exit shell
quit                   # Exit shell (alias)
```

## External Process Delegation

The shell can delegate unknown commands to the operating system:

```csharp
var shell = new CliShell(reader, writer, new ShellOptions
{
    AllowExternalCommands = true,  // Enable external process delegation
    ExternalCommandTimeout = 30    // Timeout in seconds
});
```

When enabled, commands not found in the registered command list will be executed as external processes:

```bash
myapp> ls -la               # Executes system 'ls' command
myapp> git status           # Executes git command
myapp> dotnet build         # Executes dotnet command
```

### External Command Security

```csharp
var shell = new CliShell(reader, writer, new ShellOptions
{
    AllowExternalCommands = true,
    ExternalCommandAllowList = new[] { "git", "dotnet", "npm" }, // Only allow specific commands
    ExternalCommandBlockList = new[] { "rm", "del", "format" }   // Block dangerous commands
});
```

## Shell Context and State

### Execution Context

Each command receives a `ShellExecutionContext` that provides:

```csharp
public class ShellExecutionContext
{
    public IConsoleWriter Writer { get; }       // Output writer
    public IConsoleReader Reader { get; }       // Input reader
    public string WorkingDirectory { get; set; } // Current directory
    public Dictionary<string, object> Variables { get; } // Session variables
    public CancellationToken CancellationToken { get; } // Cancellation support
}
```

### Session Variables

Commands can store and retrieve session-scoped variables:

```csharp
public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, 
    CancellationToken cancellationToken)
{
    // Store session variable
    context.Variables["api_url"] = "https://api.example.com";
    
    // Retrieve session variable
    if (context.Variables.TryGetValue("api_url", out var url))
    {
        context.Writer.WriteInfoLine($"Using API URL: {url}");
    }
    
    return Task.FromResult(0);
}
```

### Working Directory

The shell maintains a current working directory that commands can access and modify:

```csharp
public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, 
    CancellationToken cancellationToken)
{
    // Get current directory
    var currentDir = context.WorkingDirectory;
    
    // Change directory
    if (args.Length > 0 && Directory.Exists(args[0]))
    {
        context.WorkingDirectory = Path.GetFullPath(args[0]);
        context.Writer.WriteSuccessLine($"Changed to: {context.WorkingDirectory}");
    }
    
    return Task.FromResult(0);
}
```

## Advanced Shell Features

### Command Aliases

```csharp
var shell = new CliShell(reader, writer);

// Register command with aliases
await shell.RegisterAsync(new ConfigCommand(), aliases: new[] { "cfg", "settings" });

// Now all of these work:
// myapp> config show
// myapp> cfg show  
// myapp> settings show
```

### Command Completion

Basic tab completion is supported:

```csharp
var shell = new CliShell(reader, writer, new ShellOptions
{
    EnableTabCompletion = true,    // Enable basic tab completion
    CompletionMode = CompletionMode.Commands | CompletionMode.Files
});
```

### Startup Commands

Execute commands when the shell starts:

```csharp
var shell = new CliShell(reader, writer, new ShellOptions
{
    StartupCommands = new[] 
    { 
        "config load",
        "status check",
        "help" 
    }
});
```

## Command Testing

### Testing Shell Commands

```csharp
[Test]
public async Task GreetCommand_WithName_ShowsGreeting()
{
    // Arrange
    var input = new StringReader("");
    var output = new StringWriter();
    var reader = new ConsoleReader(input);
    var writer = new ConsoleWriter(enableColors: false, output);
    var context = new ShellExecutionContext(writer, reader, "/tmp", new Dictionary<string, object>());
    
    var command = new GreetCommand();
    var args = new[] { "Alice" };
    
    // Act
    var result = await command.ExecuteAsync(context, args, CancellationToken.None);
    
    // Assert
    Assert.That(result, Is.EqualTo(0));
    Assert.That(output.ToString(), Contains.Substring("Hello, Alice!"));
}
```

### Testing Shell Integration

```csharp
[Test]
public async Task Shell_WithGreetCommand_ExecutesCorrectly()
{
    // Arrange
    var input = new StringReader("greet Bob\nexit\n");
    var output = new StringWriter();
    var shell = new CliShell(
        new ConsoleReader(input), 
        new ConsoleWriter(enableColors: false, output));
    
    await shell.RegisterAsync(new GreetCommand());
    
    // Act
    await shell.RunAsync();
    
    // Assert
    var outputText = output.ToString();
    Assert.That(outputText, Contains.Substring("Hello, Bob!"));
    Assert.That(outputText, Contains.Substring("myapp>"));
}
```

## Error Handling

### Command Error Handling

```csharp
public async Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, 
    CancellationToken cancellationToken)
{
    try
    {
        // Command logic
        return 0;
    }
    catch (ArgumentException ex)
    {
        context.Writer.WriteErrorLine($"Invalid argument: {ex.Message}");
        return 4;
    }
    catch (Exception ex)
    {
        context.Writer.WriteErrorLine($"Unexpected error: {ex.Message}");
        return 1;
    }
}
```

### Shell Error Recovery

The shell automatically handles command errors and continues running:

```bash
myapp> invalid-command
Error: Unknown command 'invalid-command'. Type 'help' for available commands.
myapp> greet
Hello, World!
myapp> 
```

## Shell Customization

### Custom Prompt

```csharp
public class CustomShell : CliShell
{
    protected override string GetPrompt()
    {
        var time = DateTime.Now.ToString("HH:mm");
        var dir = Path.GetFileName(WorkingDirectory);
        return $"[{time}] {dir}> ";
    }
}
```

### Custom Welcome Message

```csharp
var shell = new CliShell(reader, writer, new ShellOptions
{
    WelcomeMessage = @"
╔══════════════════════════════╗
║     Welcome to MyApp CLI     ║
║   Type 'help' to get started ║
╚══════════════════════════════╝
"
});
```

## Best Practices

### Command Design

```csharp
// ✅ Good - Clear, focused command
public class UserCreateCommand : ICliCommand
{
    public string Name => "user-create";
    public string Description => "Create a new user account";
}

// ❌ Avoid - Overly broad command
public class UserCommand : ICliCommand
{
    public string Name => "user";
    public string Description => "Manage users"; // Too broad
}
```

### Error Messages

```csharp
// ✅ Good - Helpful error with suggestion
context.Writer.WriteErrorLine("Configuration file not found");
context.Writer.WriteHintLine("Run 'config init' to create a default configuration");

// ❌ Avoid - Cryptic error
context.Writer.WriteErrorLine("Error 404");
```

### Resource Management

```csharp
public async Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, 
    CancellationToken cancellationToken)
{
    // ✅ Good - Respect cancellation
    cancellationToken.ThrowIfCancellationRequested();
    
    // ✅ Good - Use async methods
    var result = await ProcessDataAsync(args[0], cancellationToken);
    
    return 0;
}
```

## Migration from Basic Commands

### Before (Basic Command)

```csharp
public class SimpleCommand : ICliCommand
{
    public string Name => "simple";
    public string Description => "A simple command";
    
    public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, 
        CancellationToken cancellationToken)
    {
        context.Writer.WriteLine("Hello World");
        return Task.FromResult(0);
    }
}
```

### After (Enhanced Command)

```csharp
public class SimpleCommand : BaseCliCommand
{
    public override string Name => "simple";
    public override string Description => "A simple command";
    
    protected override void ConfigureHelp(CommandHelp help)
    {
        help.Usage = "simple [name]";
        help.Arguments.Add(new CommandArgument("name", "Name to greet", required: false));
        help.Examples.Add(new CommandExample("simple Alice", "Greet Alice"));
    }
    
    protected override Task<int> ExecuteCommand(CommandLineArgs args, 
        ShellExecutionContext context, CancellationToken cancellationToken)
    {
        var name = args.Arguments.FirstOrDefault() ?? "World";
        context.Writer.WriteSuccessLine($"Hello {name}!", ConsoleThemes.Dark);
        return Task.FromResult(0);
    }
}
```

## Next Steps

- **[CLI Enhancement Features](CLI-Enhancement-Features)** - Professional CLI patterns and enhanced commands
- **[API Reference](API-Reference)** - Detailed API documentation
- **[Examples and Tutorials](Examples-and-Tutorials)** - Complete shell application examples