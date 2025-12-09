namespace Belin.Cli.Validation;

/// <summary>
/// Validates that the specified path is an existing file.
/// </summary>
/// <param name="errorMessage">The custom error message that is displayed to the user.</param>
public class ValidateFileAttribute(string errorMessage): ValidateArgumentsAttribute {

	/// <summary>
	/// Verifies that the value of <c>arguments</c> is valid.
	/// </summary>
	/// <param name="arguments">The argument value to validate.</param>
	/// <param name="engineIntrinsics">The engine APIs for the context under which the prerequisite is being evaluated.</param>
	/// <exception cref="ValidationMetadataException">The validation failed.</exception>
  protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics) {
		if (arguments is not string path || !File.Exists(path)) throw new ValidationMetadataException(errorMessage);
	}
}
