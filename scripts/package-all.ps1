#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Comprehensive packaging automation for all distribution methods
.DESCRIPTION
    This script automates the complete packaging process for EasyCLI.Demo.Enhanced
    including .NET Global Tool, single-file executables, and Homebrew formula.
.PARAMETER OutputDir
    Output directory for all packages (default: ./dist)
.PARAMETER Version
    Version for all packages (auto-detected from project if not specified)
.PARAMETER SkipClean
    Skip cleaning the output directory before packaging
.PARAMETER SkipTool
    Skip .NET Global Tool packaging
.PARAMETER SkipSingleFile
    Skip single-file executable publishing
.PARAMETER SkipHomebrew
    Skip Homebrew formula generation
.PARAMETER Platforms
    Comma-separated list of platforms for single-file publishing (default: "win-x64,osx-x64,linux-x64")
#>

param(
    [string]$OutputDir = "./dist",
    [string]$Version = "",
    [switch]$SkipClean = $false,
    [switch]$SkipTool = $false,
    [switch]$SkipSingleFile = $false,
    [switch]$SkipHomebrew = $false,
    [string]$Platforms = "win-x64,osx-x64,linux-x64"
)

$ErrorActionPreference = "Stop"

Write-Host "🚀 EasyCLI Distribution Packaging Automation" -ForegroundColor Magenta
Write-Host "=============================================" -ForegroundColor Magenta

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

Write-Host ""
Write-Host "📋 Packaging Configuration:" -ForegroundColor Cyan
Write-Host "   Version: $Version" -ForegroundColor White
Write-Host "   Output Directory: $OutputDir" -ForegroundColor White
Write-Host "   Platforms: $Platforms" -ForegroundColor White
Write-Host "   .NET Global Tool: $(!$SkipTool)" -ForegroundColor White
Write-Host "   Single-file Executables: $(!$SkipSingleFile)" -ForegroundColor White
Write-Host "   Homebrew Formula: $(!$SkipHomebrew)" -ForegroundColor White

# Clean output directory if requested
if (!$SkipClean) {
    Write-Host ""
    Write-Host "🧹 Cleaning output directory..." -ForegroundColor Yellow
    if (Test-Path $OutputDir) {
        Remove-Item $OutputDir -Recurse -Force
    }
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
    Write-Host "   ✅ Output directory cleaned" -ForegroundColor Green
}

