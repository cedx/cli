using namespace System.Diagnostics.CodeAnalysis
using namespace System.IO
using namespace System.Management.Automation
using namespace System.Runtime.CompilerServices

<#
.SYNOPSIS
	Provides information about a Web application.
#>
class WebApplication {

	<#
	.SYNOPSIS
		The application description.
	#>
	[string] $Description = ""

	<#
	.SYNOPSIS
		The environment name.
	#>
	[string] $Environment = ""

	<#
	.SYNOPSIS
		The application identifier.
	#>
	[string] $Id = ""

	<#
	.SYNOPSIS
		The application name.
	#>
	[string] $Name = ""

	<#
	.SYNOPSIS
		The path to the application root directory.
	#>
	[string] $Path = ""

	<#
	.SYNOPSIS
		The application type.
	#>
	[WebApplicationType] $Type = [WebApplicationType]::Unknown

	<#
	.SYNOPSIS
		Gets the environment variable storing the environment name.
	.OUTPUTS
		The environment variable storing the environment name.
	#>
	[SuppressMessage("PSUseDeclaredVarsMoreThanAssignments", "")]
	[string] EnvironmentVariable() {
		return $discard = switch ($this.Type) {
			([WebApplicationType]::DotNet) { "DOTNET_ENVIRONMENT"; break }
			([WebApplicationType]::Node) { "NODE_ENV"; break }
			([WebApplicationType]::PowerShell) { "PODE_ENVIRONMENT"; break }
			default { throw [SwitchExpressionException] $_ }
		}
	}

	<#
	.SYNOPSIS
		Gets the program used to run this application.
	.OUTPUTS
		The program used to run this application.
	#>
	[SuppressMessage("PSUseDeclaredVarsMoreThanAssignments", "")]
	[ApplicationInfo] Program() {
		$program = switch ($this.Type) {
			([WebApplicationType]::DotNet) { "dotnet"; break }
			([WebApplicationType]::Node) { "node"; break }
			([WebApplicationType]::PowerShell) { "pwsh"; break }
			default { throw [SwitchExpressionException] $_ }
		}

		return Get-Command $program
	}

	<#
	.SYNOPSIS
		Reads the configuration file of the application located in the specified directory.
	.PARAMETER Path
		The path to the root directory of the application.
	.OUTPUTS
		The configuration of the specified application, or `$null` if not found.
	#>
	static [WebApplication] ReadFromDirectory([string] $Path) {
		foreach ($folder in "src/Server", "src") {
			foreach ($format in "json", "psd1", "xml") {
				if ($file = Get-Item (Join-Path $Path $folder "appsettings.$format") -ErrorAction Ignore) {
					$application = switch ($format) {
						"json" { [WebApplication]::DeserializeJson($file); break }
						"psd1" { [WebApplication]::DeserializePSData($file); break }
						"xml" { [WebApplication]::DeserializeXml($file); break }
					}

					$application.Path = $Path
					$application.Type = [WebApplication]::FindType($Path)
					return $application
				}
			}
		}

		return $null
	}

	<#
	.SYNOPSIS
		Deserializes the JSON document located at the specified path.
	.PARAMETER File
		The path to the JSON document.
	.OUTPUTS
		The deserialized application configuration.
	#>
	hidden static [WebApplication] DeserializeJson([FileInfo] $File) {
		$data = Get-Content $File.FullName | ConvertFrom-Json
		return [WebApplication]@{
			Description = $data.Description
			Environment = $data.Environment
			Id = $data.Id
			Name = $data.Name
		}
	}

	<#
	.SYNOPSIS
		Deserializes the PowerShell data file located at the specified path.
	.PARAMETER File
		The path to the PowerShell data file.
	.OUTPUTS
		The deserialized application configuration.
	#>
	hidden static [WebApplication] DeserializePSData([FileInfo] $File) {
		$data = Import-PowerShellDataFile $File.FullName
		return [WebApplication]@{
			Description = $data.Description
			Environment = $data.Environment
			Id = $data.Id
			Name = $data.Name
		}
	}

	<#
	.SYNOPSIS
		Deserializes the JSON document located at the specified path.
	.PARAMETER File
		The path to the XML document.
	.OUTPUTS
		The deserialized application configuration.
	#>
	hidden static [WebApplication] DeserializeXml([FileInfo] $File) {
		$data = ([xml] (Get-Content $File.FullName)).Configuration
		return [WebApplication]@{
			Description = $data.Description
			Environment = $data.Environment
			Id = $data.Id
			Name = $data.Name
		}
	}

	<#
	.SYNOPSIS
		Determines the type of the web application located at the specified path.
	.PARAMETER Path
		The path to the root directory of the application.
	.OUTPUTS
		The determined application type.
	#>
	hidden static [WebApplicationType] FindType([string] $Path) {
		$types = @{
			cs = [WebApplicationType]::DotNet
			fs = [WebApplicationType]::DotNet
			js = [WebApplicationType]::Node
			ps1 = [WebApplicationType]::PowerShell
			psm1 = [WebApplicationType]::PowerShell
			ts = [WebApplicationType]::Node
		}

		foreach ($extension in $types.Keys) {
			if (Test-Path "$Path/Server/*.$extension" -or Test-Path "$Path/*.$extension") { return $types.$extension }
		}

		return [WebApplicationType]::Unknown
	}
}

<#
.SYNOPSIS
	Defines the type of a web project.
#>
enum WebApplicationType {
	DotNet
	Node
	PowerShell
	Unknown
}
