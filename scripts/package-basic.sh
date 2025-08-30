#!/bin/bash
# Basic packaging script for environments without PowerShell
# Provides essential packaging functionality using bash

set -e

# Configuration
OUTPUT_DIR="./dist"
VERSION=""
PROJECT_DIR="EasyCLI.Demo.Enhanced"
PROJECT_FILE="$PROJECT_DIR/EasyCLI.Demo.Enhanced.csproj"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Helper functions
print_header() {
    echo -e "${CYAN}$1${NC}"
}

print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

print_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

# Function to detect version from project file
detect_version() {
    if [ -f "$PROJECT_FILE" ]; then
        # Try to extract version from csproj file
        VERSION=$(grep -o '<Version>[^<]*</Version>' "$PROJECT_FILE" 2>/dev/null | sed 's/<[^>]*>//g' | head -1)
        if [ -z "$VERSION" ]; then
            VERSION=$(grep -o '<AssemblyVersion>[^<]*</AssemblyVersion>' "$PROJECT_FILE" 2>/dev/null | sed 's/<[^>]*>//g' | head -1)
        fi
        if [ -z "$VERSION" ]; then
            VERSION="1.0.0"
        fi
    else
        VERSION="1.0.0"
    fi
    
    # Normalize version for .NET (ensure it has at least 3 parts)
    if [[ ! "$VERSION" =~ ^[0-9]+\.[0-9]+\.[0-9]+ ]]; then
        VERSION="$VERSION.0"
    fi
}

# Function to create .NET Global Tool package
package_dotnet_tool() {
    print_header "📦 Packaging .NET Global Tool..."
    
    # Create temporary enhanced project file for tool packaging
    TEMP_PROJECT_FILE="$PROJECT_DIR/EasyCLI.Demo.Enhanced.Tool.csproj"
    
    cat > "$TEMP_PROJECT_FILE" << EOF
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    
    <!-- Tool Configuration -->
    <PackAsTool>true</PackAsTool>
    <ToolCommandName>easycli-demo</ToolCommandName>
    <PackageId>EasyCLI.Demo.Tool</PackageId>
    <Version>$VERSION</Version>
    <AssemblyVersion>$VERSION</AssemblyVersion>
    <FileVersion>$VERSION</FileVersion>
    
    <!-- Package Metadata -->
    <Title>EasyCLI Demo Tool</Title>
    <Description>A demonstration CLI tool built with EasyCLI framework showcasing modern CLI patterns and best practices</Description>
    <Authors>EasyCLI Contributors</Authors>
    <Company>EasyCLI</Company>
    <Product>EasyCLI Demo Tool</Product>
    <PackageTags>cli;console;easycli;demo;tool</PackageTags>
    <PackageProjectUrl>https://github.com/SamMRoberts/EasyCLI</PackageProjectUrl>
    <RepositoryUrl>https://github.com/SamMRoberts/EasyCLI</RepositoryUrl>
    <RepositoryType>git</RepositoryType>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
    <Copyright>Copyright (c) EasyCLI Contributors</Copyright>
    
    <!-- Build Configuration -->
    <GeneratePackageOnBuild>false</GeneratePackageOnBuild>
    <IncludeSymbols>true</IncludeSymbols>
    <SymbolPackageFormat>snupkg</SymbolPackageFormat>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="../EasyCLI/EasyCLI.csproj" />
  </ItemGroup>

</Project>
EOF
    
    # Clean, build and pack
    dotnet clean "$TEMP_PROJECT_FILE" --configuration Release || true
    dotnet build "$TEMP_PROJECT_FILE" --configuration Release --no-restore
    dotnet pack "$TEMP_PROJECT_FILE" --configuration Release --output "$OUTPUT_DIR" --no-build
    
    # Clean up temporary project file
    rm -f "$TEMP_PROJECT_FILE"
    
    # Find and report package information
    PACKAGE_FILE=$(find "$OUTPUT_DIR" -name "*.nupkg" -type f | head -1)
    if [ -f "$PACKAGE_FILE" ]; then
        PACKAGE_SIZE=$(du -h "$PACKAGE_FILE" | cut -f1)
        print_success ".NET Global Tool package created: $(basename "$PACKAGE_FILE") ($PACKAGE_SIZE)"
        
        print_info "Installation instructions:"
        echo "  dotnet tool install -g EasyCLI.Demo.Tool --add-source \"$(pwd)/$OUTPUT_DIR\""
        echo "  easycli-demo"
    else
        print_error "Failed to create .NET Global Tool package"
        return 1
    fi
}

