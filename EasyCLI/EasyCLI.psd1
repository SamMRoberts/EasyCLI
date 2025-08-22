@{
    RootModule = 'EasyCLI.dll'
    ModuleVersion = '0.1.0'
    GUID = '00000000-0000-4000-8000-000000000001'
    Author = 'SamMRoberts'
    CompanyName = 'SamMRoberts'
    Copyright = '(c) 2025 Sam'
    Description = 'EasyCLI PowerShell module providing styled console output (Write-Message, Write-Rule, Write-TitledBox) and interactive prompts (Read-Choice).'
    PowerShellVersion = '7.0'
    FunctionsToExport = @()
    # Exported cmdlets (keep in sync with implemented PSCmdlet classes)
    CmdletsToExport = @('Write-Message','Write-Rule','Write-TitledBox','Read-Choice')
    AliasesToExport = @('Select-EasyChoice','Show-Message')
    FormatsToProcess = @()
    PrivateData = @{
        PSData = @{
            Tags = @('cli','ansi','formatting','EasyCLI')
            LicenseUri = 'https://opensource.org/licenses/MIT'
            ProjectUri = 'https://github.com/SamMRoberts/EasyCLI'
            ReleaseNotes = 'Added PassThruObject outputs (RuleInfo, TitledBoxInfo), validation attributes, and legacy Show-Message alias.'
        }
    }
    DefaultCommandPrefix = 'Easy'
}
