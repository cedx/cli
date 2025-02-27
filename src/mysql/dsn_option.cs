namespace Belin.Cli.MySql;

using System.CommandLine.Parsing;

/// <summary>
/// Provides the connection string of a data source.
/// </summary>
public class DsnOption: Option<Uri> {

	/// <summary>
	/// The list of supported schemes.
	/// </summary>
	private static readonly string[] allowedSchemes = ["mariadb", "mysql"];

	/// <summary>
	/// Creates a new option.
	/// </summary>
	public DsnOption(): base(["-d", "--dsn"], /*Parse,*/ description: "The connection string.") {
		ArgumentHelpName = "uri";
		IsRequired = true;
		AddValidator(Validate);
	}

	/// <summary>
	/// Validates the result produced when parsing this option.
	/// </summary>
	/// <param name="optionResult">The parsed result.</param>
	private void Validate(OptionResult result) {
		var uri = result.GetValueForOption(this);
		if (uri is not null) {
			var schemes = string.Join(" or ", allowedSchemes.Select(scheme => $"'{scheme}'"));
			if (!uri.IsAbsoluteUri) result.ErrorMessage = $"The '--{Name}' option requires an absolute URI.";
			else if (!allowedSchemes.Contains(uri.Scheme)) result.ErrorMessage = $"The '--{Name}' option only supports the {schemes} scheme.";
			else if (!uri.UserInfo.Contains(':')) result.ErrorMessage = $"The '--{Name}' option requires full credentials to be specified.";
		}
	}
}
