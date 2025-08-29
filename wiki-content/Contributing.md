# Contributing

We welcome contributions to EasyCLI! This guide will help you get started with contributing to the project.

## 🚀 Quick Start for Contributors

### Prerequisites

- **.NET 9.0 SDK** - Required for building and testing
- **Git** - For version control
- **Code Editor** - Visual Studio, VS Code, or JetBrains Rider recommended

### Getting Started

```bash
# Fork the repository on GitHub
# Clone your fork
git clone https://github.com/YOUR-USERNAME/EasyCLI.git
cd EasyCLI

# Install .NET 9.0 SDK if needed
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version 9.0.101 --install-dir $HOME/.dotnet
export PATH="$HOME/.dotnet:$PATH"

# Verify .NET version
dotnet --version  # Should show 9.0.101 or later

# Build the project
dotnet build

# Run tests
dotnet test --verbosity minimal

# Format code
dotnet format
```

## 📋 Development Workflow

### 1. Create a Feature Branch

```bash
# Create and switch to a new branch
git checkout -b feature/amazing-feature

# Or for bug fixes
git checkout -b fix/issue-description
```

### 2. Make Your Changes

Follow these guidelines:

- **Follow existing code patterns and conventions**
- **Add tests for new functionality**
- **Update documentation for user-facing changes**
- **Keep changes focused and atomic**

### 3. Test Your Changes

```bash
# Build the project
dotnet build

# Run all tests
dotnet test --verbosity minimal

# Run specific test category
dotnet test --filter "Category=Console"
dotnet test --filter "Category=Prompts"
dotnet test --filter "Category=Shell"

# Test with demo applications
dotnet run --project EasyCLI.Demo/EasyCLI.Demo.csproj
dotnet run --project EasyCLI.Demo.Enhanced/EasyCLI.Demo.Enhanced.csproj
```

### 4. Format Your Code

**CRITICAL**: Code must pass formatting checks or CI will fail.

```bash
# Format all code
dotnet format

# Verify formatting compliance
dotnet format --verify-no-changes
```

### 5. Commit Your Changes

```bash
# Stage your changes
git add .

# Commit with a descriptive message
git commit -m "Add amazing feature that improves user experience"

# Push to your fork
git push origin feature/amazing-feature
```

### 6. Open a Pull Request

