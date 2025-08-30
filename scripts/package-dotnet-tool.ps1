#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Packages EasyCLI.Demo.Enhanced as a .NET Global Tool
.DESCRIPTION
    This script packages the EasyCLI.Demo.Enhanced project as a .NET Global Tool
    that can be installed via 'dotnet tool install'.
.PARAMETER OutputDir
    Output directory for the packaged tool (default: ./dist)
.PARAMETER Version
    Version for the package (auto-detected from project if not specified)
#>

param(
    [string]$OutputDir = "./dist",
    [string]$Version = ""
)

$ErrorActionPreference = "Stop"

Write-Host "🔧 Packaging EasyCLI.Demo.Enhanced as .NET Global Tool..." -ForegroundColor Cyan

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

Write-Host "📦 Creating enhanced project configuration for tool packaging..." -ForegroundColor Yellow

# Create a temporary enhanced project file for tool packaging
$TempProjectFile = "$ProjectDir/EasyCLI.Demo.Enhanced.Tool.csproj"
$ToolProjectContent = @"
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
    <Version>$Version</Version>
    <AssemblyVersion>$Version</AssemblyVersion>
    <FileVersion>$Version</FileVersion>
    
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
"@

$ToolProjectContent | Out-File -FilePath $TempProjectFile -Encoding UTF8

try {
    Write-Host "🔨 Building and packaging tool..." -ForegroundColor Yellow
    
    # Clean, restore and build
    dotnet clean "$TempProjectFile" --configuration Release
    dotnet restore "$TempProjectFile"
    dotnet build "$TempProjectFile" --configuration Release --no-restore
    
    # Pack the tool
    dotnet pack "$TempProjectFile" --configuration Release --output $OutputDir --no-build
    
    Write-Host "✅ Tool packaging completed successfully!" -ForegroundColor Green
    
    # Display package information
    $PackageFile = Get-ChildItem "$OutputDir/*.nupkg" | Sort-Object LastWriteTime | Select-Object -Last 1
    if ($PackageFile) {
        $PackageSize = [math]::Round($PackageFile.Length / 1MB, 2)
        Write-Host "📋 Package Details:" -ForegroundColor Cyan
        Write-Host "   Package: $($PackageFile.Name)" -ForegroundColor White
        Write-Host "   Size: ${PackageSize}MB" -ForegroundColor White
        Write-Host "   Location: $($PackageFile.FullName)" -ForegroundColor White
        
        Write-Host ""
        Write-Host "🚀 Installation Instructions:" -ForegroundColor Cyan
        Write-Host "   # Install globally from local package:" -ForegroundColor Yellow
        Write-Host "   dotnet tool install -g EasyCLI.Demo.Tool --add-source `"$((Resolve-Path $OutputDir).Path)`"" -ForegroundColor White
        Write-Host ""
        Write-Host "   # Run the tool:" -ForegroundColor Yellow
        Write-Host "   easycli-demo" -ForegroundColor White
        Write-Host ""
        Write-Host "   # Uninstall:" -ForegroundColor Yellow
        Write-Host "   dotnet tool uninstall -g EasyCLI.Demo.Tool" -ForegroundColor White
    }
    
} finally {
    # Clean up temporary project file
    if (Test-Path $TempProjectFile) {
        Remove-Item $TempProjectFile -Force
    }
}

Write-Host "🎉 .NET Global Tool packaging complete!" -ForegroundColor Green