# Core Features

EasyCLI provides a comprehensive set of core features for building beautiful and interactive command-line applications.

## ANSI Styling and Colors

### Basic Styling

EasyCLI includes extensive ANSI styling support with environment-aware color handling:

```csharp
using EasyCLI.Console;

var writer = new ConsoleWriter();

// Basic styled output
writer.WriteWithStyle("Bold text", ConsoleStyles.Bold);
writer.WriteWithStyle("Italic text", ConsoleStyles.Italic);  
writer.WriteWithStyle("Underlined", ConsoleStyles.Underline);
writer.WriteWithStyle("Red text", ConsoleStyles.Red);
writer.WriteWithStyle("Green background", ConsoleStyles.BgGreen);

// Combining styles
var combinedStyle = ConsoleStyles.Bold + ConsoleStyles.Blue + ConsoleStyles.BgYellow;
writer.WriteWithStyle("Bold blue text on yellow background", combinedStyle);
```

### Console Themes

EasyCLI provides built-in themes for consistent styling:

```csharp
// Available themes
var darkTheme = ConsoleThemes.Dark;        // Default dark theme
var lightTheme = ConsoleThemes.Light;      // Light theme for bright terminals
var highContrast = ConsoleThemes.HighContrast; // High contrast for accessibility

// Using themes
writer.WriteSuccessLine("✓ Success message", darkTheme);
writer.WriteWarningLine("⚠ Warning message", darkTheme);
writer.WriteErrorLine("✗ Error message", darkTheme);
writer.WriteInfoLine("ℹ Information", darkTheme);
writer.WriteHintLine("💡 Helpful hint", darkTheme);
```

### Custom Styling

Create your own styles and themes:

```csharp
// Custom style combination
var customStyle = new ConsoleStyle(
    foreground: ConsoleColor.Cyan,
    background: ConsoleColor.DarkBlue,
    bold: true
);

// Custom theme
var customTheme = new ConsoleTheme
{
    Success = ConsoleStyles.Green + ConsoleStyles.Bold,
    Warning = ConsoleStyles.Yellow,
    Error = ConsoleStyles.Red + ConsoleStyles.Bold,
    Info = ConsoleStyles.Blue,
    Hint = ConsoleStyles.Gray
};
```

### Environment-Aware Behavior

EasyCLI automatically respects environment conventions:

```csharp
// Automatically handles:
// - NO_COLOR=1 (disables all colors)
// - FORCE_COLOR=1 (forces colors even when redirected)
// - Output redirection detection
// - Terminal capability detection

var writer = new ConsoleWriter(); // Automatically detects environment
```

## Interactive Prompts

### String Input Prompts

```csharp
using EasyCLI.Prompts;

var writer = new ConsoleWriter();
var reader = new ConsoleReader();

// Basic string prompt
var namePrompt = new StringPrompt("Enter your name", writer, reader);
var name = namePrompt.GetValue();

// With default value
var urlPrompt = new StringPrompt("API URL", writer, reader, 
    defaultValue: "https://api.example.com");
var url = urlPrompt.GetValue();

// With validation
var emailPrompt = new StringPrompt("Email address", writer, reader,
    validator: new EmailValidator());
var email = emailPrompt.GetValue();
```

### Numeric Input Prompts

```csharp
// Integer input
var agePrompt = new IntPrompt("Enter your age", writer, reader,
    minValue: 0, maxValue: 120);
var age = agePrompt.GetValue();

// With default value
var portPrompt = new IntPrompt("Port number", writer, reader,
    defaultValue: 8080, minValue: 1, maxValue: 65535);
var port = portPrompt.GetValue();
```

### Yes/No Prompts

```csharp
// Simple yes/no
var confirmPrompt = new YesNoPrompt("Continue?", writer, reader);
var shouldContinue = confirmPrompt.GetValue();

// With default value
var savePrompt = new YesNoPrompt("Save changes?", writer, reader, 
    defaultValue: true);
var shouldSave = savePrompt.GetValue();
```

