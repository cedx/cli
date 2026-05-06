foreach ($cmdlet in Get-ChildItem "$PSScriptRoot/*/*.ps1") {
	. $cmdlet.FullName
}
