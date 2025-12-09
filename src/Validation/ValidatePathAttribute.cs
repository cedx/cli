namespace Belin.Cli.Validation;

/// <summary>
/// Validates that the specified path is valid.
/// </summary>
/// <param name="errorMessage">The custom error message that is displayed to the user.</param>
public class ValidatePathAttribute(string errorMessage): ValidateArgumentsAttribute {

	/// <summary>
	/// An array containing the characters that are not allowed in path names.
	/// </summary>
	private static readonly char[] invalidCharacters = Path.GetInvalidPathChars();

	/// <summary>
	/// Verifies that the value of <c>arguments</c> is valid.
	/// </summary>
	/// <param name="arguments">The argument value to validate.</param>
	/// <param name="engineIntrinsics">The engine APIs for the context under which the prerequisite is being evaluated.</param>
	/// <exception cref="ValidationMetadataException">The validation failed.</exception>
  protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics) {
		if (arguments is not string path || invalidCharacters.Any(path.Contains)) throw new ValidationMetadataException(errorMessage);
  }
}
