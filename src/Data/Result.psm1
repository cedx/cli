<#
.SYNOPSIS
	Provides details about the result of an SQL query.
#>
class Result {

	<#
	.SYNOPSIS
		Value indicating whether this result is a failure.
	#>
	[bool] $IsFailure = $false

	<#
	.SYNOPSIS
		Value indicating whether this result is a success.
	#>
	[bool] $IsSuccess = $true

	<#
	.SYNOPSIS
		A custom message providing details about the result.
	#>
	[string] $Message = ""

	<#
	.SYNOPSIS
		Creates a new failure.
	.OUTPUTS
		A failed result.
	#>
	static [Result] Failure() {
		return [Result]@{ IsFailure = $true; IsSuccess = $false; Message = "" };
	}

	<#
	.SYNOPSIS
		Creates a new failure.
	.PARAMETER Message
		A custom message providing details about the result.
	.OUTPUTS
		A failed result.
	#>
	static [Result] Failure([string] $Message) {
		return [Result]@{ IsFailure = $true; IsSuccess = $false; Message = $Message };
	}

	<#
	.SYNOPSIS
		Creates a new success.
	.OUTPUTS
		A successful result.
	#>
	static [Result] Success() {
		return [Result]@{ IsFailure = $false; IsSuccess = $true; Message = "" };
	}

	<#
	.SYNOPSIS
		Creates a new success.
	.PARAMETER Message
		A custom message providing details about the result.
	.OUTPUTS
		A successful result.
	#>
	static [Result] Success([string] $Message) {
		return [Result]@{ IsFailure = $false; IsSuccess = $true; Message = $Message };
	}
}
