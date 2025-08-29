## EasyCLI AI Integration Instructions

Audience: Large Language Models (LLMs), AI coding agents, and automation frameworks generating or modifying .NET code that uses or extends EasyCLI.

Goal: Ensure correct, idiomatic, minimal‑risk use of EasyCLI when building new functionality (libraries, CLIs, cmdlets, or REPL-style tooling) without re‑inventing features already provided.

---

### 1. Canonical Facts (ALWAYS honor)
1. Target framework: `net9.0` (do NOT downgrade).  
2. Core namespaces: `EasyCLI.Console`, `EasyCLI.Prompts`, `EasyCLI.Shell`, `EasyCLI.Styling`, `EasyCLI.Cmdlets` (if present).  
3. Color rules: Respect `NO_COLOR=1` (force disable) and `FORCE_COLOR=1` (force enable). Avoid raw ANSI escapes—use `ConsoleWriter` + `ConsoleStyles` / `ConsoleThemes`.  
4. Style & analyzers: StyleCop + .NET analyzers active. Keep parameter formatting consistent: all on one line OR one per line. Provide XML docs for public APIs unless explicitly internal/experimental.  
5. Build/test/format pipeline must remain green: run `dotnet build`, `dotnet test --verbosity minimal`, `dotnet format --verify-no-changes` before introducing major edits (CI enforces formatting).  
6. Avoid global singletons—prefer dependency injection patterns or explicit pass‑through of `IConsoleWriter` / `IConsoleReader`.  

---

### 2. Core Abstractions (Prefer Reuse)
| Concern | Use | Notes |
| ------- | --- | ----- |
| Styled output | `IConsoleWriter` (`ConsoleWriter`) | Methods: `Write`, `WriteLine` (styled/un-styled). Extension helpers likely exist (e.g., `WriteErrorLine`). |
| Input reading | `IConsoleReader` (`ConsoleReader`) | Testable; handles redirection. |
| Theming | `ConsoleTheme` + `ConsoleThemes` | Choose Dark/Light/HighContrast or construct custom. |
| Styles | `ConsoleStyles` | Predefined + truecolor builder `ConsoleStyles.TrueColor(r,g,b)`. |
| Prompts | `BasePrompt<T>` + concrete prompts (`StringPrompt`, `IntPrompt`, `ChoicePrompt`, etc.) | Only subclass when behavior truly new. |
| Prompt validation | `IPromptValidator<T>` & implementations in `Prompts/Validators` | Reuse for range/regex etc. |
| Shell (persistent) | `CliShell`, `ShellOptions`, `ICliCommand`, `ShellExecutionContext` | For REPL-like interactive mode. |
| PowerShell cmdlets | Existing cmdlets under `Cmdlets/` | Extend by following existing pattern (derive from `Cmdlet`). |

Never replicate tokenization, history, or external process launching that the shell already implements.

---

### 3. Decision Tree (High-Level)
1. Need one-off styled output? → Use `ConsoleWriter` + appropriate style or theme.  
2. Need interactive user input? → Use existing prompt class; only create new if *no existing one fits*.  
3. Need persistent command loop? → Use `CliShell` (configure via `ShellOptions`; register commands with `Register`/`RegisterAsync`).  
4. Need both batch + interactive mode? → Parse startup args first; invoke `shell.RunAsync()` only if `--shell` (or similar) requested and STDIN is a TTY.  
5. Need to run system commands inside shell? → Let `CliShell` fallback to external process; do NOT create parallel process runner.  
6. Need cross-tool scripting? → Prefer building shell commands (ICliCommand) that compose existing service classes.  

---

### 4. Creating Shell Commands
Implement `ICliCommand`:
```csharp
public sealed class GreetCommand : ICliCommand
{
    public string Name => "greet";
    public string Description => "Greets a user";
    public Task<int> ExecuteAsync(ShellExecutionContext ctx, string[] args, CancellationToken ct)
    {
        string who = args.Length > 0 ? args[0] : "world";
        ctx.Writer.WriteLine($"Hello {who}!", ConsoleStyles.FgGreen);
        return Task.FromResult(0);
    }
}
```
Register before `RunAsync`:
```csharp
var shell = new CliShell(reader, writer, new ShellOptions { Prompt = "myapp>" });
shell.Register(new GreetCommand());
await shell.RunAsync();
```
Return non-zero codes for failure; shell displays `(exit code X)` automatically.

