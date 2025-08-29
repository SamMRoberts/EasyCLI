# EasyCLI Environment Variables

This document provides a comprehensive reference for all environment variables supported by EasyCLI, their purposes, accepted values, and precedence rules.

## Table of Contents

- [Color and Output Control](#color-and-output-control)
- [Configuration Variables](#configuration-variables)
- [Environment Detection](#environment-detection)
- [Path and Directory Control](#path-and-directory-control)
- [CI/CD Integration](#cicd-integration)
- [Terminal and Shell](#terminal-and-shell)
- [Variable Precedence](#variable-precedence)
- [Validation and Warnings](#validation-and-warnings)
- [Examples](#examples)

## Color and Output Control

### `NO_COLOR`
**Purpose**: Disables all ANSI color output globally  
**Values**: Any non-empty value disables colors  
**Default**: Not set (colors enabled)  
**Standard**: [NO_COLOR standard](https://no-color.org/)

```bash
# Disable all colors
export NO_COLOR=1
```

### `FORCE_COLOR`
**Purpose**: Forces colored output even when output is redirected or piped  
**Values**: Any non-empty value forces colors  
**Default**: Not set  
**Precedence**: Overrides `NO_COLOR` when both are set

```bash
# Force colors in CI or when redirected
export FORCE_COLOR=1
```

### `TERM`
**Purpose**: Terminal capability detection  
**Values**: Terminal type identifier (e.g., `xterm-256color`, `dumb`)  
**Default**: Set by terminal  
**Usage**: EasyCLI uses this for color capability detection

```bash
# Indicates a "dumb" terminal with no color support
export TERM=dumb
```

### `COLORTERM`
**Purpose**: Extended color capability detection  
**Values**: Color capability indicators (e.g., `truecolor`)  
**Default**: Set by terminal  
**Usage**: EasyCLI detects truecolor support

```bash
# Indicates truecolor (24-bit) support
export COLORTERM=truecolor
```

## Configuration Variables

EasyCLI supports configuration through environment variables following the `EASYCLI_*` pattern. These variables correspond to properties in the `AppConfig` class and override values from configuration files.

### `EASYCLI_API_URL`
**Purpose**: Default API endpoint URL  
**Values**: Valid URL string  
**Default**: `https://api.example.com`  
**Property**: `AppConfig.ApiUrl`

```bash
export EASYCLI_API_URL="https://prod.api.example.com"
```

### `EASYCLI_TIMEOUT`
**Purpose**: Operation timeout in seconds  
**Values**: Positive integer  
**Default**: `30`  
**Property**: `AppConfig.Timeout`

```bash
export EASYCLI_TIMEOUT=60
```

### `EASYCLI_ENABLE_LOGGING`
**Purpose**: Enable or disable logging  
**Values**: `true`, `false`, `1`, `0`, `yes`, `no`, `on`, `off`  
**Default**: `true`  
**Property**: `AppConfig.EnableLogging`

```bash
export EASYCLI_ENABLE_LOGGING=false
```

### `EASYCLI_LOG_LEVEL`
**Purpose**: Default logging level  
**Values**: `Debug`, `Verbose`, `Info`, `Warning`, `Error`  
**Default**: `Info`  
**Property**: `AppConfig.LogLevel`

```bash
export EASYCLI_LOG_LEVEL=Debug
```

### `EASYCLI_OUTPUT_FORMAT`
**Purpose**: Default output format preference  
**Values**: `console`, `json`, `plain`, `table`  
**Default**: `console`  
**Property**: `AppConfig.OutputFormat`

```bash
export EASYCLI_OUTPUT_FORMAT=json
```

### `EASYCLI_USE_COLORS`
**Purpose**: Color usage preference in configuration  
**Values**: `true`, `false`, `1`, `0`, `yes`, `no`, `on`, `off`  
**Default**: `true`  
**Property**: `AppConfig.UseColors`  
**Note**: Overridden by `NO_COLOR` and `FORCE_COLOR`

```bash
export EASYCLI_USE_COLORS=false
```

## Environment Detection

### `CI`
**Purpose**: Continuous Integration environment detection  
**Values**: Any non-empty value indicates CI environment  
**Default**: Not set  
**Effect**: Enables non-interactive mode, adjusts output formatting

```bash
export CI=true
```

### `DOCKER_CONTAINER`
**Purpose**: Docker container environment detection  
**Values**: Any non-empty value indicates Docker environment  
**Default**: Not set  
**Effect**: Adjusts file system and permission handling

```bash
export DOCKER_CONTAINER=true
```

## Path and Directory Control

### `XDG_CONFIG_HOME`
**Purpose**: XDG Base Directory specification compliance  
**Values**: Absolute path to configuration directory  
**Default**: `~/.config`  
**Usage**: Used for user configuration file location

```bash
export XDG_CONFIG_HOME="/custom/config/path"
```

## CI/CD Integration

EasyCLI automatically detects various CI/CD environments through their standard environment variables:

### GitHub Actions
- `GITHUB_ACTIONS` - GitHub Actions environment
- `GITHUB_RUN_NUMBER` - Build number metadata

### GitLab CI
- `GITLAB_CI` - GitLab CI environment

### Jenkins
- `JENKINS_URL` - Jenkins environment
- `BUILD_NUMBER` - Build number metadata

### Travis CI
- `TRAVIS` - Travis CI environment

### CircleCI
- `CIRCLECI` - CircleCI environment

### AppVeyor
- `APPVEYOR` - AppVeyor environment

### Azure Pipelines
- `AZURE_PIPELINES` - Azure Pipelines environment

## Terminal and Shell

### `SHELL`
**Purpose**: Default shell for interactive features  
**Values**: Path to shell executable  
**Default**: `/bin/bash`  
**Usage**: Used for shell command execution and completion

```bash
export SHELL=/bin/zsh
```

## Variable Precedence

EasyCLI follows a clear precedence order for configuration values:

1. **Command-line flags** (highest precedence)
2. **Environment variables** (`EASYCLI_*`)
3. **Local configuration file** (`.easycli.json`)
4. **User configuration file** (`~/.config/easycli/config.json`)
5. **System configuration file** (`/etc/easycli/config.json`)
6. **Default values** (lowest precedence)

### Color Precedence
Special precedence rules apply to color control:

1. `FORCE_COLOR=1` (forces colors, overrides everything)
2. `NO_COLOR=1` (disables colors)
3. `EASYCLI_USE_COLORS` (configuration preference)
4. Automatic detection (TTY, terminal capabilities)

## Validation and Warnings

### Unknown EASYCLI_* Variables
EasyCLI validates environment variables with the `EASYCLI_` prefix and warns about unknown variables that don't correspond to configuration properties.

**Recognized patterns:**
- `EASYCLI_API_URL`
- `EASYCLI_TIMEOUT`
- `EASYCLI_ENABLE_LOGGING`
- `EASYCLI_LOG_LEVEL`
- `EASYCLI_OUTPUT_FORMAT`
- `EASYCLI_USE_COLORS`

**Warning behavior:**
- Unknown `EASYCLI_*` variables trigger warnings
- Warnings include suggestions for correct variable names
- Warnings can be suppressed with `--quiet` flag
- Warnings are logged but don't affect execution

### Validation Examples
```bash
# This will work correctly
export EASYCLI_API_URL="https://api.example.com"

# This will trigger a warning (typo in variable name)
export EASYCLI_API_ULR="https://api.example.com"
# Warning: Unknown environment variable 'EASYCLI_API_ULR'. Did you mean 'EASYCLI_API_URL'?

# This will trigger a warning (unknown property)
export EASYCLI_UNKNOWN_SETTING="value"
# Warning: Unknown environment variable 'EASYCLI_UNKNOWN_SETTING'. Available variables: EASYCLI_API_URL, EASYCLI_TIMEOUT, ...
```

## Examples

### Development Environment
```bash
# Development configuration
export EASYCLI_API_URL="http://localhost:3000"
export EASYCLI_LOG_LEVEL=Debug
export EASYCLI_ENABLE_LOGGING=true
export EASYCLI_OUTPUT_FORMAT=console
```

### Production Environment
```bash
# Production configuration
export EASYCLI_API_URL="https://prod.api.example.com"
export EASYCLI_TIMEOUT=60
export EASYCLI_LOG_LEVEL=Warning
export EASYCLI_OUTPUT_FORMAT=json
```

### CI/CD Environment
```bash
# CI environment (often set automatically)
export CI=true
export NO_COLOR=1
export EASYCLI_OUTPUT_FORMAT=json
export EASYCLI_LOG_LEVEL=Info
```

### Automated Scripting
```bash
# Disable interactivity and colors for scripting
export NO_COLOR=1
export EASYCLI_OUTPUT_FORMAT=json
export EASYCLI_LOG_LEVEL=Error
```

### Custom Configuration Location
```bash
# Use custom config directory
export XDG_CONFIG_HOME="/opt/myapp/config"
export EASYCLI_API_URL="https://custom.api.example.com"
```

## Getting Help

For more information about environment variables:

```bash
# Show current environment configuration
easycli config env

# Show all configuration sources and precedence
easycli config show --verbose

# Get help for specific commands
easycli config --help
easycli --help
```

## See Also

- [Configuration Management](../README.md#configuration-management)
- [Output Contract](output-contract.md)
- [NO_COLOR Standard](https://no-color.org/)
- [XDG Base Directory Specification](https://specifications.freedesktop.org/basedir-spec/basedir-spec-latest.html)