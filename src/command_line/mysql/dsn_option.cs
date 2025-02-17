namespace Belin.Cli.CommandLine.MySql;

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
	///
	/// </summary>
	/// <param name=""></param>
	private static Uri Parse(ArgumentResult result) {
		var dsn = result.Tokens[0].Value;
		if (string.IsNullOrWhiteSpace(dsn)) {
			result.ErrorMessage = "TODO !!!!!!!!!!!!";
			return new Uri("error:string.IsNullOrWhiteSpace");
		}

		//if (!dsn.Contains("://")) dsn = $"mysql://{dsn}";

		// if (!Uri.TryCreate(dsn, UriKind.Absolute, out var uri)) {
		// 	result.ErrorMessage = "TODO invalid URI";
		// 	return new Uri("invalid");
		// }

		// if (!allowedSchemes.Contains(uri.Scheme)) {
		// 	result.ErrorMessage = "TODO invalid SCHEME";
		// 	return new Uri("invalid");
		// }

		return new Uri($"todo:{dsn}");
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="optionResult"></param>
	private static void Validate(OptionResult result) {
		if (result.ErrorMessage is not null) {
			Console.WriteLine(result.ErrorMessage);
		}

		var uri = result.GetValueForOption(result.Option)!;
		Console.WriteLine(uri);
		// if (!uri.IsAbsoluteUri) uri.Scheme = "mysql";
		// result.Option.Parse()
	}
}
