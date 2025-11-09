using namespace System.Management.Automation
using module ./Application.psm1

<#
.SYNOPSIS
	Represents a Node.js application.
#>
class NodeApplication: Application {

	<#
	.SYNOPSIS
		The entry point of this application.
	#>
	hidden [string] $EntryPoint = ""

	<#
	.SYNOPSIS
		Creates a new application.
	.PARAMETER Path
		The path to the application root directory.
	#>
	NodeApplication([string] $Path): base($Path) {
		if ($file = Get-Item "$($this.Path)/package.json" -ErrorAction Ignore) {
			$package = Get-Content $file.FullName | ConvertFrom-Json -AsHashtable
			if (-not $this.Description) { $this.Description = $package.description }
			if (-not $this.Name) { $this.Name = $package.name }

			$keys = $package.bin?.Keys
			if ($keys) {
				$firstKey = $keys[0]
				$this.EntryPoint = Join-Path $this.Path $package.bin.$firstKey -Resolve -ErrorAction Ignore
			}
		}
	}

	<#
	.SYNOPSIS
		Gets the entry point of this application.
	.OUTPUTS
		The entry point of this application.
	#>
	[string] GetEntryPoint() {
		if ($this.EntryPoint) { return $this.EntryPoint }
		throw [EntryPointNotFoundException] "Unable to resolve the application entry point."
	}

	<#
	.SYNOPSIS
		Gets the name of the environment variable storing the application environment.
	.OUTPUTS
		The name of the environment variable storing the application environment.
	#>
	[string] GetEnvironmentVariable() {
		return "NODE_ENV"
	}

	<#
	.SYNOPSIS
		Gets the program used to run this application.
	.OUTPUTS
		The program used to run this application.
	#>
	[ApplicationInfo] Program() {
		return Get-Command "node"
	}
}
