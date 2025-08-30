# Signal Handling and Cleanup Hooks

EasyCLI provides robust signal handling capabilities that enable CLI applications to gracefully respond to interruption signals (like Ctrl+C) and perform cleanup operations before exiting.

## Overview

The signal handling system consists of three main components:

1. **Signal Handler** - Captures OS signals and triggers shutdown
2. **Cleanup Manager** - Coordinates cleanup actions in proper order
3. **Terminal State Manager** - Restores terminal state on exit

## Enabling Signal Handling

Signal handling is **disabled by default** to maintain backwards compatibility. Enable it via `ShellOptions`:

```csharp
var options = new ShellOptions
{
    EnableSignalHandling = true  // Enable graceful shutdown
};

using var shell = new CliShell(reader, writer, options);
```

## Basic Usage

### Simple Command with Cleanup

```csharp
public class MyCommand : ICleanupAwareCommand
{
    public string Name => "mycommand";
    public string Description => "Example command with cleanup";
    public string Category => "Demo";

    // Register cleanup actions before command execution
    public void RegisterCleanupActions(ICleanupManager cleanupManager, ShellExecutionContext context)
    {
        cleanupManager.RegisterCleanup(() =>
        {
            context.Writer.WriteInfoLine("Performing cleanup...");
            // Your cleanup logic here
        }, "my-cleanup");
    }

    public async Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken cancellationToken)
    {
        try
        {
            context.Writer.WriteInfoLine("Running long task... (Press Ctrl+C to interrupt)");
            
            // Long-running work that can be interrupted
            for (int i = 0; i < 60; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Delay(1000, cancellationToken);
            }
            
            return ExitCodes.Success;
        }
        catch (OperationCanceledException)
        {
            context.Writer.WriteWarningLine("Operation interrupted");
            return ExitCodes.UserCancelled;
        }
    }
}
```

### Cleanup Execution Order

Cleanup actions execute in **LIFO (Last In, First Out)** order - the last registered cleanup runs first:

```csharp
public void RegisterCleanupActions(ICleanupManager cleanupManager, ShellExecutionContext context)
{
    cleanupManager.RegisterCleanup(() => Console.WriteLine("First registered, runs LAST"), "first");
    cleanupManager.RegisterCleanup(() => Console.WriteLine("Second registered, runs SECOND"), "second");  
    cleanupManager.RegisterCleanup(() => Console.WriteLine("Third registered, runs FIRST"), "third");
}

// On Ctrl+C, output will be:
// Third registered, runs FIRST
// Second registered, runs SECOND  
// First registered, runs LAST
```

## Advanced Features

### Async Cleanup Actions

```csharp
cleanupManager.RegisterCleanup(async (cancellationToken) =>
{
    context.Writer.WriteInfoLine("Async cleanup starting...");
    await SomeAsyncCleanupAsync(cancellationToken);
    context.Writer.WriteInfoLine("Async cleanup completed");
}, "async-cleanup");
```

### Unregistering Cleanup Actions

```csharp
// Register cleanup and get a handle
var handle = cleanupManager.RegisterCleanup(() => 
{
    // This cleanup might not be needed anymore
}, "conditional-cleanup");

// Later, unregister if conditions change
if (someCondition)
{
    handle.Unregister();
}
```

### Terminal State Management

When signal handling is enabled, the shell automatically:

- Captures initial terminal state (cursor position, visibility)
- Restores terminal state on interruption
- Flushes output streams before exit

```csharp
// Manual terminal state operations (advanced usage)
using var terminalManager = new TerminalStateManager();

terminalManager.CaptureState();

// Temporarily hide cursor
using var hideCursor = terminalManager.TemporaryModification(TerminalModification.HideCursor);

// Cursor is automatically restored when disposed
```

## Signal Types

The system handles these signal types:

- **Interrupt** - Ctrl+C (SIGINT on Unix)
- **Terminate** - SIGTERM on Unix systems  
- **Hangup** - SIGHUP on Unix systems
- **Other** - Application-specific signals

## Implementation Notes

### Cross-Platform Support

- Uses `Console.CancelKeyPress` for cross-platform compatibility
- Gracefully handles cases where console features aren't available (redirected output)
- Future versions may add native POSIX signal support for Unix systems

### Error Handling

- Cleanup failures don't prevent other cleanup actions from running
- Cleanup has a 5-second timeout to prevent hanging during shutdown
- Terminal restoration errors are silently ignored to prevent exit issues

