# ErrorCollector - Batch Error Aggregation

The `ErrorCollector` class in EasyCLI provides a mechanism for accumulating and aggregating errors during batch operations, making it easy to collect, categorize, and present grouped error summaries to users.

## Features

- **Error Categorization**: Automatically groups errors by category (FileSystem, Network, Validation, etc.)
- **Rich Output Formatting**: Provides styled, table-based summaries and detailed error reports
- **Exception Inference**: Automatically infers error categories from exception types
- **Flexible Reporting**: Supports both summary and detailed views
- **Theme Support**: Respects console themes for consistent styling

## Basic Usage

```csharp
using EasyCLI;
using EasyCLI.Console;

var writer = new ConsoleWriter();
var collector = new ErrorCollector(writer);

// Add errors during batch processing
collector.AddError(BatchErrorCategory.FileSystem, "File not found", "config.json");
collector.AddError(BatchErrorCategory.Network, "Connection timeout", "api.example.com");

// Print summary at the end
collector.PrintSummary(showDetails: true);
```

## Error Categories

The `BatchErrorCategory` enum provides the following built-in categories:

- `General` - General application errors
- `FileSystem` - File and directory operations
- `Network` - Network connectivity issues
- `Validation` - Input validation errors
- `InvalidArgument` - Invalid arguments or parameters
- `Security` - Permission and security errors
- `Configuration` - Configuration-related errors
- `ExternalService` - External service failures

## Adding Errors

### Manual Error Addition

```csharp
// Basic error with category and message
collector.AddError(BatchErrorCategory.FileSystem, "Disk space insufficient");

// Error with source context
collector.AddError(BatchErrorCategory.Network, "Connection failed", "api.example.com");

// Error with full details
collector.AddError(BatchErrorCategory.Validation, 
    "Invalid email format", 
    "user@invalid", 
    "Expected format: user@domain.com");
```

### Exception-Based Error Addition

The collector can automatically infer categories from exception types:

```csharp
try
{
    // Some operation that might fail
    var content = File.ReadAllText("config.json");
}
catch (Exception ex)
{
    // Automatically categorizes as FileSystem error
    collector.AddError(ex, "config.json");
}
```

**Exception Category Mapping:**
- `FileNotFoundException`, `DirectoryNotFoundException`, `UnauthorizedAccessException` → `FileSystem`
- `HttpRequestException` → `Network`
- `ArgumentException`, `ArgumentNullException`, `ArgumentOutOfRangeException` → `Validation`
- `SecurityException` → `Security`
- Other exceptions → `General`

## Reporting Options

### Summary Report

```csharp
// Basic summary (shows counts and percentages)
collector.PrintSummary(showDetails: false);
```

Output:
```
Error Summary (8 total)

+------------------+-------+------+
| Category         | Count | %    |
+------------------+-------+------+
| File System      | 4     | 50.0%|
| Validation       | 3     | 37.5%|
| Network          | 1     | 12.5%|
+------------------+-------+------+

💡 Use --verbose or --details to see individual error messages
```

### Detailed Report

```csharp
// Full detailed report
collector.PrintSummary(showDetails: true);
```

Shows the summary table followed by detailed error listings for each category.

### Category-Specific Details

```csharp
// Show details for specific categories only
collector.PrintCategoryDetails(BatchErrorCategory.FileSystem);
```

## Advanced Usage

### Filtering Summaries

```csharp
// Get summaries for specific categories only
var criticalSummaries = collector.GetSummaries(
    BatchErrorCategory.FileSystem, 
    BatchErrorCategory.Security);

foreach (var summary in criticalSummaries)
{
    Console.WriteLine($"{summary.CategoryDisplayName}: {summary.Count} errors");
}
```

### Programmatic Access

```csharp
// Access collected errors programmatically
Console.WriteLine($"Total errors: {collector.TotalCount}");
Console.WriteLine($"Has errors: {collector.HasErrors}");

foreach (var error in collector.Errors)
{
    Console.WriteLine($"[{error.Category}] {error.Message}");
    if (!string.IsNullOrEmpty(error.Source))
        Console.WriteLine($"  Source: {error.Source}");
}
```

### Clearing Errors

```csharp
// Clear all collected errors
collector.Clear();
```

## Batch Processing Example

Here's a complete example showing how to use ErrorCollector in a batch file processing scenario:

```csharp
using EasyCLI;
using EasyCLI.Console;

public async Task ProcessFilesAsync(string[] filePaths)
{
    var writer = new ConsoleWriter();
    var collector = new ErrorCollector(writer);
    int successCount = 0;

    writer.WriteInfoLine($"Processing {filePaths.Length} files...");

    foreach (var filePath in filePaths)
    {
        try
        {
            await ProcessFileAsync(filePath);
            successCount++;
        }
        catch (FileNotFoundException ex)
        {
            collector.AddError(ex, filePath);
        }
        catch (UnauthorizedAccessException ex)
        {
            collector.AddError(ex, filePath);
        }
        catch (ArgumentException ex)
        {
            collector.AddError(ex, filePath);
        }
        catch (Exception ex)
        {
            collector.AddError(BatchErrorCategory.General, ex.Message, filePath);
        }
    }

    // Report results
    writer.WriteLine();
    writer.WriteSuccessLine($"✓ Successfully processed {successCount} files");
    
    if (collector.HasErrors)
    {
        writer.WriteLine();
        collector.PrintSummary(showDetails: true);
    }
}
```

## Integration with Themes

ErrorCollector respects EasyCLI's console themes for consistent styling:

```csharp
var collector = new ErrorCollector(writer, ConsoleThemes.Light);
```

The collector will use the specified theme for all output formatting, ensuring consistency with your application's visual style.

## Best Practices

1. **Use specific categories** - Choose the most appropriate category for each error type
2. **Provide context** - Include source information (file names, URLs, etc.) where relevant
3. **Add details for complex errors** - Include additional context that helps with debugging
4. **Clear between operations** - Use `Clear()` when starting a new batch operation
5. **Show summaries** - Always provide error summaries at the end of batch operations
6. **Handle exceptions consistently** - Use the exception-based `AddError` method for consistent categorization

## Thread Safety

**Note**: ErrorCollector is not thread-safe. If you need to collect errors from multiple threads, use synchronization mechanisms or collect errors in thread-local storage and merge them afterwards.