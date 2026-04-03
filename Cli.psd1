@{
	ModuleVersion = "5.0.0"
	PowerShellVersion = "7.6"
	RootModule = "bin/Belin.Cli.dll"

	Author = "Cédric Belin <cedx@outlook.com>"
	CompanyName = "Cedric-Belin.fr"
	Copyright = "© Cédric Belin"
	Description = "A set of command-line tools for common administrative tasks, such as database management, service management and software installation."
	GUID = "b489d27c-f48e-49b1-b1d4-c99752f2c828"

	AliasesToExport = @()
	CmdletsToExport = @()
	VariablesToExport = @()

	FunctionsToExport = @(
		"Backup-MySqlTable"
		"ConvertTo-Encoding"
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
		"src/Get-CliVersion.psm1"
		"src/Deployment/Install-Jdk.psm1"
		"src/Deployment/Install-Node.psm1"
		"src/Deployment/Install-Php.psm1"
		"src/MySql/Backup-MySqlTable.psm1"
		"src/MySql/Optimize-MySqlTable.psm1"
		"src/MySql/Restore-MySqlTable.psm1"
		"src/MySql/Set-MySqlCharset.psm1"
		"src/MySql/Set-MySqlEngine.psm1"
		"src/Nssm/New-NssmService.psm1"
		"src/Nssm/Remove-NssmService.psm1"
		"src/Text/ConvertTo-Encoding.psm1"
	)

	RequiredAssemblies = @(
		"bin/Microsoft.Extensions.DependencyInjection.Abstractions.dll"
		"bin/Microsoft.Extensions.Logging.Abstractions.dll"
		"bin/MySqlConnector.dll"
	)

	RequiredModules = @(
		"Sql"
	)

	PrivateData = @{
		PSData = @{
			LicenseUri = "https://github.com/cedx/cli/blob/main/License.md"
			ProjectUri = "https://github.com/cedx/cli"
			ReleaseNotes = "https://github.com/cedx/cli/releases"
			Tags = "cli", "dotnet", "powershell", "system", "tools"
		}
	}
}