### Backwards Compatibility

- Existing commands work unchanged - no modifications required
- `ICleanupAwareCommand` is an optional interface for enhanced commands
- Signal handling is disabled by default

## Best Practices

### 1. Keep Cleanup Actions Fast

```csharp
// Good: Quick cleanup
cleanupManager.RegisterCleanup(() =>
{
    tempFile?.Delete();
    cache?.Clear();
}, "fast-cleanup");

// Avoid: Slow operations that might timeout
cleanupManager.RegisterCleanup(async (ct) =>
{
    await SlowNetworkOperation(ct); // Could timeout!
}, "slow-cleanup");
```

### 2. Handle Cleanup Failures Gracefully

```csharp
cleanupManager.RegisterCleanup(() =>
{
    try
    {
        File.Delete(tempFile);
    }
    catch (Exception ex)
    {
        // Log but don't throw - let other cleanup actions proceed
        context.Writer.WriteWarningLine($"Could not delete temp file: {ex.Message}");
    }
}, "safe-cleanup");
```

### 3. Use Descriptive Names

```csharp
// Good: Descriptive names help with debugging
cleanupManager.RegisterCleanup(cleanup, "database-connection-cleanup");
cleanupManager.RegisterCleanup(cleanup, "temp-file-cleanup");
cleanupManager.RegisterCleanup(cleanup, "cache-invalidation");

// Avoid: Generic names
cleanupManager.RegisterCleanup(cleanup, "cleanup1");
```

### 4. Consider Cleanup Dependencies

Register dependent cleanup actions in reverse dependency order:

```csharp
// Database should be cleaned up before connection is closed
cleanupManager.RegisterCleanup(() => connection.Close(), "close-connection");
cleanupManager.RegisterCleanup(() => database.Cleanup(), "cleanup-database");
```

## Testing Signal Handling

### Unit Testing Cleanup Logic

```csharp
[Test]
public async Task CleanupActions_ExecuteInCorrectOrder()
{
    var cleanupManager = new CleanupManager();
    var executionOrder = new List<string>();
    
    cleanupManager.RegisterCleanup(() => executionOrder.Add("first"), "first");
    cleanupManager.RegisterCleanup(() => executionOrder.Add("second"), "second");
    
    await cleanupManager.ExecuteCleanupAsync(TimeSpan.FromSeconds(5));
    
    Assert.Equal(new[] { "second", "first" }, executionOrder);
}
```

### Integration Testing

```csharp
[Test]
public void SignalHandler_TriggersShutdownToken()
{
    using var handler = new SignalHandler();
    handler.Start();
    
    Assert.False(handler.ShutdownToken.IsCancellationRequested);
    
    // Simulate Ctrl+C (would require platform-specific code)
    // In real scenarios, test by running the application and sending signals
}
```

## Migration Guide

### From Basic Commands

```csharp
// Before: Regular command
public class MyCommand : ICliCommand
{
    // ... existing implementation
}

// After: Add cleanup support
public class MyCommand : ICleanupAwareCommand  // Implements ICliCommand
{
    // ... existing implementation unchanged
    
    // Add this method to register cleanup actions
    public void RegisterCleanupActions(ICleanupManager cleanupManager, ShellExecutionContext context)
    {
        cleanupManager.RegisterCleanup(() =>
        {
            // Cleanup logic that was previously in a finally block
        }, "my-cleanup");
    }
}
```

### Enabling in Existing Applications

```csharp
// Before: Default shell
var shell = new CliShell(reader, writer);

// After: Enable signal handling
var options = new ShellOptions 
{ 
    EnableSignalHandling = true 
};
var shell = new CliShell(reader, writer, options);
```

## Troubleshooting

### "Cleanup actions not executing"
- Ensure `EnableSignalHandling = true` in `ShellOptions`
- Verify command implements `ICleanupAwareCommand`
- Check that cleanup actions are registered in `RegisterCleanupActions`

### "Application hanging on exit"
- Cleanup actions have a 5-second timeout
- Avoid long-running operations in cleanup
- Use `CancellationToken` in async cleanup actions

### "Terminal state not restored"
- This is handled automatically when signal handling is enabled
- Manual restoration only needed for custom terminal modifications
- Some terminal features may not be available in redirected environments

## Examples

See the `SignalHandlingDemo.cs` in the Demo project for a complete working example of signal handling with cleanup hooks.