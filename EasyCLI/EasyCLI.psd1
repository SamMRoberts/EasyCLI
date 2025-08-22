@{
    RootModule = 'EasyCLI.dll'
    ModuleVersion = '0.1.0'
    GUID = '00000000-0000-4000-8000-000000000001'
    Author = 'SamMRoberts'
    CompanyName = 'SamMRoberts'
    Copyright = '(c) 2025 Sam'
    Description = 'EasyCLI PowerShell module exposing ANSI styling helpers and cmdlets (Write-EasyMessage, Write-EasyRule, Write-EasyTitledBox).'
    PowerShellVersion = '7.0'
    FunctionsToExport = @()
    # Exported cmdlets (keep in sync with implemented PSCmdlet classes)
    CmdletsToExport = @('Write-Message','Write-Rule','Write-TitledBox','Read-Choice')
    AliasesToExport = @('Select-EasyChoice')
    FormatsToProcess = @()
    PrivateData = @{
        PSData = @{
            Tags = @('cli','ansi','formatting','EasyCLI')
            LicenseUri = 'https://opensource.org/licenses/MIT'
            ProjectUri = 'https://github.com/SamMRoberts/EasyCLI'
        }
    }
    DefaultCommandPrefix = 'Easy'
}
