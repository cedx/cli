using namespace System.IO

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
						"json" { [WebApplication]::DeserializeJson($file) }
						"psd1" { [WebApplication]::DeserializePSData($file) }
						"xml" { [WebApplication]::DeserializeXml($file) }
					}

					$application.Path = $Path
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
}
