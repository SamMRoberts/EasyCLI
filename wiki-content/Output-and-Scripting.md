# Output and Scripting

EasyCLI provides stable output formats for reliable scripting and automation, following the comprehensive [Output Contract](https://github.com/SamMRoberts/EasyCLI/blob/main/docs/output-contract.md).

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

**Stability**: ✅ **Stable - Semantic Versioning Applied**

Plain format provides consistent, parseable text output:
- No ANSI color codes or escape sequences
- Consistent field delimiters and formatting
- No decorative symbols or box drawing characters
- Stable column positions and headers
- Compatible with standard UNIX text processing tools

**Use Case**: Shell scripts, automation, CI/CD pipelines

**Example**:
```bash
$ mycli config show --plain
Setting: API URL
Value: https://api.example.com
Source: default

Setting: Timeout
Value: 30
Source: default

Setting: Enable Logging
Value: True
Source: default
```

### JSON Format (`--json`, `-j`)

**Stability**: ✅ **Stable - Semantic Versioning Applied**

JSON format provides structured, machine-readable output:
- Valid JSON that can be parsed by standard libraries
- Consistent field names and structure
- Nested objects for complex data
- Array format for lists and collections
- Null values for missing or undefined data

**Use Case**: API integration, complex automation, data processing

**Example**:
```bash
$ mycli config show --json
{
  "settings": [
    {
      "name": "API URL",
      "value": "https://api.example.com",
      "source": "default"
    },
    {
      "name": "Timeout", 
      "value": 30,
      "source": "default"
    },
    {
      "name": "Enable Logging",
      "value": true,
      "source": "default"
    }
  ]
}
```

## Error Output

### Error Messages

All error messages are sent to **stderr**, allowing proper separation of data and error information:

```bash
# Data goes to stdout, errors to stderr
mycli process data.json > output.json 2> errors.log
```

### Error Format

**Plain Mode Errors**:
```
Error: Configuration file not found
Suggestion: Run 'mycli config init' to create a default configuration
```

**JSON Mode Errors**:
```json
{
  "error": {
    "message": "Configuration file not found",
    "code": "CONFIG_NOT_FOUND",
    "suggestions": [
      "Run 'mycli config init' to create a default configuration"
    ]
  }
}
```

## Exit Codes

EasyCLI follows standard UNIX exit code conventions:

| Exit Code | Meaning | Description |
|-----------|---------|-------------|
| 0 | Success | Command completed successfully |
| 1 | General Error | Unspecified error occurred |
| 2 | File Not Found | Required file or resource not found |
| 3 | Permission Denied | Insufficient permissions |
| 4 | Invalid Arguments | Invalid command line arguments |
| 5 | Service Unavailable | External service not accessible |
| 130 | User Cancelled | User interrupted operation (Ctrl+C) |

### Using Exit Codes in Scripts

```bash
#!/bin/bash

# Check exit code
if mycli deploy staging --json > deployment.json; then
    echo "Deployment successful"
    # Process deployment.json
else
    exit_code=$?
    echo "Deployment failed with exit code: $exit_code"
    exit $exit_code
fi

# Handle specific exit codes
mycli validate config.json
case $? in
    0) echo "Configuration valid" ;;
    2) echo "Configuration file not found" ;;
    4) echo "Invalid configuration format" ;;
    *) echo "Unexpected error" ;;
esac
```

## Environment Considerations

### Environment Variables

EasyCLI respects standard environment variables:

| Variable | Effect |
|----------|--------|
| `NO_COLOR` | Disables all color output (any value) |
| `FORCE_COLOR` | Forces color output even when redirected |
| `CI` | Detected as non-interactive environment |
| `TERM` | Used for terminal capability detection |

```bash
# Disable colors for parsing
NO_COLOR=1 mycli status --plain

# Force colors in CI
FORCE_COLOR=1 mycli status

# CI detection
CI=1 mycli deploy --yes  # Assumes non-interactive mode
```

### Output Redirection

EasyCLI automatically detects output redirection and adjusts behavior:

```bash
# Interactive terminal - shows colors and progress
mycli process files/

# Redirected to file - plain output automatically
mycli process files/ > results.txt

# Piped to another command - plain output
mycli status | grep "Running"
```

## Version Compatibility

### Semantic Versioning Promise

EasyCLI follows semantic versioning for output stability:

- **Major version changes** (e.g., 1.x → 2.x) may break output format
- **Minor version changes** (e.g., 1.1 → 1.2) may add new fields but won't remove existing ones
- **Patch version changes** (e.g., 1.1.1 → 1.1.2) guarantee output compatibility

### Recommended Script Patterns

```bash
#!/bin/bash
# Version-aware script pattern

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

## Best Practices for Script Authors

### 1. Always Use Explicit Format Flags

```bash
# ✅ Good - Explicit format specification
mycli status --json | jq '.status'
mycli config show --plain | grep "API URL"

# ❌ Avoid - Default format may change
mycli status | jq '.status'  # Will break if default isn't JSON
```

### 2. Handle Errors Properly

```bash
# ✅ Good - Check exit codes and handle stderr
if ! result=$(mycli config show --json 2>/dev/null); then
  echo "Command failed with exit code $?"
  exit 1
fi

# Process the result
api_url=$(echo "$result" | jq -r '.api_url')
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

# For CSV-like data - use cut/awk
status=$(mycli status --plain | cut -d: -f2 | xargs)
```

## Advanced Scripting Examples

### Configuration Management Script

```bash
#!/bin/bash
set -euo pipefail

# Save current config
current_config=$(mycli config show --json)

# Update specific setting
mycli config set api-url "https://new-api.example.com" --json

# Verify change
new_url=$(mycli config show --json | jq -r '.settings[] | select(.name=="API URL") | .value')

if [[ "$new_url" == "https://new-api.example.com" ]]; then
    echo "Configuration updated successfully"
else
    echo "Configuration update failed"
    # Restore previous config
    echo "$current_config" | mycli config import --json
    exit 1
fi
```

### Batch Processing Script

```bash
#!/bin/bash

# Process multiple files with progress tracking
files=(*.txt)
total=${#files[@]}
processed=0
failed=0

for file in "${files[@]}"; do
    echo "Processing $file ($((processed + 1))/$total)..."
    
    if mycli process "$file" --json > "${file%.txt}.json" 2>/dev/null; then
        ((processed++))
        echo "✓ Processed $file"
    else
        ((failed++))
        echo "✗ Failed to process $file"
    fi
done

# Summary with proper exit code
echo "Completed: $processed/$total processed, $failed failed"
exit $([[ $failed -eq 0 ]] && echo 0 || echo 1)
```

### CI/CD Integration Script

```bash
#!/bin/bash
# CI/CD deployment script

set -euo pipefail

# Ensure non-interactive mode
export CI=1

# Validate configuration
if ! mycli config validate --json > validation.json; then
    echo "Configuration validation failed:"
    jq -r '.errors[]' validation.json
    exit 4
fi

# Deploy with confirmation bypass
if ! mycli deploy production --yes --json > deployment.json; then
    echo "Deployment failed:"
    jq -r '.error.message' deployment.json
    exit 5
fi

# Extract deployment information
deployment_id=$(jq -r '.deployment.id' deployment.json)
deployment_url=$(jq -r '.deployment.url' deployment.json)

echo "Deployment successful!"
echo "Deployment ID: $deployment_id"
echo "URL: $deployment_url"

# Save deployment info for later stages
jq -c '{id: .deployment.id, url: .deployment.url}' deployment.json > deployment-info.json
```

## Breaking Change Policy

When output format changes are necessary, EasyCLI will:

1. **Announce changes** in release notes and migration guides
2. **Provide transition period** with both old and new formats supported
3. **Increment major version** for breaking changes
4. **Offer migration tools** when possible

### Migration Between Versions

#### Checking for Breaking Changes

Before upgrading EasyCLI in production scripts:

1. Review release notes for output format changes
2. Test scripts against new version in staging environment
3. Update scripts to handle new field names or structures
4. Consider pinning to specific version in CI/CD

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

## Next Steps

- **[API Reference](API-Reference)** - Detailed class and method documentation
- **[Examples and Tutorials](Examples-and-Tutorials)** - More complex scripting examples
- **[CLI Enhancement Features](CLI-Enhancement-Features)** - Professional CLI patterns