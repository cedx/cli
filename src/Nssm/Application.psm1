using namespace System.Management.Automation

<#
.SYNOPSIS
	Represents a web application.
#>
class Application {

	<#
	.SYNOPSIS
		The application description.
	#>
	[string] $Description = ""

	<#
	.SYNOPSIS
		The application environment.
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
	[PathInfo] $Path

	<#
	.SYNOPSIS
		Creates a new application.
	#>
	Application([string] $Path) {
		$this.Path = Resolve-Path $Path

		foreach ($folder in "src/Server", "src") {
			foreach ($format in "json", "psd1", "xml") {
				if ($file = Get-Item (Join-Path $this.Path $folder "appsettings.$format") -ErrorAction Ignore) {
					$data = switch ($format) {
						"json" { Get-Content $file.FullName | ConvertFrom-Json; break }
						"psd1" { Import-PowerShellDataFile $file.FullName; break }
						"xml" { ([xml] (Get-Content $file.FullName)).Configuration; break }
					}

					$this.Description = $data.Description
					$this.Environment = $data.Environment ?? "Production"
					$this.Id = $data.Id
					$this.Name = $data.Name
					return
				}
			}
		}

		if (-not $this.Id) {
			throw [EntryPointNotFoundException] "Unable to locate the application configuration file."
		}
	}

	<#
	.SYNOPSIS
		Gets the entry point of this application.
	.OUTPUTS
		The entry point of this application.
	#>
	[string] GetEntryPoint() {
		throw [NotImplementedException]::new()
	}

	<#
	.SYNOPSIS
		Gets the name of the environment variable storing the application environment.
	.OUTPUTS
		The name of the environment variable storing the application environment.
	#>
	[string] GetEnvironmentVariable() {
		throw [NotImplementedException]::new()
	}

	<#
	.SYNOPSIS
		Gets the program used to run this application.
	.OUTPUTS
		The program used to run this application.
	#>
	[ApplicationInfo] GetProgram() {
		throw [NotImplementedException]::new()
	}
}