**IMPORTANT - Reserved Command Names**: EasyCLI protects built-in commands by preventing registration of commands with reserved names. The following names are reserved and will throw `CommandNamingException` if used:
- `help` - Show help or detailed help for a command
- `history` - Show recent command history  
- `pwd` - Print working directory
- `cd` - Change working directory
- `clear` - Clear the screen
- `complete` - List completions for a prefix
- `exit` - Exit the shell (implicit, not registered but reserved)
- `quit` - Quit the shell (implicit, not registered but reserved)
- `echo` - Built-in echo command with enhanced features
- `config` - Built-in configuration management command

Additionally, duplicate command names (case-insensitive) will throw `CommandNamingException`. Choose unique, non-reserved command names for your custom commands.

Avoid: Async void, blocking waits inside handlers, manual Console.* calls (use injected writer).

---

### 5. Adding Prompts
Use existing prompts:
```csharp
var name = new StringPrompt("Name", writer, reader, @default: "Anon").GetValue();
var age = new IntPrompt("Age", writer, reader).GetValue();
```
Validation example:
```csharp
var pct = new IntPrompt("Percent", writer, reader, validator: new IntRangeValidator(0,100)).GetValue();
```
Only subclass `BasePrompt<T>` when input shape or UX (multi-line, structured) differs materially.

---

### 6. Styling & Themes
Prefer semantic colors over raw RGB unless necessary:
```csharp
writer.WriteLine("Success", ConsoleStyles.FgGreen);
writer.WriteLine("Critical", ConsoleStyles.FgRed);
```
Custom theme override example:
```csharp
var themed = EasyCLI.ConsoleThemes.Dark with { Warning = ConsoleStyles.FgMagenta };
writer.WriteWarningLine("Magenta warning", themed);
```
Respect environment toggles NO_COLOR / FORCE_COLOR automatically (handled in `ConsoleWriter`). Do not reimplement detection logic.

---

### 7. External Process Execution (Within Shell)
`CliShell` already provides external fallback when a token doesn’t match a registered command. Do **not** write a duplicate runner. To run a process programmatically inside a custom command, mirror logic: use `ProcessStartInfo`, redirect output, style stderr red, honor `ShellExecutionContext.CurrentDirectory`.

---

### 8. Error Handling & Cancellation
Patterns:
- Catch broad exceptions at *shell* level only.  
- Inside commands, validate input early; return exit code 2 for usage errors (optional convention).  
- On `OperationCanceledException`, shell prints `^C` (yellow). Do not add extra noise.  
- Avoid leaking stack traces unless debug mode is explicitly added (future extension point).  

---

### 9. Logging Integration (Optional)
Bridge `ILogger` → `IConsoleWriter` mapping levels to styles. Do not embed logging frameworks into shell core. Keep adapter separate so base library remains lean.

---

### 10. Performance & Allocation Guidance
- Hot path = tokenization + dispatch; keep O(n) over input length.  
- Avoid LINQ in tight loops unless clarity outweighs negligible overhead.  
- Use `StringBuilder` locally; do NOT static-cache across threads.  
- Truecolor styles only when needed (basic 16 often sufficient).  

---

### 11. Testing Guidelines
1. Use existing test patterns (see `EasyCLI.Tests`).  
2. For shell commands: feed scripted input via `StringReader`; capture output with `StringWriter`.  
3. Assert absence/presence of ANSI sequences using stable markers (prefix `\u001b[` when colors enabled).  
4. Add *at least* one happy path + one edge case per new command or prompt.  
5. No sleeps; rely on deterministic outputs.  

Minimal shell command test pattern:
```csharp
var input = new StringReader("greet Sam\nexit\n");
var output = new StringWriter();
var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors:false, output));
shell.Register(new GreetCommand());
await shell.RunAsync();
Assert.Contains("Hello Sam!", output.ToString());
```

---

### 12. Versioning & Packaging
- Workflow sets `Version` / `PackageVersion` explicitly during CI pack. Do not hardcode versions in code.  
- For additional metadata: prefer `InformationalVersion` property passed at pack time.  
- Do not add auto-updating attributes inside source.  

