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
		The content of the associated "package.json" file.
	#>
	hidden [hashtable] $Package = @{}

	<#
	.SYNOPSIS
		Creates a new application.
	.PARAMETER Path
		The path to the application root directory.
	#>
	NodeApplication([string] $Path): base($Path) {
		if ($file = Get-Item "$($this.Path)/package.json" -ErrorAction Ignore) {
			$this.Package = Get-Content $file.FullName | ConvertFrom-Json -AsHashtable
			if (-not $this.Description) { $this.Description = $this.Package.description }
			if (-not $this.Name) { $this.Name = $this.Package.name }

			$keys = $this.Package.bin?.Keys
			if ($keys) { $this.EntryPoint = Join-Path $this.Path $this.Package.bin[$keys[0]] }
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
		throw [EntryPointNotFoundException] "Unable to determine the application entry point."
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
	[ApplicationInfo] GetProgram() {
		return Get-Command "node"
	}
}
