# API Reference

This page provides a comprehensive reference for the core EasyCLI classes and interfaces.

## Console Classes

### ConsoleWriter

The primary class for writing styled output to the console.

```csharp
public class ConsoleWriter : IConsoleWriter
```

#### Constructors

```csharp
// Default constructor with auto-detection
public ConsoleWriter()

// Explicit control over colors and output
public ConsoleWriter(bool enableColors = true, TextWriter? output = null)
```

#### Key Methods

```csharp
// Basic output
void WriteLine(string text = "")
void Write(string text)

// Styled output  
void WriteWithStyle(string text, ConsoleStyle style)
void WriteSuccessLine(string text, ConsoleTheme theme)
void WriteWarningLine(string text, ConsoleTheme theme)
void WriteErrorLine(string text, ConsoleTheme theme)
void WriteInfoLine(string text, ConsoleTheme theme)
void WriteHintLine(string text, ConsoleTheme theme)

// Rich formatting
void WriteKeyValues(IEnumerable<(string Key, string Value)> pairs, ConsoleStyle? keyStyle = null)
void WriteTableSimple(string[] headers, string[][] rows, ConsoleStyle? headerStyle = null, ConsoleStyle? borderStyle = null)
void WriteTitledBox(string title, IEnumerable<string> content, ConsoleTheme theme)
void WriteRule(string title, ConsoleStyle style)
void WriteWrapped(string text, int maxWidth = 80)

// Batch operations
IDisposable BeginBatch()
```

#### Properties

```csharp
bool SupportsColor { get; }          // Whether terminal supports colors
bool IsOutputRedirected { get; }     // Whether output is redirected
TextWriter Output { get; }           // Underlying TextWriter
```

### ConsoleReader

Class for reading input from the console with support for various input types.

```csharp
public class ConsoleReader : IConsoleReader
```

#### Constructors

```csharp
// Default constructor
public ConsoleReader()

// Explicit input source
public ConsoleReader(TextReader input)
```

#### Key Methods

```csharp
// Basic input
string? ReadLine()
Task<string?> ReadLineAsync()
ConsoleKeyInfo ReadKey(bool intercept = false)

// Input validation
string ReadLineWithValidation(Func<string, bool> validator, string errorMessage)
Task<string> ReadLineWithValidationAsync(Func<string, bool> validator, string errorMessage)
```

#### Properties

```csharp
bool IsInteractive { get; }          // Whether input is from interactive terminal
TextReader Input { get; }            // Underlying TextReader
```

### ConsoleWriterFactory

Factory class for creating appropriately configured ConsoleWriter instances.

```csharp
public static class ConsoleWriterFactory
```

#### Methods

```csharp
// Create writer with environment detection
public static ConsoleWriter Create(bool respectEnvironment = true)

// Create writer for plain mode
public static ConsoleWriter CreatePlain()

// Create writer with explicit configuration
public static ConsoleWriter Create(bool enableColors, TextWriter output)
```

## Styling Classes

### ConsoleStyle

Represents a combination of ANSI styling attributes.

```csharp
public class ConsoleStyle
```

#### Constructors

```csharp
public ConsoleStyle(ConsoleColor? foreground = null, ConsoleColor? background = null, 
                   bool bold = false, bool italic = false, bool underline = false)
```

#### Operators

```csharp
// Combine styles
public static ConsoleStyle operator +(ConsoleStyle left, ConsoleStyle right)

// Implicit conversion from ConsoleColor
public static implicit operator ConsoleStyle(ConsoleColor color)
```

#### Properties

```csharp
ConsoleColor? Foreground { get; }
ConsoleColor? Background { get; }
bool Bold { get; }
bool Italic { get; }
bool Underline { get; }
string AnsiCode { get; }             // Generated ANSI escape sequence
```

### ConsoleStyles

Static class containing predefined styles.

```csharp
public static class ConsoleStyles
```

#### Color Styles

```csharp
public static ConsoleStyle Black { get; }
public static ConsoleStyle Red { get; }
public static ConsoleStyle Green { get; }
public static ConsoleStyle Yellow { get; }
public static ConsoleStyle Blue { get; }
public static ConsoleStyle Magenta { get; }
public static ConsoleStyle Cyan { get; }
public static ConsoleStyle White { get; }
public static ConsoleStyle Gray { get; }
public static ConsoleStyle DarkRed { get; }
// ... other colors
```