# Function to create single-file executable for Linux
package_single_file_linux() {
    print_header "🚀 Publishing single-file executable for Linux..."
    
    PLATFORM="linux-x64"
    EXE_NAME="easycli-demo-$PLATFORM"
    PLATFORM_OUTPUT_DIR="$OUTPUT_DIR/single-file/$PLATFORM"
    
    # Ensure platform output directory exists
    mkdir -p "$PLATFORM_OUTPUT_DIR"
    
    # Publish single-file executable
    dotnet publish "$PROJECT_FILE" \
        --configuration Release \
        --runtime "$PLATFORM" \
        --self-contained true \
        --output "$PLATFORM_OUTPUT_DIR" \
        --property:PublishSingleFile=true \
        --property:IncludeNativeLibrariesForSelfExtract=true \
        --property:PublishTrimmed=false \
        --property:AssemblyVersion="$VERSION" \
        --property:FileVersion="$VERSION" \
        --property:InformationalVersion="$VERSION"
    
    # Rename executable to standard name
    if [ -f "$PLATFORM_OUTPUT_DIR/EasyCLI.Demo.Enhanced" ]; then
        mv "$PLATFORM_OUTPUT_DIR/EasyCLI.Demo.Enhanced" "$PLATFORM_OUTPUT_DIR/$EXE_NAME"
    fi
    
    if [ -f "$PLATFORM_OUTPUT_DIR/$EXE_NAME" ]; then
        # Make executable
        chmod +x "$PLATFORM_OUTPUT_DIR/$EXE_NAME"
        
        # Get file size
        FILE_SIZE=$(du -h "$PLATFORM_OUTPUT_DIR/$EXE_NAME" | cut -f1)
        print_success "Linux executable created: $EXE_NAME ($FILE_SIZE)"
        
        # Create compressed archive
        ARCHIVE_NAME="easycli-demo-$PLATFORM-v$VERSION.tar.gz"
        ARCHIVE_PATH="$OUTPUT_DIR/$ARCHIVE_NAME"
        
        if command -v tar >/dev/null 2>&1; then
            (cd "$PLATFORM_OUTPUT_DIR" && tar -czf "../../$ARCHIVE_NAME" "$EXE_NAME")
            if [ -f "$ARCHIVE_PATH" ]; then
                ARCHIVE_SIZE=$(du -h "$ARCHIVE_PATH" | cut -f1)
                print_success "Archive created: $ARCHIVE_NAME ($ARCHIVE_SIZE)"
            fi
        else
            print_warning "tar not available, skipping archive creation"
        fi
        
        print_info "Usage: ./$EXE_NAME"
    else
        print_error "Failed to create Linux executable"
        return 1
    fi
}

