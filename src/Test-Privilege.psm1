using namespace System.IO

<#
.SYNOPSIS
	Checks whether the current process is privileged.
.PARAMETER Path
	TODO The path to the output directory.
.OUTPUTS
	Value indicating whether the current process is privileged.
#>
function Test-Privilege {
	[OutputType([bool])] param (
		[Parameter(Position = 0)]
		[DirectoryInfo] $Path # TODO or String ????
	)

	$isPrivileged = [Environment]::IsPrivilegedProcess
	if (-not [string]::IsNullOrWhiteSpace($Path)) {
		$homePath = [DirectoryInfo] [Environment]::GetFolderPath([Environment]::SpecialFolder.Personal)
		if (<# TODO ($Path.Root.Name -ne $homePath.Root.Name) -or #> $Path.FullName.StartsWith($homePath.FullName)) { $isPrivileged = $true }
	}

	# if ($null -ne $Path) {
	# 	# TODO ???
	# }

	$isPrivileged
}