#### Background Styles

```csharp
public static ConsoleStyle BgBlack { get; }
public static ConsoleStyle BgRed { get; }
public static ConsoleStyle BgGreen { get; }
// ... other background colors
```

#### Formatting Styles

```csharp
public static ConsoleStyle Bold { get; }
public static ConsoleStyle Italic { get; }
public static ConsoleStyle Underline { get; }
public static ConsoleStyle Reset { get; }
```

### ConsoleTheme

Represents a coordinated set of styles for different message types.

```csharp
public class ConsoleTheme
```

#### Properties

```csharp
ConsoleStyle Success { get; set; }    // For success messages
ConsoleStyle Warning { get; set; }    // For warning messages  
ConsoleStyle Error { get; set; }      // For error messages
ConsoleStyle Info { get; set; }       // For informational messages
ConsoleStyle Hint { get; set; }       // For hints and suggestions
ConsoleStyle Heading { get; set; }    // For headings and titles
```

### ConsoleThemes

Static class containing predefined themes.

```csharp
public static class ConsoleThemes
```

#### Built-in Themes

```csharp
public static ConsoleTheme Dark { get; }          // Default dark theme
public static ConsoleTheme Light { get; }         // Light theme for bright terminals
public static ConsoleTheme HighContrast { get; }  // High contrast for accessibility
```

## Prompt Classes

### StringPrompt

Prompt for string input with validation support.

```csharp
public class StringPrompt : BasePrompt<string>
```

#### Constructors

```csharp
public StringPrompt(string message, IConsoleWriter writer, IConsoleReader reader,
                   string? defaultValue = null, IValidator<string>? validator = null)
```

#### Methods

```csharp
public string GetValue()
public Task<string> GetValueAsync()
```

### IntPrompt

Prompt for integer input with range validation.

```csharp
public class IntPrompt : BasePrompt<int>
```

#### Constructors

```csharp
public IntPrompt(string message, IConsoleWriter writer, IConsoleReader reader,
                int? defaultValue = null, int? minValue = null, int? maxValue = null)
```

### YesNoPrompt

Prompt for boolean yes/no input.

```csharp
public class YesNoPrompt : BasePrompt<bool>
```

#### Constructors

```csharp
public YesNoPrompt(string message, IConsoleWriter writer, IConsoleReader reader,
                  bool? defaultValue = null)
```

### ChoicePrompt<T>

Prompt for selecting from a list of choices.

```csharp
public class ChoicePrompt<T> : BasePrompt<T>
```

#### Constructors

```csharp
public ChoicePrompt(string message, IEnumerable<Choice<T>> choices,
                   IConsoleWriter writer, IConsoleReader reader)
```

### MultiChoicePrompt<T>

Prompt for selecting multiple items from a list.

```csharp
public class MultiChoicePrompt<T> : BasePrompt<List<T>>
```

### HiddenPrompt

Prompt for hidden input (passwords, API keys).

```csharp
public class HiddenPrompt : BasePrompt<string>
```

#### Constructors

```csharp
public HiddenPrompt(string message, IConsoleWriter writer, IConsoleReader reader,
                   string? placeholder = null)
```

### Choice<T>

Represents a choice option in choice prompts.

```csharp
public class Choice<T>
```

#### Constructors

```csharp
public Choice(string displayText, T value)
```

#### Properties

```csharp
string DisplayText { get; }  // Text shown to user
T Value { get; }            // Actual value returned
```

## Validation Classes

### IValidator<T>

Interface for input validation.

```csharp
public interface IValidator<T>
{
    ValidationResult Validate(T value);
}
```

### Built-in Validators

```csharp
public class RequiredValidator : IValidator<string>
public class EmailValidator : IValidator<string>  
public class UrlValidator : IValidator<string>
public class RangeValidator<T> : IValidator<T> where T : IComparable<T>
```

### ValidationResult

Result of validation operation.

```csharp
public class ValidationResult
```

#### Static Methods

```csharp
public static ValidationResult Success()
public static ValidationResult Error(string message)
```

#### Properties

```csharp
bool IsValid { get; }
string? ErrorMessage { get; }
```

## Shell Framework Classes

### CliShell