### Choice Prompts

```csharp
// Single choice selection
var choices = new[]
{
    new Choice<string>("Development", "dev"),
    new Choice<string>("Staging", "staging"), 
    new Choice<string>("Production", "prod")
};

var envPrompt = new ChoicePrompt<string>("Select environment", choices, writer, reader);
var environment = envPrompt.GetValue();

// Multi-select choice
var featureChoices = new[]
{
    new Choice<string>("Authentication", "auth"),
    new Choice<string>("Logging", "log"),
    new Choice<string>("Caching", "cache"),
    new Choice<string>("Monitoring", "monitor")
};

var multiPrompt = new MultiChoicePrompt<string>("Select features", featureChoices, writer, reader);
var selectedFeatures = multiPrompt.GetValues();
```

### Hidden Input Prompts

```csharp
// Password input (hidden)
var passwordPrompt = new HiddenPrompt("Password", writer, reader);
var password = passwordPrompt.GetValue();

// API key input
var keyPrompt = new HiddenPrompt("API Key", writer, reader,
    placeholder: "Enter your secret API key");
var apiKey = keyPrompt.GetValue();
```

### Input Validation

```csharp
using EasyCLI.Prompts.Validators;

// Built-in validators
var requiredValidator = new RequiredValidator();
var emailValidator = new EmailValidator();
var urlValidator = new UrlValidator();

// Custom validator
public class PortValidator : IValidator<string>
{
    public ValidationResult Validate(string value)
    {
        if (int.TryParse(value, out int port) && port >= 1 && port <= 65535)
            return ValidationResult.Success();
        
        return ValidationResult.Error("Port must be between 1 and 65535");
    }
}

// Using validator
var portPrompt = new StringPrompt("Port", writer, reader,
    validator: new PortValidator());
```

## Rich Formatting

### Tables

```csharp
// Simple table
var headers = new[] { "Name", "Age", "City" };
var rows = new[]
{
    new[] { "Alice", "30", "New York" },
    new[] { "Bob", "25", "Los Angeles" },
    new[] { "Charlie", "35", "Chicago" }
};

writer.WriteTableSimple(headers, rows,
    headerStyle: ConsoleThemes.Dark.Info,
    borderStyle: ConsoleThemes.Dark.Hint);

// Key-value table
writer.WriteKeyValues(new[]
{
    ("Application", "MyApp"),
    ("Version", "1.0.0"),
    ("Status", "Running"),
    ("Uptime", "2 hours 15 minutes")
}, keyStyle: ConsoleThemes.Dark.Info);
```

### Boxes and Rules

```csharp
// Titled box
writer.WriteTitledBox("System Information", new[]
{
    "OS: Windows 11",
    "CPU: Intel i7-12700K", 
    "Memory: 32 GB",
    "Disk: 1 TB SSD"
}, ConsoleThemes.Dark);

// Simple rule/separator
writer.WriteRule("Configuration", ConsoleThemes.Dark.Info);

// Horizontal line
writer.WriteHorizontalRule('═', ConsoleThemes.Dark.Hint);
```

### Wrapped Text

```csharp
// Automatic text wrapping
writer.WriteWrapped(
    "This is a very long line of text that will be automatically wrapped " +
    "to fit within the terminal width while preserving word boundaries.",
    maxWidth: 80);

// Indented wrapped text
writer.WriteWrappedIndented(
    "This text will be wrapped and indented, useful for displaying " +
    "detailed descriptions or help text in a formatted way.",
    indent: 4, maxWidth: 80);
```

## PowerShell Integration

### Available Cmdlets

EasyCLI provides ready-to-use PowerShell cmdlets:

```powershell
# Write-Message (alias: Show-Message)
Write-Message "Success!" -Style Success
Write-Message "Warning!" -Style Warning
Write-Message "Error!" -Style Error
Show-Message "Info!" -Style Info

# Write-Rule  
Write-Rule "Section Header" -Style Info

# Write-TitledBox
Write-TitledBox -Title "System Info" -Content @("OS: Windows", "CPU: Intel") -Theme Dark

# Read-Choice
$env = Read-Choice "Environment?" @("Dev", "Test", "Prod")
```

