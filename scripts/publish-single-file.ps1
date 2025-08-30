#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Creates single-file executable packages for multiple platforms
.DESCRIPTION
    This script publishes EasyCLI.Demo.Enhanced as self-contained single-file executables
    for Windows, macOS, and Linux platforms.
.PARAMETER OutputDir
    Output directory for the published executables (default: ./dist)
.PARAMETER Version
    Version for the executables (auto-detected from project if not specified)
.PARAMETER Platforms
    Comma-separated list of platforms to build for (default: "win-x64,osx-x64,linux-x64")
.PARAMETER IncludeAot
    Include AOT (Ahead of Time) compilation for smaller, faster executables (requires .NET 8+)
#>

param(
    [string]$OutputDir = "./dist",
    [string]$Version = "",
    [string]$Platforms = "win-x64,osx-x64,linux-x64",
    [switch]$IncludeAot = $false
)

$ErrorActionPreference = "Stop"

Write-Host "🚀 Publishing EasyCLI.Demo.Enhanced as single-file executables..." -ForegroundColor Cyan

# Get project directory
$ProjectDir = "EasyCLI.Demo.Enhanced"
$ProjectFile = "$ProjectDir/EasyCLI.Demo.Enhanced.csproj"

if (!(Test-Path $ProjectFile)) {
    Write-Error "Project file not found: $ProjectFile"
    exit 1
}

# Auto-detect version if not provided
if ([string]::IsNullOrEmpty($Version)) {
    Write-Host "📋 Auto-detecting version from project..." -ForegroundColor Yellow
    
    # Get version from csproj file or use default
    $csprojContent = Get-Content $ProjectFile -Raw
    if ($csprojContent -match '<Version>([^<]+)</Version>') {
        $Version = $matches[1]
    } elseif ($csprojContent -match '<AssemblyVersion>([^<]+)</AssemblyVersion>') {
        $Version = $matches[1]
    } else {
        $Version = "1.0.0"
    }
    
    Write-Host "   Version detected: $Version" -ForegroundColor Green
}

# Ensure output directory exists
if (!(Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

# Parse platforms
$PlatformList = $Platforms -split ','

Write-Host "🎯 Target platforms: $($PlatformList -join ', ')" -ForegroundColor Yellow

foreach ($Platform in $PlatformList) {
    $Platform = $Platform.Trim()
    
    Write-Host ""
    Write-Host "📦 Building for platform: $Platform" -ForegroundColor Cyan
    
    # Determine executable name and extension
    $ExeName = if ($Platform -like "win-*") { "easycli-demo-$Platform.exe" } else { "easycli-demo-$Platform" }
    $PlatformOutputDir = "$OutputDir/single-file/$Platform"
    
    # Ensure platform output directory exists
    if (!(Test-Path $PlatformOutputDir)) {
        New-Item -ItemType Directory -Path $PlatformOutputDir -Force | Out-Null
    }
    
    try {
        # Build publish arguments
        $PublishArgs = @(
            "publish"
            $ProjectFile
            "--configuration", "Release"
            "--runtime", $Platform
            "--self-contained", "true"
            "--output", $PlatformOutputDir
            "--property:PublishSingleFile=true"
            "--property:IncludeNativeLibrariesForSelfExtract=true"
            "--property:PublishTrimmed=false"
            "--property:AssemblyVersion=$Version"
            "--property:FileVersion=$Version"
            "--property:InformationalVersion=$Version"
        )
        
        # Add AOT compilation if requested and supported
        if ($IncludeAot) {
            $PublishArgs += "--property:PublishAot=true"
            Write-Host "   🔥 AOT compilation enabled" -ForegroundColor Yellow
        }
        
        Write-Host "   🔨 Publishing..." -ForegroundColor Yellow
        & dotnet @PublishArgs
        
        if ($LASTEXITCODE -ne 0) {
            Write-Error "Failed to publish for platform: $Platform"
            continue
        }
        
        # Rename executable to standard name
        $PublishedExe = Get-ChildItem "$PlatformOutputDir/*" -Include "*.exe", "EasyCLI.Demo.Enhanced" | Where-Object { !$_.PSIsContainer } | Select-Object -First 1
        if ($PublishedExe) {
            $FinalExePath = Join-Path $PlatformOutputDir $ExeName
            if ($PublishedExe.FullName -ne $FinalExePath) {
                Move-Item $PublishedExe.FullName $FinalExePath -Force
            }
            
            # Get file size
            $FileSize = [math]::Round((Get-Item $FinalExePath).Length / 1MB, 2)
            Write-Host "   ✅ Success: $ExeName (${FileSize}MB)" -ForegroundColor Green
            
            # Create compressed archive
            $ArchiveName = "easycli-demo-$Platform-v$Version.tar.gz"
            $ArchivePath = "$OutputDir/$ArchiveName"
            
            Write-Host "   📦 Creating archive: $ArchiveName" -ForegroundColor Yellow
            
            # Use tar for cross-platform compression
            Push-Location $PlatformOutputDir
            try {
                if (Get-Command tar -ErrorAction SilentlyContinue) {
                    tar -czf (Resolve-Path $ArchivePath -ErrorAction SilentlyContinue) $ExeName
                    if ($LASTEXITCODE -eq 0) {
                        $ArchiveSize = [math]::Round((Get-Item $ArchivePath).Length / 1MB, 2)
                        Write-Host "   ✅ Archive created: $ArchiveName (${ArchiveSize}MB)" -ForegroundColor Green
                    }
                } else {
                    Write-Host "   ⚠️  tar not available, skipping archive creation" -ForegroundColor Yellow
                }
            } finally {
                Pop-Location
            }
        } else {
            Write-Warning "Could not find published executable for $Platform"
        }
        
    } catch {
        Write-Error "Failed to publish for platform ${Platform}: $($_.Exception.Message)"
    }
}

Write-Host ""
Write-Host "📋 Build Summary:" -ForegroundColor Cyan
Write-Host "   Output directory: $OutputDir" -ForegroundColor White
Write-Host "   Platforms built: $($PlatformList -join ', ')" -ForegroundColor White

# List all created files
$CreatedFiles = Get-ChildItem "$OutputDir" -Recurse -File | Where-Object { $_.Name -like "easycli-demo*" }
if ($CreatedFiles) {
    Write-Host "   Created files:" -ForegroundColor White
    foreach ($File in $CreatedFiles) {
        $RelativePath = $File.FullName.Substring((Resolve-Path $OutputDir).Path.Length + 1)
        $FileSize = [math]::Round($File.Length / 1MB, 2)
        Write-Host "     $RelativePath (${FileSize}MB)" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "🚀 Usage Instructions:" -ForegroundColor Cyan
Write-Host "   # Run the executable directly (no .NET runtime required):" -ForegroundColor Yellow
foreach ($Platform in $PlatformList) {
    $Platform = $Platform.Trim()
    $ExeName = if ($Platform -like "win-*") { "easycli-demo-$Platform.exe" } else { "./easycli-demo-$Platform" }
    Write-Host "   $ExeName  # $Platform" -ForegroundColor White
}

Write-Host ""
Write-Host "🎉 Single-file executable publishing complete!" -ForegroundColor Green