---

### 13. Extension Scenarios
| Scenario | Recommended Approach |
|----------|---------------------|
| Add new command set | Implement each as `ICliCommand`, register pre-run. |
| Add feature flag | Inject via DI/config object passed to command constructor. |
| Multi-tenant prompts | Wrap `IConsoleWriter` with decorator that prefixes context. |
| Themed output per user preference | Accept `ConsoleTheme` parameter; fallback to `ConsoleThemes.Dark`. |
| Batch mode + interactive fallback | Run batch function; if no args consumed & TTY interactive, start `CliShell`. |

---

### 14. Prohibited / Discouraged Patterns
- Direct use of `System.Console` outside of ConsoleWriter/Reader (breaks styling consistency & testability).  
- Duplicating prompt loops (use `BasePrompt<T>`).  
- Re-implementing color environment detection.  
- Spawning threads for simple command execution (prefer async).  
- Storing large state in shell command instances without necessity.  

---

### 15. Self-Check Before Submitting AI-Generated Changes
Checklist (must pass all):
1. Uses existing abstractions (writer, reader, themes).  
2. No raw ANSI escape literals.  
3. New public APIs documented (XML summary).  
4. Test(s) added or updated meaningfully.  
5. Build, tests, formatting succeed locally.  
6. No accidental framework downgrade / multi-target addition.  
7. No unapproved new dependencies.  
8. Prompts only created when interactive input required.  
9. Shell commands return int codes (0 success).  
10. External process execution uses existing pattern / fallback.  

---

### 16. Example Composite Pattern
Register multiple commands sharing a service:
```csharp
public interface ITimeService { DateTime Now(); }
public sealed class TimeService : ITimeService { public DateTime Now() => DateTime.UtcNow; }

public abstract class TimeCommandBase(ITimeService svc) : ICliCommand
{
    protected ITimeService Svc { get; } = svc;
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract Task<int> ExecuteAsync(ShellExecutionContext ctx, string[] args, CancellationToken ct);
}

public sealed class UtcCommand(ITimeService svc) : TimeCommandBase(svc)
{
    public override string Name => "utc";
    public override string Description => "Show UTC time";
    public override Task<int> ExecuteAsync(ShellExecutionContext ctx, string[] args, CancellationToken ct)
    {
        ctx.Writer.WriteLine(Svc.Now().ToString("O"), ConsoleStyles.FgCyan);
        return Task.FromResult(0);
    }
}
```

---

### 17. Future Evolution Notes (Treat as Informational)
Items that may change (avoid hard assumptions):
- Shell completion (possible richer model; current `complete` built-in lists prefix matches).
- History navigation (line editing not yet implemented).  
- PowerShell integration inside `CliShell` (potential runspace embedding).  

Design accordingly: keep extensions loosely coupled.

---

### 18. Escalation & Deviation
If a requirement conflicts with these rules:  
1. Prefer minimal change path using existing abstractions.  
2. Document deviation rationale in PR description and code comments (XML remark).  
3. Add targeted tests demonstrating necessity.  

---

### 19. Minimal Starter Template (For AI Use)
```csharp
using EasyCLI.Console;
using EasyCLI.Shell;

var reader = new ConsoleReader();
var writer = new ConsoleWriter();
var shell = new CliShell(reader, writer, new ShellOptions { Prompt = "sample>" });
shell.Register(new GreetCommand());
await shell.RunAsync();

sealed class GreetCommand : ICliCommand
{
    public string Name => "greet";
    public string Description => "Greets a user";
    public Task<int> ExecuteAsync(ShellExecutionContext ctx, string[] args, CancellationToken ct)
    {
        ctx.Writer.WriteLine($"Hello {(args.Length>0?args[0]:"world")}!", ConsoleStyles.FgGreen);
        return Task.FromResult(0);
    }
}
```

---

### 20. Summary Directive for LLMs
ALWAYS: reuse abstractions, respect environment color rules, keep code testable, avoid re-implementing shell/prompt logic, return int codes, add tests, run build+test+format. When in doubt: smaller change, more reuse.

---

End of EasyCLI AI Integration Instructions.
