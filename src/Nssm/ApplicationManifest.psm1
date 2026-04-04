using namespace System.Diagnostics.CodeAnalysis
using namespace System.IO

<#
.SYNOPSIS
	Provides information about a web application.
#>
class ApplicationManifest {

	<#
	.SYNOPSIS
		The application description.
	#>
	[ValidateNotNull()]
	[string] $Description = ""

	<#
	.SYNOPSIS
		The application environment.
	#>
	[ValidateNotNull()]
	[string] $Environment = ""

	<#
	.SYNOPSIS
		The application identifier.
	#>
	[ValidateNotNullOrEmpty()]
	[string] $Id = ""

	<#
	.SYNOPSIS
		The application name.
	#>
	[ValidateNotNullOrEmpty()]
	[string] $Name = ""

	<#
	.SYNOPSIS
		Reads the application manifest located at the specified path.
	.PARAMETER Path
		The path to the manifest file.
	.OUTPUTS
		The application manifest corresponding to the specified file.
	#>
	[SuppressMessage("PSUseDeclaredVarsMoreThanAssignments", "")]
	static [ApplicationManifest] Read([string] $Path) {
		return $discard = switch ((Split-Path $Path -Extension).ToLowerInvariant()) {
			".config" { [ApplicationManifest]::ReadXmlManifest($Path); break }
			".json" { [ApplicationManifest]::ReadJsonManifest($Path); break }
			".psd1" { [ApplicationManifest]::ReadPowerShellManifest($Path); break }
			".xml" { [ApplicationManifest]::ReadXmlManifest($Path); break }
			default { throw [NotSupportedException] "The ""$_"" file format is not supported." }
		}
	}

	<#
	.SYNOPSIS
		Reads the JSON application manifest located at the specified path.
	.PARAMETER Path
		The path to the manifest file.
	.OUTPUTS
		The application manifest corresponding to the specified JSON file.
	#>
	hidden static [ApplicationManifest] ReadJsonManifest([string] $Path) {
		$manifest = Get-Content $Path | ConvertFrom-Json
		return [ApplicationManifest]@{
			Description = $manifest.Description ?? ""
			Environment = $manifest.Environment ?? ""
			Id = $manifest.Id
			Name = $manifest.Name
		}
	}

	<#
	.SYNOPSIS
		Reads the PowerShell application manifest located at the specified path.
	.PARAMETER Path
		The path to the manifest file.
	.OUTPUTS
		The application manifest corresponding to the specified PowerShell file.
	#>
	hidden static [ApplicationManifest] ReadPowerShellManifest([string] $Path) {
		$manifest = Import-PowerShellDataFile $Path
		return [ApplicationManifest]@{
			Description = $manifest.Description ?? ""
			Environment = $manifest.Environment ?? ""
			Id = $manifest.Id
			Name = $manifest.Name
		}
	}

	<#
	.SYNOPSIS
		Reads the XML application manifest located at the specified path.
	.PARAMETER Path
		The path to the manifest file.
	.OUTPUTS
		The application manifest corresponding to the specified XML file.
	#>
	hidden static [ApplicationManifest] ReadXmlManifest([string] $Path) {
		$manifest = ([xml] (Get-Content $Path)).Configuration
		return [ApplicationManifest]@{
			Description = $manifest.Description ?? ""
			Environment = $manifest.Environment ?? ""
			Id = $manifest.Id
			Name = $manifest.Name
		}
	}
}
