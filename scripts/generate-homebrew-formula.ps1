#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Generates a Homebrew formula for EasyCLI.Demo.Enhanced
.DESCRIPTION
    This script generates a Homebrew formula template for macOS package distribution
    with proper SHA256 hash verification.
.PARAMETER OutputDir
    Output directory for the generated formula (default: ./dist)
.PARAMETER Version
    Version for the formula (auto-detected from project if not specified)
.PARAMETER ArchiveUrl
    Base URL where the release archives will be hosted (for GitHub releases)
.PARAMETER DryRun
    Generate formula without requiring actual archive files (for testing)
#>

param(
    [string]$OutputDir = "./dist",
    [string]$Version = "",
    [string]$ArchiveUrl = "https://github.com/SamMRoberts/EasyCLI/releases/download",
    [switch]$DryRun = $false
)

$ErrorActionPreference = "Stop"

Write-Host "🍺 Generating Homebrew formula for EasyCLI.Demo.Enhanced..." -ForegroundColor Cyan

# Auto-detect version if not provided
if ([string]::IsNullOrEmpty($Version)) {
    Write-Host "📋 Auto-detecting version from project..." -ForegroundColor Yellow
    
    # Get version from csproj file or use default
    $ProjectFile = "EasyCLI.Demo.Enhanced/EasyCLI.Demo.Enhanced.csproj"
    if (Test-Path $ProjectFile) {
        $csprojContent = Get-Content $ProjectFile -Raw
        if ($csprojContent -match '<Version>([^<]+)</Version>') {
            $Version = $matches[1]
        } elseif ($csprojContent -match '<AssemblyVersion>([^<]+)</AssemblyVersion>') {
            $Version = $matches[1]
        } else {
            $Version = "1.0.0"
        }
    } else {
        $Version = "1.0.0"
    }
    
    Write-Host "   Version detected: $Version" -ForegroundColor Green
}

# Ensure output directory exists
if (!(Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

# Calculate SHA256 hash for macOS archive
$MacArchivePath = "$OutputDir/easycli-demo-osx-x64-v$Version.tar.gz"
$Sha256Hash = ""

if (!$DryRun) {
    if (Test-Path $MacArchivePath) {
        Write-Host "📊 Calculating SHA256 hash for macOS archive..." -ForegroundColor Yellow
        $Sha256Hash = (Get-FileHash $MacArchivePath -Algorithm SHA256).Hash.ToLower()
        Write-Host "   SHA256: $Sha256Hash" -ForegroundColor Green
    } else {
        Write-Warning "macOS archive not found: $MacArchivePath"
        Write-Host "   Run publish-single-file.ps1 first to create the archive" -ForegroundColor Yellow
        if (!$DryRun) {
            exit 1
        }
    }
} else {
    $Sha256Hash = "# SHA256 hash will be calculated during actual release"
    Write-Host "   🏃 Dry run mode: Using placeholder SHA256" -ForegroundColor Yellow
}

# Generate Homebrew formula
$FormulaContent = @"
class EasycliDemo < Formula
  desc "A demonstration CLI tool built with EasyCLI framework"
  homepage "https://github.com/SamMRoberts/EasyCLI"
  url "$ArchiveUrl/v$Version/easycli-demo-osx-x64-v$Version.tar.gz"
  sha256 "$Sha256Hash"
  license "MIT"
  version "$Version"

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
"@

# Write formula to file
$FormulaPath = "$OutputDir/easycli-demo.rb"
$FormulaContent | Out-File -FilePath $FormulaPath -Encoding UTF8

Write-Host "✅ Homebrew formula generated successfully!" -ForegroundColor Green
Write-Host "📋 Formula Details:" -ForegroundColor Cyan
Write-Host "   File: $FormulaPath" -ForegroundColor White
Write-Host "   Version: $Version" -ForegroundColor White
Write-Host "   Archive URL: $ArchiveUrl/v$Version/easycli-demo-osx-x64-v$Version.tar.gz" -ForegroundColor White
if (!$DryRun) {
    Write-Host "   SHA256: $Sha256Hash" -ForegroundColor White
}

Write-Host ""
Write-Host "🚀 Installation Instructions:" -ForegroundColor Cyan
Write-Host "   # Add to local Homebrew tap:" -ForegroundColor Yellow
Write-Host "   cp $FormulaPath /usr/local/Homebrew/Library/Taps/homebrew/homebrew-core/Formula/" -ForegroundColor White
Write-Host ""
Write-Host "   # Or install directly from local formula:" -ForegroundColor Yellow
Write-Host "   brew install --formula $FormulaPath" -ForegroundColor White
Write-Host ""
Write-Host "   # Run the tool:" -ForegroundColor Yellow
Write-Host "   easycli-demo" -ForegroundColor White
Write-Host ""
Write-Host "   # Uninstall:" -ForegroundColor Yellow
Write-Host "   brew uninstall easycli-demo" -ForegroundColor White

Write-Host ""
Write-Host "📝 Publishing Notes:" -ForegroundColor Cyan
Write-Host "   - Upload the macOS archive to GitHub releases" -ForegroundColor Yellow
Write-Host "   - Update the SHA256 hash in the formula if needed" -ForegroundColor Yellow
Write-Host "   - Submit to homebrew-core or create your own tap" -ForegroundColor Yellow

Write-Host ""
Write-Host "🎉 Homebrew formula generation complete!" -ForegroundColor Green