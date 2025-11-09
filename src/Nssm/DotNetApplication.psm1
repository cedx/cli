using namespace System.Management.Automation
using module ./Application.psm1

<#
.SYNOPSIS
	Represents a .NET application.
#>
class DotNetApplication: Application {

	<#
	.SYNOPSIS
		The path of the application entry point.
	#>
	hidden [string] $EntryPath = ""

	<#
	.SYNOPSIS
		Creates a new application.
	.PARAMETER Path
		The path to the application root directory.
	#>
	DotNetApplication([string] $Path): base($Path) {
		if ($file = Get-Item "$($this.Path)/src/Server/*.csproj" -ErrorAction Ignore || Get-Item "$($this.Path)/src/*.csproj" -ErrorAction Ignore) {
			$project = @{ AssemblyName = ""; OutDir = "" }

			foreach ($propertyGroup in ([xml] (Get-Content $file.FullName)).Project.PropertyGroup) {
				if (-not $this.Description) { $this.Description = $propertyGroup.Description }
				if (-not $this.Name) { $this.Name = $propertyGroup.Product }
				if (-not $project.AssemblyName) { $project.AssemblyName = $propertyGroup.AssemblyName }
				if ((-not $project.OutDir) -and $propertyGroup.OutDir) { $project.OutDir = "$($file.DirectoryName)/$($propertyGroup.OutDir)" }
			}

			if (-not $project.AssemblyName) { $project.AssemblyName = $file.BaseName }
			if (-not $project.OutDir) { $project.OutDir = "$($this.Path)/bin" }
			$this.EntryPath = Join-Path $project.OutDir "$($project.AssemblyName).dll" -Resolve -ErrorAction Ignore
		}
	}

	<#
	.SYNOPSIS
		Gets the entry point of this application.
	.OUTPUTS
		The entry point of this application.
	#>
	[string] EntryPoint() {
		if ($this.EntryPath) { return $this.EntryPath }
		throw [EntryPointNotFoundException] "Unable to resolve the application entry point."
	}

	<#
	.SYNOPSIS
		Gets the name of the environment variable storing the application environment.
	.OUTPUTS
		The name of the environment variable storing the application environment.
	#>
	[string] EnvironmentVariable() {
		return "DOTNET_ENVIRONMENT"
	}

	<#
	.SYNOPSIS
		Gets the program used to run this application.
	.OUTPUTS
		The program used to run this application.
	#>
	[ApplicationInfo] Program() {
		return Get-Command "dotnet"
	}
}
