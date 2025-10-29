using namespace System.IO

<#
.SYNOPSIS
	Checks whether the current process is privileged.
.PARAMETER Path
	The path to a directory used to verify if the process has sufficient permissions.
.OUTPUTS
	Value indicating whether the current process is privileged.
#>
function Test-Privilege {
	[OutputType([bool])] param (
		[Parameter(Position = 0)]
		[ValidateScript({ Test-Path $_ -IsValid }, ErrorMessage = "The specified output path is invalid.")]
		[string] $Path
	)

	$isPrivileged = [Environment]::IsPrivilegedProcess
	if (-not [string]::IsNullOrWhiteSpace($Path)) {
		$homePath = [DirectoryInfo] $HOME
		$outputPath = [DirectoryInfo] $Path
		if (($outputPath.Root.Name -ne $homePath.Root.Name) -or $outputPath.FullName.StartsWith($homePath.FullName)) { $isPrivileged = $true }
	}

	$isPrivileged
}
