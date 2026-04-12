& "$PSScriptRoot/Build.ps1"

"Running the test suite..."
pwsh -Command {
	Import-Module Pester
	Invoke-Pester test
	exit $LASTEXITCODE
}
