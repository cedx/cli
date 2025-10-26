@{
	ModuleVersion = "3.0.0"
	RootModule = "src/Program.psm1"

	Author = "Cédric Belin <cedx@outlook.com>"
	CompanyName = "Cedric-Belin.fr"
	Copyright = "© Cédric Belin"
	Description = "Command line interface of Cédric Belin, full stack developer."
	GUID = "b489d27c-f48e-49b1-b1d4-c99752f2c828"

	AliasesToExport = @()
	CmdletsToExport = @()
	VariablesToExport = @()

	FunctionsToExport = @(
		"Install-Jdk"
		"Install-Node"
		"Install-Php"
		"Optimize-MySqlTable"
		"Set-MySqlCharset"
		"Set-MySqlEngine"
	)

	NestedModules = @(
		"src/Install-Jdk.psm1"
		"src/Install-Node.psm1"
		"src/Install-Php.psm1"
		"src/Optimize-MySqlTable.psm1"
		"src/Set-MySqlCharset.psm1"
		"src/Set-MySqlEngine.psm1"
	)

	PrivateData = @{
		PSData = @{
			LicenseUri = "https://raw.githubusercontent.com/cedx/cli/main/License.md"
			ProjectUri = "https://github.com/cedx/cli"
			ReleaseNotes = "https://github.com/cedx/cli/releases"
			Tags = "cli", "dotnet", "powershell"
		}
	}
}
