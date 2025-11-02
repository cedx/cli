@{
	ModuleVersion = "3.1.0"
	PowerShellVersion = "7.5"
	RootModule = "src/Main.psm1"

	Author = "Cédric Belin <cedx@outlook.com>"
	CompanyName = "Cedric-Belin.fr"
	Copyright = "© Cédric Belin"
	Description = "Command line interface of Cédric Belin, full stack developer."
	GUID = "b489d27c-f48e-49b1-b1d4-c99752f2c828"

	AliasesToExport = @()
	CmdletsToExport = @()
	VariablesToExport = @()

	FunctionsToExport = @(
		"Backup-MySqlTable"
		"Get-CliVersion"
		"Install-Jdk"
		"Install-Node"
		"Install-Php"
		"New-NssmService"
		"Optimize-MySqlTable"
		"Remove-NssmService"
		"Restore-MySqlTable"
		"Set-MySqlCharset"
		"Set-MySqlEngine"
	)

	NestedModules = @(
		"src/Backup-MySqlTable.psm1"
		"src/Install-Jdk.psm1"
		"src/Install-Node.psm1"
		"src/Install-Php.psm1"
		"src/New-NssmService.psm1"
		"src/Optimize-MySqlTable.psm1"
		"src/Remove-NssmService.psm1"
		"src/Restore-MySqlTable.psm1"
		"src/Set-MySqlCharset.psm1"
		"src/Set-MySqlEngine.psm1"
	)

	RequiredAssemblies = @(
		"bin/Microsoft.Extensions.DependencyInjection.Abstractions.dll"
		"bin/Microsoft.Extensions.Logging.Abstractions.dll"
		"bin/MySqlConnector.dll"
	)

	PrivateData = @{
		PSData = @{
			LicenseUri = "https://github.com/cedx/cli/blob/main/License.md"
			ProjectUri = "https://github.com/cedx/cli"
			ReleaseNotes = "https://github.com/cedx/cli/releases"
			Tags = "cli", "dotnet", "powershell"
		}
	}
}