Interactive shell for executing commands.

```csharp
public class CliShell
```

#### Constructors

```csharp
public CliShell(IConsoleReader reader, IConsoleWriter writer, ShellOptions? options = null)
```

#### Key Methods

```csharp
// Command registration
Task RegisterAsync(ICliCommand command, string[]? aliases = null)
Task UnregisterAsync(string commandName)

// Shell execution
Task RunAsync(CancellationToken cancellationToken = default)
Task<int> ExecuteCommandAsync(string commandLine, CancellationToken cancellationToken = default)

// State management
void SetVariable(string name, object value)
T? GetVariable<T>(string name)
void ClearVariables()
```

#### Properties

```csharp
string WorkingDirectory { get; set; }
IReadOnlyDictionary<string, ICliCommand> Commands { get; }
IReadOnlyList<string> History { get; }
```

### ICliCommand

Interface for shell commands.

```csharp
public interface ICliCommand
{
    string Name { get; }
    string Description { get; }
    string Category { get; }  // Optional, defaults to "General"
    
    Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, 
                          CancellationToken cancellationToken);
}
```

### BaseCliCommand

Base class for commands with help support.

```csharp
public abstract class BaseCliCommand : ICliCommand
```

#### Abstract Members

```csharp
public abstract string Name { get; }
public abstract string Description { get; }
public virtual string Category => "General";
```

#### Virtual Methods

```csharp
protected virtual void ConfigureHelp(CommandHelp help)
protected virtual void ShowHelp(ShellExecutionContext context)
protected virtual void ShowConciseHelp(ShellExecutionContext context)
protected virtual void ShowSuggestion(ShellExecutionContext context, string suggestion)
```

### EnhancedCliCommand

Enhanced command base class with professional CLI features.

```csharp
public abstract class EnhancedCliCommand : BaseCliCommand
```

#### Additional Properties

```csharp
protected Logger? Logger { get; }
protected AppConfig? Config { get; }
protected EnvironmentInfo? Environment { get; }
```

#### Abstract Methods

```csharp
protected abstract Task<int> ExecuteCommand(CommandLineArgs args, 
    ShellExecutionContext context, CancellationToken cancellationToken);
```

### ShellExecutionContext

Context passed to commands during execution.

```csharp
public class ShellExecutionContext
```

#### Properties

```csharp
IConsoleWriter Writer { get; }
IConsoleReader Reader { get; }
string WorkingDirectory { get; set; }
Dictionary<string, object> Variables { get; }
CancellationToken CancellationToken { get; }
```

### ShellOptions

Configuration options for the shell.

```csharp
public class ShellOptions
```

#### Properties

```csharp
string Prompt { get; set; } = ">";
ConsoleStyle? PromptStyle { get; set; }
int HistoryLimit { get; set; } = 100;
string? WelcomeMessage { get; set; }
string[] ExitCommands { get; set; } = { "exit", "quit" };
bool ShowHelpOnStart { get; set; } = true;
bool ClearScreenOnStart { get; set; } = false;
bool AllowExternalCommands { get; set; } = false;
int ExternalCommandTimeout { get; set; } = 30;
string[]? ExternalCommandAllowList { get; set; }
string[]? ExternalCommandBlockList { get; set; }
bool EnableTabCompletion { get; set; } = true;
CompletionMode CompletionMode { get; set; } = CompletionMode.Commands;
string[]? StartupCommands { get; set; }
```

## CLI Enhancement Classes

### CommandLineArgs

Parsed command line arguments with enhanced features.

```csharp
public class CommandLineArgs
```

#### Properties

```csharp
IReadOnlyList<string> Arguments { get; }    // Positional arguments
IReadOnlyDictionary<string, string?> Options { get; }  // Named options
IReadOnlyList<string> Flags { get; }        // Boolean flags
```

#### Convenience Properties

```csharp
bool IsVerbose { get; }      // --verbose flag
bool IsQuiet { get; }        // --quiet flag  
bool IsDebug { get; }        // --debug flag
bool IsDryRun { get; }       // --dry-run flag
bool IsPlain { get; }        // --plain flag
bool ShowHelp { get; }       // --help flag
```

#### Methods