1. Go to the [EasyCLI repository](https://github.com/SamMRoberts/EasyCLI)
2. Click "New Pull Request"
3. Select your branch
4. Fill out the PR template with details about your changes
5. Submit the pull request

## 🎯 Types of Contributions

### 🐛 Bug Reports

When reporting bugs, please include:

- **EasyCLI version** (`dotnet list package` or NuGet package version)
- **.NET version** and target framework
- **Operating system** and terminal details
- **Clear reproduction steps**
- **Expected vs actual behavior**
- **Relevant code samples** or error messages

Use this template:

```markdown
**Bug Description**
A clear description of what the bug is.

**Reproduction Steps**
1. Go to '...'
2. Click on '....'
3. Scroll down to '....'
4. See error

**Expected Behavior**
What you expected to happen.

**Actual Behavior**
What actually happened.

**Environment**
- EasyCLI Version: [e.g. 0.2.0]
- .NET Version: [e.g. 9.0.101]
- OS: [e.g. Windows 11, Ubuntu 22.04]
- Terminal: [e.g. Windows Terminal, iTerm2]

**Additional Context**
Any other context about the problem.
```

### ✨ Feature Requests

For feature requests, please:

1. **Check existing issues** to avoid duplicates
2. **Describe the problem** you're trying to solve
3. **Propose a solution** with examples
4. **Consider backwards compatibility**
5. **Think about the API design**

### 🔧 Code Contributions

We welcome:

- **Bug fixes**
- **New features**
- **Performance improvements**
- **Documentation improvements**
- **Test coverage improvements**
- **Code quality improvements**

## 🏗️ Development Guidelines

### Code Style

EasyCLI follows standard C# conventions:

```csharp
// ✅ Good - Clear naming and structure
public class ConsoleWriter : IConsoleWriter
{
    private readonly bool _enableColors;
    private readonly TextWriter _output;

    public ConsoleWriter(bool enableColors = true, TextWriter? output = null)
    {
        _enableColors = enableColors;
        _output = output ?? Console.Out;
    }

    public void WriteSuccessLine(string text, ConsoleTheme theme)
    {
        WriteWithStyle($"{text}\n", theme.Success);
    }
}

// ❌ Avoid - Poor naming and structure
public class writer
{
    public bool colors;
    public void write(string s, object t) { /* ... */ }
}
```

### Testing Guidelines

1. **Use existing test patterns** (see `EasyCLI.Tests`)
2. **Test both happy path and edge cases**
3. **Mock external dependencies**
4. **Use descriptive test names**
5. **Add at least one test per new feature**

```csharp
[Test]
public void WriteSuccessLine_WithValidText_WritesStyledOutput()
{
    // Arrange
    var output = new StringWriter();
    var writer = new ConsoleWriter(enableColors: true, output);
    var theme = ConsoleThemes.Dark;

    // Act
    writer.WriteSuccessLine("Success message", theme);

    // Assert
    var result = output.ToString();
    Assert.That(result, Contains.Substring("Success message"));
    Assert.That(result, Contains.Substring("\u001b[")); // ANSI escape sequence
}

[Test]
public void WriteSuccessLine_WithColorsDisabled_WritesPlainText()
{
    // Arrange
    var output = new StringWriter();
    var writer = new ConsoleWriter(enableColors: false, output);
    var theme = ConsoleThemes.Dark;

    // Act
    writer.WriteSuccessLine("Success message", theme);

    // Assert
    var result = output.ToString();
    Assert.That(result, Contains.Substring("Success message"));
    Assert.That(result, Does.Not.Contain("\u001b[")); // No ANSI escape sequences
}
```

### Performance Guidelines

1. **Avoid allocations in hot paths**
2. **Use `StringBuilder` for complex string building**
3. **Cache compiled styles when possible**
4. **Use `async`/`await` for I/O operations**

```csharp
// ✅ Good - Efficient string building
public void WriteTable(string[] headers, string[][] rows)
{
    var sb = new StringBuilder();
    
    // Build table structure once
    for (int i = 0; i < rows.Length; i++)
    {
        sb.AppendLine(string.Join(" | ", rows[i]));
    }
    
    _output.Write(sb.ToString());
}

// ❌ Avoid - Multiple allocations
public void WriteTable(string[] headers, string[][] rows)
{
    foreach (var row in rows)
    {
        _output.WriteLine(string.Join(" | ", row)); // Multiple Write calls
    }
}
```

### Documentation Guidelines

1. **Add XML documentation** for public APIs
2. **Include usage examples** in documentation
3. **Update README.md** for significant changes
4. **Add/update wiki pages** for new features

```csharp
/// <summary>
/// Writes a success message with styling based on the provided theme.
/// </summary>
/// <param name="text">The text to write.</param>
/// <param name="theme">The console theme containing success styling.</param>
/// <example>
/// <code>
/// var writer = new ConsoleWriter();
/// var theme = ConsoleThemes.Dark;
/// writer.WriteSuccessLine("Operation completed!", theme);
/// </code>
/// </example>
public void WriteSuccessLine(string text, ConsoleTheme theme)
{
    WriteWithStyle($"{text}\n", theme.Success);
}
```

## 🧪 Testing Your Changes

### Manual Testing

1. **Build and run the demo applications**:
   ```bash
   dotnet run --project EasyCLI.Demo/EasyCLI.Demo.csproj
   dotnet run --project EasyCLI.Demo.Enhanced/EasyCLI.Demo.Enhanced.csproj
   ```

2. **Test different environments**:
   ```bash
   # Test color detection
   NO_COLOR=1 dotnet run --project EasyCLI.Demo/EasyCLI.Demo.csproj
   FORCE_COLOR=1 dotnet run --project EasyCLI.Demo/EasyCLI.Demo.csproj
   
   # Test output redirection
   dotnet run --project EasyCLI.Demo/EasyCLI.Demo.csproj > output.txt
   ```

3. **Test PowerShell integration** (if you have PowerShell):
   ```powershell
   # Import and test PowerShell module
   Import-Module ./EasyCLI/bin/Debug/net9.0/EasyCLI.dll
   Write-Message "Test message" -Style Success
   ```

### Automated Testing

```bash
# Run all tests
dotnet test --verbosity minimal

# Run tests with coverage (if you have tools installed)
dotnet test --collect:"XPlat Code Coverage"

# Run specific test categories
dotnet test --filter "Category=Console"
dotnet test --filter "TestCategory=Prompts"
dotnet test --filter "ClassName~Shell"
```

### CI/CD Validation

Before submitting your PR, ensure it passes the same checks as CI:

```bash
# Build in Release mode
dotnet build --configuration Release

# Run all tests
dotnet test --configuration Release --verbosity minimal

# Verify formatting
dotnet format --verify-no-changes

# Check for any warnings
dotnet build --verbosity normal | grep -i warning
```

## 📚 Project Structure

Understanding the codebase structure:

```
EasyCLI/
├── EasyCLI/                    # Main library
│   ├── Console/                # Console output and styling
│   ├── Prompts/                # Interactive prompts
│   ├── Shell/                  # Interactive shell framework
│   ├── Cmdlets/                # PowerShell cmdlets
│   └── Configuration/          # Configuration management (v0.2.0+)
├── EasyCLI.Tests/              # Unit tests
├── EasyCLI.Demo/               # Basic demo application
├── EasyCLI.Demo.Enhanced/      # Enhanced CLI demo (v0.2.0+)
├── docs/                       # Documentation
└── wiki-content/              # GitHub Wiki content
```

### Key Classes to Understand

- **`ConsoleWriter`** - Core output formatting
- **`ConsoleReader`** - Input handling
- **`ConsoleThemes`** - Built-in styling themes
- **`BasePrompt<T>`** - Base class for prompts
- **`CliShell`** - Interactive shell framework
- **`BaseCliCommand`** - Base class for shell commands
- **`EnhancedCliCommand`** - Enhanced CLI command base (v0.2.0+)

## 🔄 Release Process

### Versioning

EasyCLI follows [Semantic Versioning](https://semver.org/):

- **MAJOR**: Incompatible API changes
- **MINOR**: New functionality (backward compatible)
- **PATCH**: Bug fixes (backward compatible)

### Stability Status

- **Core APIs** (Console, Styling, Prompts): ✅ **Stable**
- **PowerShell Cmdlets**: ✅ **Stable**
- **Shell Framework**: ⚠️ **Experimental** (API may change before 1.0)

### Release Notes

When contributing, consider if your change should be mentioned in release notes:

- **Breaking changes** - Always document
- **New features** - Document with examples
- **Bug fixes** - Document if user-visible
- **Performance improvements** - Document if significant
- **Internal changes** - Usually don't document

## 🎯 Areas Needing Help

We're especially looking for contributions in these areas:

### High Priority

- **📝 Documentation improvements** - Examples, tutorials, API docs
- **🧪 Test coverage expansion** - Edge cases, integration tests
- **🐛 Bug fixes** - See GitHub issues labeled "bug"
- **♿ Accessibility improvements** - Screen reader support, high contrast

### Medium Priority

- **⚡ Performance optimizations** - Reduce allocations, faster rendering
- **🌐 Cross-platform testing** - Linux, macOS compatibility
- **📱 Terminal compatibility** - Support for more terminal types
- **🔧 Developer experience** - Better error messages, debugging

### Future Features

- **🔮 Tab completion enhancements** - Smarter completion logic
- **📈 Progress indicators** - Progress bars, spinners
- **🎨 Advanced styling** - More themes, custom styling
- **🔌 Plugin system** - Extensible command loading

## 🆘 Getting Help

### Where to Ask Questions

- **💬 [GitHub Discussions](https://github.com/SamMRoberts/EasyCLI/discussions)** - General questions, ideas
- **🐛 [GitHub Issues](https://github.com/SamMRoberts/EasyCLI/issues)** - Bug reports, feature requests
- **📧 Email** - See repository contacts for direct communication

### Before Asking

1. **Search existing issues and discussions**
2. **Check the documentation and wiki**
3. **Try to reproduce the issue**
4. **Provide minimal reproduction example**

## 🏆 Recognition

Contributors are recognized in several ways:

- **Author attribution** in commit messages
- **Contributor list** in repository README
- **Release notes** mentions for significant contributions
- **Maintainer status** for consistent, high-quality contributions

### Hall of Fame

We maintain a list of contributors who have made significant impacts:

- **Core Contributors** - Major feature development
- **Documentation Heroes** - Comprehensive docs and examples
- **Bug Hunters** - Finding and fixing critical issues
- **Community Champions** - Helping other users and contributors

## 📜 Code of Conduct

By participating in this project, you agree to abide by our code of conduct:

### Our Standards

- **Be welcoming and inclusive**
- **Be respectful of different viewpoints**
- **Accept constructive criticism gracefully**
- **Focus on what's best for the community**
- **Show empathy towards other community members**

### Unacceptable Behavior

- **Harassment or discriminatory language**
- **Personal attacks or political discussions**
- **Publishing private information without permission**
- **Spam or excessive self-promotion**
- **Any other conduct that could reasonably be considered inappropriate**

### Enforcement

Community leaders are responsible for clarifying and enforcing standards. They have the right to remove, edit, or reject comments, commits, code, issues, and other contributions that don't align with this code of conduct.

## 📞 Contact

For questions about contributing:

- **General Questions**: [GitHub Discussions](https://github.com/SamMRoberts/EasyCLI/discussions)
- **Bug Reports**: [GitHub Issues](https://github.com/SamMRoberts/EasyCLI/issues)
- **Security Issues**: See repository security policy
- **Direct Contact**: See repository for maintainer contact information

---

**Thank you for contributing to EasyCLI!** 🎉

Your contributions help make CLI development easier and more enjoyable for everyone. Whether you're fixing a typo, adding a feature, or helping other users, every contribution matters.