# Function to generate basic Homebrew formula
generate_homebrew_formula() {
    print_header "🍺 Generating Homebrew formula..."
    
    ARCHIVE_URL="https://github.com/SamMRoberts/EasyCLI/releases/download"
    MAC_ARCHIVE_PATH="$OUTPUT_DIR/easycli-demo-osx-x64-v$VERSION.tar.gz"
    
    # Calculate SHA256 if archive exists, otherwise use placeholder
    if [ -f "$MAC_ARCHIVE_PATH" ]; then
        if command -v sha256sum >/dev/null 2>&1; then
            SHA256_HASH=$(sha256sum "$MAC_ARCHIVE_PATH" | cut -d' ' -f1)
        elif command -v shasum >/dev/null 2>&1; then
            SHA256_HASH=$(shasum -a 256 "$MAC_ARCHIVE_PATH" | cut -d' ' -f1)
        else
            SHA256_HASH="# SHA256 hash will be calculated during actual release"
            print_warning "sha256sum or shasum not available, using placeholder"
        fi
    else
        SHA256_HASH="# SHA256 hash will be calculated during actual release"
        print_warning "macOS archive not found, using placeholder SHA256"
    fi
    
    # Generate Homebrew formula
    FORMULA_PATH="$OUTPUT_DIR/easycli-demo.rb"
    cat > "$FORMULA_PATH" << EOF
class EasycliDemo < Formula
  desc "A demonstration CLI tool built with EasyCLI framework"
  homepage "https://github.com/SamMRoberts/EasyCLI"
  url "$ARCHIVE_URL/v$VERSION/easycli-demo-osx-x64-v$VERSION.tar.gz"
  sha256 "$SHA256_HASH"
  license "MIT"
  version "$VERSION"

  def install
    bin.install "easycli-demo-osx-x64" => "easycli-demo"
  end

  test do
    # Test that the binary runs and shows version/help
    output = shell_output("#{bin}/easycli-demo --help 2>&1")
    assert_match "EasyCLI", output
  end

  def caveats
    <<~EOS
      EasyCLI Demo has been installed as 'easycli-demo'.
      
      Run 'easycli-demo' to start the interactive CLI shell.
      
      This is a demonstration tool showcasing EasyCLI framework features:
      - Interactive prompts and command handling
      - Rich console output with ANSI styling  
      - Cross-platform CLI best practices
      
      For more information, visit:
      https://github.com/SamMRoberts/EasyCLI
    EOS
  end
end
EOF
    
    print_success "Homebrew formula generated: easycli-demo.rb"
    print_info "Installation: brew install --formula $FORMULA_PATH"
}

# Main execution
main() {
    print_header "🚀 EasyCLI Basic Packaging Script"
    print_header "================================="
    
    # Check if project file exists
    if [ ! -f "$PROJECT_FILE" ]; then
        print_error "Project file not found: $PROJECT_FILE"
        exit 1
    fi
    
    # Detect version
    detect_version
    print_info "Detected version: $VERSION"
    
    # Create output directory
    mkdir -p "$OUTPUT_DIR"
    
    # Execute packaging steps
    START_TIME=$(date +%s)
    
    print_header ""
    print_header "Step 1: .NET Global Tool Packaging"
    if package_dotnet_tool; then
        TOOL_STATUS="✅ Success"
    else
        TOOL_STATUS="❌ Failed"
    fi
    
    print_header ""
    print_header "Step 2: Single-file Executable (Linux)"
    if package_single_file_linux; then
        SINGLE_FILE_STATUS="✅ Success"
    else
        SINGLE_FILE_STATUS="❌ Failed"
    fi
    
    print_header ""
    print_header "Step 3: Homebrew Formula"
    if generate_homebrew_formula; then
        HOMEBREW_STATUS="✅ Success"
    else
        HOMEBREW_STATUS="❌ Failed"
    fi
    
    END_TIME=$(date +%s)
    DURATION=$((END_TIME - START_TIME))
    
    # Summary
    print_header ""
    print_header "🎉 Packaging Complete!"
    print_header "====================="
    print_header ""
    print_header "📊 Results Summary:"
    echo "   .NET Global Tool: $TOOL_STATUS"
    echo "   Single-file Executable: $SINGLE_FILE_STATUS"
    echo "   Homebrew Formula: $HOMEBREW_STATUS"
    print_header ""
    print_info "Total Duration: ${DURATION}s"
    
    # List generated files
    if [ -d "$OUTPUT_DIR" ]; then
        print_header ""
        print_header "📦 Generated Files:"
        find "$OUTPUT_DIR" -type f \( -name "*.nupkg" -o -name "easycli-demo-*" -o -name "*.rb" -o -name "*.tar.gz" \) | while read -r file; do
            relative_path=${file#"$OUTPUT_DIR/"}
            file_size=$(du -h "$file" | cut -f1)
            echo "   📄 $relative_path ($file_size)"
        done
    fi
    
    print_header ""
    print_success "Basic packaging completed successfully!"
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --output-dir)
            OUTPUT_DIR="$2"
            shift 2
            ;;
        --version)
            VERSION="$2"
            shift 2
            ;;
        --help)
            echo "Usage: $0 [options]"
            echo "Options:"
            echo "  --output-dir DIR    Output directory (default: ./dist)"
            echo "  --version VERSION   Package version (auto-detected if not specified)"
            echo "  --help              Show this help message"
            exit 0
            ;;
        *)
            print_error "Unknown option: $1"
            echo "Use --help for usage information"
            exit 1
            ;;
    esac
done

# Run main function
main