### Pipeline Support

```powershell
# Pipeline input support
@("File1.txt", "File2.txt") | Write-Message -Style Success

# Object pipeline binding
Get-Process | Select-Object Name, CPU | Write-Message -Style Info

# PassThru support
$result = "Processing..." | Write-Message -Style Info -PassThru
```

### Module Import

```powershell
# Import the module
Import-Module EasyCLI

# Check available commands
Get-Command -Module EasyCLI

# Get help for specific cmdlet
Get-Help Write-Message -Full
```

## Plain Mode Support

For automation and scripting, EasyCLI supports plain mode that strips all formatting:

```csharp
// Enable plain mode globally
var writer = new ConsoleWriter(enableColors: false);

// Or use ConsoleWriterFactory for environment detection
var writer = ConsoleWriterFactory.Create(respectEnvironment: true);

// Plain mode removes:
// - All ANSI color codes
// - Decorative symbols (✓, ⚠, ✗, etc.)
// - Box drawing characters
// - Styling and formatting
```

## Error Handling

### Graceful Degradation

```csharp
try
{
    // Attempt styled output
    writer.WriteSuccessLine("✓ Operation completed", ConsoleThemes.Dark);
}
catch (Exception ex)
{
    // Fallback to plain text
    Console.WriteLine("Operation completed");
    
    // Log the styling error if needed
    Debug.WriteLine($"Styling error: {ex.Message}");
}
```

### Environment Detection

```csharp
// Check terminal capabilities
if (writer.SupportsColor)
{
    writer.WriteWithStyle("Colorful output!", ConsoleStyles.Green);
}
else
{
    writer.WriteLine("Plain output");
}

// Check if output is redirected
if (writer.IsOutputRedirected)
{
    // Adjust output for file/pipe redirection
    writer.WriteLine("Data for processing");
}
else
{
    // Interactive terminal output
    writer.WriteInfoLine("Interactive mode", ConsoleThemes.Dark);
}
```

## Best Practices

### Performance Tips

```csharp
// Batch writes for better performance
using (writer.BeginBatch())
{
    for (int i = 0; i < 1000; i++)
    {
        writer.WriteInfoLine($"Processing item {i}", ConsoleThemes.Dark);
    }
} // Automatically flushes at end

// Avoid frequent style changes
var style = ConsoleThemes.Dark.Success;
writer.WriteWithStyle("Item 1", style);
writer.WriteWithStyle("Item 2", style);
writer.WriteWithStyle("Item 3", style);
```

### Accessibility

```csharp
// Use high contrast theme for better visibility
var theme = ConsoleThemes.HighContrast;

// Provide text alternatives to symbols
writer.WriteSuccessLine("SUCCESS: Operation completed", theme);
// Instead of just: writer.WriteSuccessLine("✓", theme);

// Support plain mode for screen readers
var plainMode = Environment.GetEnvironmentVariable("SCREEN_READER") != null;
var writer = new ConsoleWriter(enableColors: !plainMode);
```

### Cross-Platform Compatibility

```csharp
// Use Path.Combine for file paths
var configPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
    ".myapp", "config.json");

// Respect platform conventions
var theme = Environment.OSVersion.Platform == PlatformID.Win32NT
    ? ConsoleThemes.Light  // Windows often uses light themes
    : ConsoleThemes.Dark;  // Unix systems often use dark themes
```

## Next Steps

- **[CLI Enhancement Features](CLI-Enhancement-Features)** - Professional CLI patterns and configuration
- **[Shell Framework](Shell-Framework)** - Building interactive shells
- **[Output and Scripting](Output-and-Scripting)** - Automation-friendly output formats
- **[API Reference](API-Reference)** - Detailed API documentation