# Ensure output directory exists
if (!(Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

$StartTime = Get-Date
$Results = @()

# Step 1: .NET Global Tool Packaging
if (!$SkipTool) {
    Write-Host ""
    Write-Host "1️⃣ Packaging .NET Global Tool..." -ForegroundColor Cyan
    try {
        & "./scripts/package-dotnet-tool.ps1" -OutputDir $OutputDir -Version $Version
        $Results += @{ Step = ".NET Global Tool"; Status = "✅ Success"; Error = $null }
        Write-Host "   ✅ .NET Global Tool packaging completed" -ForegroundColor Green
    } catch {
        $Results += @{ Step = ".NET Global Tool"; Status = "❌ Failed"; Error = $_.Exception.Message }
        Write-Host "   ❌ .NET Global Tool packaging failed: $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host ""
    Write-Host "1️⃣ Skipping .NET Global Tool packaging..." -ForegroundColor Yellow
    $Results += @{ Step = ".NET Global Tool"; Status = "⏭️  Skipped"; Error = $null }
}

# Step 2: Single-file Executable Publishing
if (!$SkipSingleFile) {
    Write-Host ""
    Write-Host "2️⃣ Publishing single-file executables..." -ForegroundColor Cyan
    try {
        & "./scripts/publish-single-file.ps1" -OutputDir $OutputDir -Version $Version -Platforms $Platforms
        $Results += @{ Step = "Single-file Executables"; Status = "✅ Success"; Error = $null }
        Write-Host "   ✅ Single-file executable publishing completed" -ForegroundColor Green
    } catch {
        $Results += @{ Step = "Single-file Executables"; Status = "❌ Failed"; Error = $_.Exception.Message }
        Write-Host "   ❌ Single-file executable publishing failed: $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host ""
    Write-Host "2️⃣ Skipping single-file executable publishing..." -ForegroundColor Yellow
    $Results += @{ Step = "Single-file Executables"; Status = "⏭️  Skipped"; Error = $null }
}

# Step 3: Homebrew Formula Generation
if (!$SkipHomebrew) {
    Write-Host ""
    Write-Host "3️⃣ Generating Homebrew formula..." -ForegroundColor Cyan
    try {
        & "./scripts/generate-homebrew-formula.ps1" -OutputDir $OutputDir -Version $Version
        $Results += @{ Step = "Homebrew Formula"; Status = "✅ Success"; Error = $null }
        Write-Host "   ✅ Homebrew formula generation completed" -ForegroundColor Green
    } catch {
        $Results += @{ Step = "Homebrew Formula"; Status = "❌ Failed"; Error = $_.Exception.Message }
        Write-Host "   ❌ Homebrew formula generation failed: $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host ""
    Write-Host "3️⃣ Skipping Homebrew formula generation..." -ForegroundColor Yellow
    $Results += @{ Step = "Homebrew Formula"; Status = "⏭️  Skipped"; Error = $null }
}

$EndTime = Get-Date
$Duration = $EndTime - $StartTime

Write-Host ""
Write-Host "🎉 Packaging Automation Complete!" -ForegroundColor Magenta
Write-Host "=================================" -ForegroundColor Magenta

Write-Host ""
Write-Host "📊 Results Summary:" -ForegroundColor Cyan
foreach ($Result in $Results) {
    Write-Host "   $($Result.Step): $($Result.Status)" -ForegroundColor White
    if ($Result.Error) {
        Write-Host "     Error: $($Result.Error)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "⏱️  Total Duration: $($Duration.Minutes)m $($Duration.Seconds)s" -ForegroundColor Cyan

# Display final package information
if (Test-Path $OutputDir) {
    Write-Host ""
    Write-Host "📦 Generated Packages:" -ForegroundColor Cyan
    
    $PackageFiles = Get-ChildItem $OutputDir -Recurse -File | Where-Object { 
        $_.Extension -in @(".nupkg", ".exe", ".tar.gz", ".rb") -or 
        ($_.Name -like "easycli-demo-*" -and $_.Extension -eq "")
    } | Sort-Object FullName
    
    if ($PackageFiles) {
        foreach ($File in $PackageFiles) {
            $RelativePath = $File.FullName.Substring((Resolve-Path $OutputDir).Path.Length + 1)
            $FileSize = [math]::Round($File.Length / 1MB, 2)
            $Icon = switch ($File.Extension) {
                ".nupkg" { "📦" }
                ".exe" { "🖥️ " }
                ".tar.gz" { "📁" }
                ".rb" { "🍺" }
                default { "⚙️ " }
            }
            Write-Host "   $Icon $RelativePath (${FileSize}MB)" -ForegroundColor White
        }
        
        $TotalSize = [math]::Round(($PackageFiles | Measure-Object Length -Sum).Sum / 1MB, 2)
        Write-Host "   📊 Total size: ${TotalSize}MB" -ForegroundColor Gray
    } else {
        Write-Host "   ⚠️  No package files found" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "🚀 Next Steps:" -ForegroundColor Cyan
Write-Host "   1. Test the generated packages" -ForegroundColor Yellow
Write-Host "   2. Upload archives to GitHub releases" -ForegroundColor Yellow
Write-Host "   3. Publish .NET tool to NuGet.org" -ForegroundColor Yellow
Write-Host "   4. Submit Homebrew formula to homebrew-core" -ForegroundColor Yellow

# Check for any failures
$FailedSteps = $Results | Where-Object { $_.Status -like "*Failed*" }
if ($FailedSteps) {
    Write-Host ""
    Write-Host "⚠️  Some steps failed. Check the error messages above." -ForegroundColor Red
    exit 1
} else {
    Write-Host ""
    Write-Host "🎊 All enabled packaging steps completed successfully!" -ForegroundColor Green
    exit 0
}