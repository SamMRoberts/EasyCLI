using System.Diagnostics;
using EasyCLI.Console;
using EasyCLI.Shell.Utilities;

namespace EasyCLI.Shell
{
    /// <summary>
    /// An interactive, persistent CLI shell hosting registered commands and delegating to external processes.
    /// </summary>
    public class CliShell
    {
        private readonly IConsoleReader _reader;
        private readonly IConsoleWriter _writer;
        private readonly ShellOptions _options;
        private readonly Dictionary<string, ICliCommand> _commands = new(StringComparer.OrdinalIgnoreCase);
        private readonly List<string> _history = [];
        private readonly Lock _historyLock = new();

        /// <summary>
        /// Set of reserved command names that cannot be overridden by user commands.
        /// </summary>
        public static readonly IReadOnlySet<string> ReservedCommandNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "help",
            "history",
            "pwd",
            "cd",
            "clear",
            "complete",
            "exit",
            "quit",
            "echo",
            "config",
        };

        private static bool IsExitCommand(string line)
        {
            return string.Equals(line, "exit", StringComparison.OrdinalIgnoreCase)
                || string.Equals(line, "quit", StringComparison.OrdinalIgnoreCase);
        }

        private static string[] Tokenize(string line)
        {
            // Simple tokenizer supporting quotes
            List<string> result = [];
            StringBuilder sb = new();
            bool inQuotes = false;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '\"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }
                if (char.IsWhiteSpace(c) && !inQuotes)
                {
                    if (sb.Length > 0)
                    {
                        result.Add(sb.ToString());
                        _ = sb.Clear();
                    }
                    continue;
                }
                _ = sb.Append(c);
            }
            if (sb.Length > 0)
            {
                result.Add(sb.ToString());
            }
            return [.. result];
        }

        /// <summary>
        /// Determines if a command line contains shell operators that require native shell interpretation.
        /// </summary>
        /// <param name="line">The command line to analyze.</param>
        /// <returns>True if the line contains shell operators that need native shell handling.</returns>
        private static bool ContainsShellOperators(string line)
        {
            // Check for common shell operators that require native shell interpretation
            return line.Contains('|') // Pipes
                || line.Contains('>') // Output redirection
                || line.Contains('<') // Input redirection
                || line.Contains("&&") // Command chaining (AND)
                || line.Contains("||") // Command chaining (OR)
                || line.Contains(';') // Command separator
                || line.Contains('&') // Background process
                || line.Contains('$') // Variable expansion
                || line.Contains('`') // Command substitution (backticks)
                || line.Contains("$(") // Command substitution (modern)
                || line.Contains('*') // Wildcards
                || line.Contains('?') // Wildcards
                || line.Contains('[') // Character classes
                || line.Contains('~') // Home directory expansion
                || line.Contains(">>") // Append redirection
                || line.Contains("2>") // Stderr redirection
                || line.Contains("2&1"); // Stderr to stdout redirection
        }

        /// <summary>
        /// Shows commands for a specific category.
        /// </summary>
        /// <param name="context">The shell execution context.</param>
        /// <param name="categoryName">The name of the category.</param>
        /// <param name="commands">The commands in the category.</param>
        /// <param name="theme">The console theme to use.</param>
        /// <param name="compact">Whether to use compact display format.</param>
        private static void ShowCategoryCommands(ShellExecutionContext context, string categoryName, IEnumerable<ICliCommand> commands, ConsoleTheme theme, bool compact)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(commands);

            List<ICliCommand> commandList = [.. commands.OrderBy(c => c.Name)];
            if (commandList.Count == 0)
            {
                return;
            }

            context.Writer.WriteInfoLine($"{categoryName}:", theme);

            if (compact && commandList.Count > 6)
            {
                // Show first few commands with "and X more" hint
                List<ICliCommand> displayed = [.. commandList.Take(4)];
                foreach (ICliCommand? cmd in displayed)
                {
                    context.Writer.WriteLine($"  {cmd.Name,-12} {cmd.Description}");
                }
                context.Writer.WriteHintLine($"  ... and {commandList.Count - 4} more", theme);
            }
            else
            {
                // Show all commands, potentially in table format for better organization
                if (commandList.Count <= 8)
                {
                    // Simple list for small categories
                    foreach (ICliCommand? cmd in commandList)
                    {
                        context.Writer.WriteLine($"  {cmd.Name,-12} {cmd.Description}");
                    }
                }
                else
                {
                    // Table format for larger categories
                    string[] headers = ["Command", "Description"];
                    string[][] rows = [.. commandList.Select(cmd => new[] { cmd.Name, cmd.Description })];

                    context.Writer.WriteTableSimple(headers, rows, headerStyle: theme.Heading, borderStyle: theme.Hint);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CliShell"/> class.
        /// </summary>
        /// <param name="reader">The console reader used for input.</param>
        /// <param name="writer">The console writer used for output.</param>
        /// <param name="options">Optional shell options.</param>
        public CliShell(IConsoleReader reader, IConsoleWriter writer, ShellOptions? options = null)
        {
            _reader = reader;
            _writer = writer;
            _options = options ?? new ShellOptions();
            // Synchronously wait for async built-in registration (constructor cannot be async)
            RegisterBuiltInsAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets or sets the current working directory for shell commands.
        /// </summary>
        public string CurrentDirectory { get; set; } = Directory.GetCurrentDirectory();

        /// <summary>
        /// Registers a command instance. Validates against reserved names and duplicate registrations.
        /// </summary>
        /// <param name="command">The command to register.</param>
        /// <returns>The current <see cref="CliShell"/> instance for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when command is null.</exception>
        /// <exception cref="CommandNamingException">Thrown when the command name conflicts with a reserved name or already registered command.</exception>
        public CliShell Register(ICliCommand command)
        {
            ArgumentNullException.ThrowIfNull(command);
            ValidateCommandName(command.Name);
            _commands[command.Name] = command;
            return this;
        }

        /// <summary>
        /// Asynchronously registers a command instance. Validates against reserved names and duplicate registrations.
        /// </summary>
        /// <param name="command">The command to register.</param>
        /// <param name="cancellationToken">Cancellation token (currently unused; provided for future extensibility).</param>
        /// <returns>A completed task.</returns>
        /// <exception cref="ArgumentNullException">Thrown when command is null.</exception>
        /// <exception cref="CommandNamingException">Thrown when the command name conflicts with a reserved name or already registered command.</exception>
        public ValueTask RegisterAsync(ICliCommand command, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(command);
            ValidateCommandName(command.Name);
            _commands[command.Name] = command;
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// Gets a snapshot of registered command names.
        /// </summary>
        public IReadOnlyCollection<string> CommandNames => [.. _commands.Keys];

        /// <summary>
        /// Validates that a command name is not reserved and not already registered.
        /// </summary>
        /// <param name="commandName">The command name to validate.</param>
        /// <exception cref="CommandNamingException">Thrown when the command name is invalid.</exception>
        private void ValidateCommandName(string commandName)
        {
            if (string.IsNullOrWhiteSpace(commandName))
            {
                throw new CommandNamingException("Command name cannot be null, empty, or whitespace.");
            }

            if (ReservedCommandNames.Contains(commandName))
            {
                throw new CommandNamingException($"Command name '{commandName}' is reserved and cannot be overridden. Reserved names: {string.Join(", ", ReservedCommandNames.OrderBy(n => n))}");
            }

            if (_commands.ContainsKey(commandName))
            {
                throw new CommandNamingException($"Command '{commandName}' is already registered. Each command name must be unique.");
            }
        }

        /// <summary>
        /// Internal method to register built-in commands without validation.
        /// </summary>
        /// <param name="command">The command to register.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A completed task.</returns>
        private ValueTask RegisterBuiltInAsync(ICliCommand command, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(command);
            _commands[command.Name] = command;
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// Runs the shell loop until exit/quit or cancellation.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="Task{Int32}"/> representing the asynchronous shell loop operation. Returns 0 on normal exit.</returns>
        public async Task<int> RunAsync(CancellationToken cancellationToken = default)
        {
            ShellExecutionContext ctx = new(this, _writer, _reader);

            while (!cancellationToken.IsCancellationRequested)
            {
                WritePrompt();
                string line = _reader.ReadLine();
                if (line == string.Empty && cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                if (line is null)
                {
                    // EOF
                    break;
                }

                line = line.Trim();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                if (_options.EnableHistory)
                {
                    AddHistory(line);
                }

                if (IsExitCommand(line))
                {
                    break;
                }

                try
                {
                    int code = await DispatchAsync(ctx, line, cancellationToken).ConfigureAwait(false);
                    if (code != 0)
                    {
                        _writer.WriteLine($"(exit code {code})", ConsoleStyles.FgRed);
                    }
                }
                catch (OperationCanceledException)
                {
                    _writer.WriteLine("^C", ConsoleStyles.FgYellow);
                }
                catch (Exception ex)
                {
                    _writer.WriteLine(ex.Message, ConsoleStyles.FgRed);
                }
            }
            return 0;
        }

        private async Task<int> DispatchAsync(ShellExecutionContext ctx, string line, CancellationToken ct)
        {
            // Check if native shell delegation is enabled and the command contains shell operators
            if (_options.EnableNativeShellDelegation && ContainsShellOperators(line))
            {
                return await RunNativeShellAsync(line, ct).ConfigureAwait(false);
            }

            string[] parts = Tokenize(line);
            if (parts.Length == 0)
            {
                return 0;
            }
            string name = parts[0];
            string[] args = parts.Length > 1 ? parts[1..] : [];
            if (_commands.TryGetValue(name, out ICliCommand? cmd))
            {
                return await cmd.ExecuteAsync(ctx, args, ct).ConfigureAwait(false);
            }

            // Check if it might be a mistyped command and suggest alternatives
            string? suggestion = LevenshteinDistance.FindBestMatch(name, _commands.Keys);
            if (suggestion != null)
            {
                _writer.WriteLine($"Unknown command '{name}'", ConsoleStyles.FgRed);
                _writer.WriteLine($"💡 Did you mean '{suggestion}'?", ConsoleStyles.FgYellow);
                return ExitCodes.CommandNotFound;
            }

            // External process fallback
            return await RunExternalAsync(name, args, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes a command line through the native shell to preserve shell operators and functionality.
        /// </summary>
        /// <param name="commandLine">The complete command line to execute.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The exit code from the native shell.</returns>
        private async Task<int> RunNativeShellAsync(string commandLine, CancellationToken ct)
        {
            try
            {
                ProcessStartInfo psi = new()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = false,
                    CreateNoWindow = true,
                    WorkingDirectory = CurrentDirectory,
                };

                // Determine the appropriate shell based on the operating system
                if (OperatingSystem.IsWindows())
                {
                    psi.FileName = "cmd.exe";
                    psi.Arguments = $"/c \"{commandLine}\"";
                }
                else
                {
                    // Use bash on Unix-like systems (Linux, macOS)
                    // If bash is not available, fall back to sh
                    string shell = System.Environment.GetEnvironmentVariable("SHELL") ?? "/bin/bash";
                    if (!File.Exists(shell))
                    {
                        shell = "/bin/sh";
                    }
                    psi.FileName = shell;
                    psi.Arguments = $"-c \"{commandLine}\"";
                }

                using Process proc = new() { StartInfo = psi, EnableRaisingEvents = true };
                if (!proc.Start())
                {
                    _writer.WriteLine($"Unable to start native shell for command: {commandLine}", ConsoleStyles.FgRed);
                    return 127;
                }

                Task<string> stdOutTask = proc.StandardOutput.ReadToEndAsync(ct);
                Task<string> stdErrTask = proc.StandardError.ReadToEndAsync(ct);
                await proc.WaitForExitAsync(ct).ConfigureAwait(false);
                string outText = await stdOutTask.ConfigureAwait(false);
                string errText = await stdErrTask.ConfigureAwait(false);

                if (outText.Length > 0)
                {
                    _writer.Write(outText);
                }
                if (errText.Length > 0)
                {
                    _writer.Write(errText, ConsoleStyles.FgRed);
                }

                return proc.ExitCode;
            }
            catch (Exception ex)
            {
                _writer.WriteLine($"Native shell execution failed for '{commandLine}': {ex.Message}", ConsoleStyles.FgRed);
                return ExitCodes.CommandNotFound;
            }
        }

        private async Task<int> RunExternalAsync(string fileName, string[] args, CancellationToken ct)
        {
            try
            {
                ProcessStartInfo psi = new()
                {
                    FileName = fileName,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = false,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = CurrentDirectory,
                };
                foreach (string a in args)
                {
                    psi.ArgumentList.Add(a);
                }
                using Process proc = new() { StartInfo = psi, EnableRaisingEvents = true };
                if (!proc.Start())
                {
                    _writer.WriteLine($"Unable to start process '{fileName}'", ConsoleStyles.FgRed);
                    return 127;
                }
                Task<string> stdOutTask = proc.StandardOutput.ReadToEndAsync(ct);
                Task<string> stdErrTask = proc.StandardError.ReadToEndAsync(ct);
                await proc.WaitForExitAsync(ct).ConfigureAwait(false);
                string outText = await stdOutTask.ConfigureAwait(false);
                string errText = await stdErrTask.ConfigureAwait(false);
                if (outText.Length > 0)
                {
                    _writer.Write(outText);
                }
                if (errText.Length > 0)
                {
                    _writer.Write(errText, ConsoleStyles.FgRed);
                }
                return proc.ExitCode;
            }
            catch (Exception ex)
            {
                _writer.WriteLine($"Command '{fileName}' failed: {ex.Message}", ConsoleStyles.FgRed);
                return ExitCodes.CommandNotFound;
            }
        }

        private void AddHistory(string line)
        {
            lock (_historyLock)
            {
                _history.Add(line);
                if (_options.HistoryLimit > 0 && _history.Count > _options.HistoryLimit)
                {
                    _history.RemoveAt(0);
                }
            }
        }

        private void WritePrompt()
        {
            if (_options.PromptStyle != null)
            {
                _writer.Write(_options.Prompt, _options.PromptStyle.Value);
            }
            else
            {
                _writer.Write(_options.Prompt);
            }
            _writer.Write(" ");
        }

        private async Task RegisterBuiltInsAsync(CancellationToken cancellationToken = default)
        {
            await RegisterBuiltInAsync(
                new DelegateCommand("help", "Show help or detailed help for a command", (ctx, args, ct) =>
            {
                if (args.Length == 0)
                {
                    ShowCategorizedHelp(ctx);
                    return Task.FromResult(0);
                }

                string cmd = args[0];

                // Handle "help all" for full categorized index
                if (string.Equals(cmd, "all", StringComparison.OrdinalIgnoreCase))
                {
                    ShowFullCategorizedHelp(ctx);
                    return Task.FromResult(0);
                }

                // Handle specific command help
                if (_commands.TryGetValue(cmd, out ICliCommand? target))
                {
                    // Try to trigger help via ExecuteAsync with --help flag
                    return target.ExecuteAsync(ctx, ["--help"], ct);
                }

                ctx.Writer.WriteLine($"Unknown command '{cmd}'", ConsoleStyles.FgRed);

                // Suggest similar commands
                string[] suggestions = [.. _commands.Keys
                    .Where(k => LevenshteinDistance.Calculate(k, cmd) <= 2)
                    .OrderBy(k => LevenshteinDistance.Calculate(k, cmd))
                    .Take(3)];

                if (suggestions.Length > 0)
                {
                    ctx.Writer.WriteLine($"Did you mean: {string.Join(", ", suggestions)}");
                }

                return Task.FromResult(1);
            }),
                cancellationToken).ConfigureAwait(false);

            await RegisterBuiltInAsync(
                new DelegateCommand("history", "Show recent command history", (ctx, args, ct) =>
            {
                int index = 1;
                lock (_historyLock)
                {
                    foreach (string h in _history)
                    {
                        ctx.Writer.WriteLine($"{index,4}  {h}");
                        index++;
                    }
                }
                return Task.FromResult(0);
            }),
                cancellationToken).ConfigureAwait(false);

            await RegisterBuiltInAsync(
                new DelegateCommand("pwd", "Print working directory", (ctx, args, ct) =>
            {
                ctx.Writer.WriteLine(CurrentDirectory);
                return Task.FromResult(0);
            }),
                cancellationToken).ConfigureAwait(false);

            await RegisterBuiltInAsync(
                new DelegateCommand("cd", "Change working directory", (ctx, args, ct) =>
            {
                if (args.Length == 0)
                {
                    ctx.Writer.WriteLine(CurrentDirectory);
                    return Task.FromResult(0);
                }
                string path = args[0];
                string target = Path.IsPathRooted(path) ? path : Path.Combine(CurrentDirectory, path);
                if (Directory.Exists(target))
                {
                    CurrentDirectory = Path.GetFullPath(target);
                    return Task.FromResult(0);
                }
                ctx.Writer.WriteLine($"Directory not found: {path}", ConsoleStyles.FgRed);
                return Task.FromResult(1);
            }),
                cancellationToken).ConfigureAwait(false);

            await RegisterBuiltInAsync(
                new DelegateCommand("clear", "Clear the screen", (ctx, args, ct) =>
            {
                try
                {
                    System.Console.Clear();
                }
                catch
                {
                    // ignore if redirected
                }
                return Task.FromResult(0);
            }),
                cancellationToken).ConfigureAwait(false);

            await RegisterBuiltInAsync(
                new DelegateCommand("complete", "List completions for a prefix", (ctx, args, ct) =>
            {
                string prefix = args.Length == 0 ? string.Empty : args[0];
                string[] matches = [.. _commands.Keys.Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).OrderBy(k => k)];
                foreach (string m in matches)
                {
                    ctx.Writer.WriteLine(m);
                }
                return Task.FromResult(0);
            }),
                cancellationToken).ConfigureAwait(false);

            // Register enhanced CLI example commands
            await RegisterBuiltInAsync(new Commands.EchoCommand(), cancellationToken).ConfigureAwait(false);
            await RegisterBuiltInAsync(new Commands.ConfigCommand(), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Shows a categorized, compact help display.
        /// </summary>
        /// <param name="context">The shell execution context.</param>
        private void ShowCategorizedHelp(ShellExecutionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            ConsoleTheme theme = ConsoleThemes.Dark;
            context.Writer.WriteHeadingLine("Available Commands", theme);
            context.Writer.WriteLine("");

            // Group commands by category
            List<IGrouping<string, ICliCommand>> categorizedCommands = [.. _commands.Values
                .GroupBy(c => c.Category)
                .OrderBy(g => g.Key)];

            // Show essential categories first, then others if there aren't too many total
            string[] essentialCategories = ["Core", "General", "Utility"];
            HashSet<string> shownCategories = [];

            // Show essential categories that exist
            foreach (string category in essentialCategories)
            {
                IGrouping<string, ICliCommand>? categoryGroup = categorizedCommands.FirstOrDefault(g => g.Key == category);
                if (categoryGroup != null)
                {
                    ShowCategoryCommands(context, categoryGroup.Key, categoryGroup, theme, compact: true);
                    _ = shownCategories.Add(category);
                }
            }

            // Show remaining categories if total count is reasonable (≤ 5 categories)
            if (categorizedCommands.Count <= 5)
            {
                foreach (IGrouping<string, ICliCommand> categoryGroup in categorizedCommands)
                {
                    if (!shownCategories.Contains(categoryGroup.Key))
                    {
                        ShowCategoryCommands(context, categoryGroup.Key, categoryGroup, theme, compact: true);
                    }
                }
            }

            // Show hint for full help
            context.Writer.WriteLine("");
            context.Writer.WriteHintLine("Use 'help all' to see all commands organized by category", theme);
            context.Writer.WriteHintLine("Use 'help <command>' for detailed information about a specific command", theme);

            // Add standard footer
            HelpFooter.WriteFooter(context.Writer, theme);
        }

        /// <summary>
        /// Shows the full categorized help index.
        /// </summary>
        /// <param name="context">The shell execution context.</param>
        private void ShowFullCategorizedHelp(ShellExecutionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            ConsoleTheme theme = ConsoleThemes.Dark;
            context.Writer.WriteHeadingLine("Command Index - All Categories", theme);
            context.Writer.WriteLine("");

            // Group commands by category
            List<IGrouping<string, ICliCommand>> categorizedCommands = [.. _commands.Values
                .GroupBy(c => c.Category)
                .OrderBy(g => g.Key)];

            // Show all categories
            bool first = true;
            foreach (IGrouping<string, ICliCommand>? categoryGroup in categorizedCommands)
            {
                if (!first)
                {
                    context.Writer.WriteLine("");
                }
                ShowCategoryCommands(context, categoryGroup.Key, categoryGroup, theme, compact: false);
                first = false;
            }

            // Add standard footer
            context.Writer.WriteLine("");
            HelpFooter.WriteFooter(context.Writer, theme);
        }

        private sealed class DelegateCommand(string name, string description, Func<ShellExecutionContext, string[], CancellationToken, Task<int>> handler, string category = "Core") : ICliCommand
        {
            private readonly Func<ShellExecutionContext, string[], CancellationToken, Task<int>> _handler = handler;
            public string Name { get; } = name;
            public string Description { get; } = description;
            public string Category { get; } = category;
            public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken cancellationToken)
            {
                return _handler(context, args, cancellationToken);
            }
        }
    }
}
