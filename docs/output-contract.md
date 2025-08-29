# EasyCLI Output Contract

This document describes EasyCLI's output stability guarantees and provides guidance for building reliable scripts and automation that depend on command output.

## Overview

EasyCLI provides three output formats to balance human readability with machine-parseable reliability:

- **Table Format** (default) - Human-readable, rich formatting with colors and styling
- **Plain Format** (`--plain`, `-p`) - Script-friendly, stable text output
- **JSON Format** (`--json`, `-j`) - Machine-readable, structured data

## ⚠️ Important for Script Authors

**For automation and scripts, always use `--plain` or `--json` flags.** The default table format may change between versions and includes styling that can interfere with parsing.

```bash
# ✅ Good - Stable for scripts
mycli status --json
mycli config show --plain

# ❌ Avoid - May change between versions  
mycli status
mycli config show
```

## Output Format Specifications

### Table Format (Default)

**Stability**: ⚠️ **No Stability Guarantee**

The default table format is optimized for human readability and may change between versions. Changes may include:
- Column layout and alignment
- Header formatting and borders
- Color schemes and styling
- Spacing and visual elements
- Additional decorative elements

**Use Case**: Interactive terminal use, development, debugging

**Example**:
```
Current Configuration

+----------------+-------------------------+---------+
| Setting        | Value                   | Source  |
+----------------+-------------------------+---------+
| API URL        | https://api.example.com | default |
| Timeout        | 30                      | default |
| Enable Logging | True                    | default |
+----------------+-------------------------+---------+
```

### Plain Format (`--plain`, `-p`)

**Stability**: ✅ **Stable Contract**

Plain format provides a stable, parseable text output suitable for scripts and automation.

**Guarantees**:
- Key-value pairs use consistent `Key: Value` format
- No ANSI escape sequences or colors
- Consistent field ordering within major versions
- Fields separated by `: ` (colon-space)
- One item per line
- No decorative borders or formatting

**Use Case**: Shell scripts, text processing, automation

**Example**:
```
API URL: https://api.example.com
Timeout: 30
Enable Logging: True
Log Level: Info
Output Format: console
Use Colors: True
```

**Parsing Example**:
```bash
# Extract specific values
mycli config show --plain | grep "API URL" | cut -d: -f2 | xargs

# Process all key-value pairs
mycli config show --plain | while IFS=': ' read -r key value; do
  echo "Setting $key has value $value"
done
```

### JSON Format (`--json`, `-j`)

**Stability**: ✅ **Stable Contract**

JSON format provides structured data following JSON specification with stability guarantees.

**Guarantees**:
- Valid JSON output (RFC 7159 compliant)
- Consistent field names within major versions
- Stable data types for each field
- No pretty-printing unless explicitly requested
- UTF-8 encoding
- No extra text outside the JSON structure

**Use Case**: API integration, structured data processing, configuration management

**Example**:
```json
{
  "api_url": "https://api.example.com",
  "timeout": 30,
  "enable_logging": true,
  "log_level": "Info",
  "output_format": "console",
  "use_colors": true
}
```

**Parsing Example**:
```bash
# Using jq for JSON processing
API_URL=$(mycli config show --json | jq -r '.api_url')

# Extract multiple values
mycli config show --json | jq -r '.api_url, .timeout'

# Process in scripts
eval $(mycli config show --json | jq -r 'to_entries[] | "CONFIG_\(.key | ascii_upcase)=\(.value)"')
```

## Error Output

All formats send errors to stderr, preserving stdout for data output:

```bash
# Data goes to stdout, errors to stderr
mycli status --json > output.json 2> errors.log

# Combine both streams if needed
mycli status --json 2>&1 | tee combined.log
```

## Exit Codes

EasyCLI follows standard UNIX exit code conventions:

- `0` - Success
- `1` - General error
- `2` - Invalid arguments or usage
- `3` - File not found
- `4` - Permission denied
- `130` - Interrupted by user (Ctrl+C)

**Script Example**:
```bash
#!/bin/bash
if mycli deploy --json --dry-run > deployment.json; then
  echo "Deployment plan valid"
  exit 0
else
  echo "Deployment validation failed with exit code $?"
  exit 1
fi
```

## Environment Considerations

### Color Output

EasyCLI respects standard environment variables for color control:

- `NO_COLOR=1` - Disable all colors (affects table format)
- `FORCE_COLOR=1` - Force colors even when output is redirected

**Script Recommendation**: Set `NO_COLOR=1` to ensure consistent parsing:

```bash
# Disable colors for reliable parsing
NO_COLOR=1 mycli status --plain
```