```csharp
string? GetOption(string name, string? defaultValue = null)
int GetOptionAsInt(string name, int defaultValue = 0)
bool GetOptionAsBool(string name, bool defaultValue = false)
bool HasOption(string name)
bool HasFlag(string name)
```

### Logger

Structured logging with verbosity levels.

```csharp
public class Logger
```

#### Constructors

```csharp
public Logger(LogLevel level, IConsoleWriter writer, ConsoleTheme? theme = null)
```

#### Methods

```csharp
void LogDebug(string message, object? context = null)
void LogVerbose(string message, object? context = null)
void LogInfo(string message, object? context = null)
void LogSuccess(string message, object? context = null)
void LogWarning(string message, object? context = null)
void LogError(string message, object? context = null)
```

#### Properties

```csharp
LogLevel Level { get; }
bool IsDebugEnabled { get; }
bool IsVerboseEnabled { get; }
```

### ConfigManager

Configuration management with hierarchical loading.

```csharp
public static class ConfigManager
```

#### Methods

```csharp
Task<T> LoadConfigAsync<T>() where T : new()
Task SaveConfigAsync<T>(T config, bool global = false)
Task<bool> ConfigExistsAsync(bool global = false)
Task DeleteConfigAsync(bool global = false)
string GetConfigPath(bool global = false)
```

### EnvironmentInfo

Environment detection and information.

```csharp
public class EnvironmentInfo
```

#### Properties

```csharp
Platform Platform { get; }
bool IsInteractive { get; }
bool IsContinuousIntegration { get; }
string? CiProvider { get; }
bool IsGitRepository { get; }
string? GitBranch { get; }
bool IsDockerEnvironment { get; }
bool HasConfigFile { get; }
string? ConfigFile { get; }
```

## Enums

### LogLevel

```csharp
public enum LogLevel
{
    Quiet = 0,
    Normal = 1,
    Verbose = 2,
    Debug = 3
}
```

### Platform

```csharp
public enum Platform
{
    Windows,
    Linux,
    MacOS,
    Unknown
}
```

### CompletionMode

```csharp
[Flags]
public enum CompletionMode
{
    None = 0,
    Commands = 1,
    Files = 2,
    Directories = 4
}
```

## Exception Classes

### EasyCliException

Base exception for EasyCLI-specific errors.

```csharp
public class EasyCliException : Exception
```

### ValidationException

Exception thrown for validation errors.

```csharp
public class ValidationException : EasyCliException
```

### CommandException

Exception thrown for command execution errors.

```csharp
public class CommandException : EasyCliException
```

## Extension Methods

### String Extensions

```csharp
public static class StringExtensions
{
    public static string Truncate(this string value, int maxLength)
    public static string PadCenter(this string value, int totalWidth)
    public static bool IsNullOrWhiteSpace(this string? value)
}
```

### IEnumerable Extensions

```csharp
public static class EnumerableExtensions
{
    public static string JoinStrings<T>(this IEnumerable<T> values, string separator = ", ")
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> values)
}
```

## Usage Examples

### Basic Console Output

```csharp
var writer = new ConsoleWriter();
var theme = ConsoleThemes.Dark;

writer.WriteSuccessLine("Operation completed!", theme);
writer.WriteKeyValues(new[] { ("Status", "OK"), ("Time", "2.3s") });
```

### Interactive Prompts

```csharp
var reader = new ConsoleReader();
var writer = new ConsoleWriter();

var namePrompt = new StringPrompt("Your name", writer, reader);
var name = namePrompt.GetValue();

var choices = new[] 
{
    new Choice<string>("Development", "dev"),
    new Choice<string>("Production", "prod")
};
var envPrompt = new ChoicePrompt<string>("Environment", choices, writer, reader);
var env = envPrompt.GetValue();
```

### Shell Command

```csharp
public class StatusCommand : BaseCliCommand
{
    public override string Name => "status";
    public override string Description => "Show application status";

    protected override Task<int> ExecuteCommand(CommandLineArgs args, 
        ShellExecutionContext context, CancellationToken cancellationToken)
    {
        context.Writer.WriteSuccessLine("Application is running", ConsoleThemes.Dark);
        return Task.FromResult(0);
    }
}
```

## Next Steps

- **[Examples and Tutorials](Examples-and-Tutorials)** - Complete examples and tutorials
- **[Contributing](Contributing)** - Development and contribution guidelines