### CI/CD Environments

EasyCLI automatically detects CI environments and adjusts output:

- Disables interactive prompts
- Reduces visual formatting
- Provides more structured output by default

Common CI environment variables recognized:
- `CI=true`
- `GITHUB_ACTIONS=true`
- `JENKINS_URL`
- `TRAVIS=true`

## Version Compatibility

### Semantic Versioning Promise

EasyCLI follows semantic versioning for output stability:

- **Major version changes** (e.g., 1.x → 2.x) may break output format
- **Minor version changes** (e.g., 1.1 → 1.2) may add new fields but won't remove existing ones
- **Patch version changes** (e.g., 1.1.1 → 1.1.2) guarantee output compatibility

### Recommended Script Patterns

**Defensive Programming**:
```bash
# Check for required fields in JSON
if ! mycli config show --json | jq -e '.api_url' > /dev/null; then
  echo "Error: api_url field not found in config"
  exit 1
fi

# Graceful handling of new fields
mycli status --json | jq -r '.status // "unknown"'
```

**Version Checking**:
```bash
# Verify compatible version
REQUIRED_VERSION="1.0.0"
CURRENT_VERSION=$(mycli --version --plain | cut -d' ' -f2)

if ! version_compare "$CURRENT_VERSION" "$REQUIRED_VERSION"; then
  echo "Error: Requires EasyCLI >= $REQUIRED_VERSION"
  exit 1
fi
```

## Best Practices for Script Authors

### 1. Always Use Explicit Format Flags

```bash
# ✅ Good - Explicit and stable
mycli config show --json
mycli status --plain

# ❌ Bad - Implicit format may change
mycli config show
```

### 2. Handle Errors Properly

```bash
# ✅ Good - Check exit codes
if ! output=$(mycli deploy --json 2>/dev/null); then
  echo "Command failed with exit code $?"
  exit 1
fi

# ✅ Good - Capture both stdout and stderr
{
  output=$(mycli deploy --json 2>&3)
  exit_code=$?
} 3>&2

if [ $exit_code -ne 0 ]; then
  echo "Command failed"
  exit $exit_code
fi
```

### 3. Validate Expected Structure

```bash
# ✅ Good - Validate JSON structure
if ! echo "$output" | jq -e '.status' > /dev/null; then
  echo "Error: Invalid response structure"
  exit 1
fi

# ✅ Good - Check for required fields in plain format  
if ! echo "$output" | grep -q "^Status:"; then
  echo "Error: Status field not found"
  exit 1
fi
```

### 4. Use Appropriate Parsing Tools

```bash
# For JSON - use jq
api_url=$(mycli config show --json | jq -r '.api_url')

# For plain text - use standard UNIX tools
api_url=$(mycli config show --plain | awk -F': ' '/^API URL:/ {print $2}')
```

## Breaking Change Policy

When output format changes are necessary, EasyCLI will:

1. **Announce changes** in release notes and migration guides
2. **Provide transition period** with both old and new formats supported
3. **Increment major version** for breaking changes
4. **Offer migration tools** when possible

## Migration Between Versions

### Checking for Breaking Changes

Before upgrading EasyCLI in production scripts:

1. Review release notes for output format changes
2. Test scripts against new version in staging environment
3. Update scripts to handle new field names or structures
4. Consider pinning to specific version in CI/CD

### Example Migration Pattern

```bash
#!/bin/bash
# Version-aware script example

VERSION=$(mycli --version --plain | cut -d' ' -f2)
MAJOR_VERSION=$(echo "$VERSION" | cut -d'.' -f1)

case "$MAJOR_VERSION" in
  "1")
    # Version 1.x logic
    api_url=$(mycli config show --json | jq -r '.api_url')
    ;;
  "2")
    # Version 2.x logic (hypothetical breaking change)
    api_url=$(mycli config show --json | jq -r '.endpoint_url')
    ;;
  *)
    echo "Unsupported EasyCLI version: $VERSION"
    exit 1
    ;;
esac
```

## Support and Feedback

For questions about output stability or to report parsing issues:

- **Issues**: [GitHub Issues](https://github.com/SamMRoberts/EasyCLI/issues)
- **Documentation**: [GitHub Repository](https://github.com/SamMRoberts/EasyCLI)
- **Discussions**: [GitHub Discussions](https://github.com/SamMRoberts/EasyCLI/discussions)

When reporting output-related issues, please include:
- EasyCLI version (`mycli --version`)
- Command that produced unexpected output
- Expected vs actual output
- Operating system and terminal information