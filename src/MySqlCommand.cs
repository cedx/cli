namespace Belin.Cli;

using Belin.Cli.MySql;
using System.CommandLine.Parsing;

/// <summary>
/// Manages MariaDB/MySQL databases.
/// </summary>
public class MySqlCommand: Command {

	/// <summary>
	/// The connection string.
	/// </summary>
	internal static readonly DsnOption dsnOption = new();

	/// <summary>
	/// Creates a new <c>mysql</c> command.
	/// </summary>
	/// <param name="backup">The <c>backup</c> subcommand.</param>
	/// <param name="charset">The <c>charset</c> subcommand.</param>
	/// <param name="engine">The <c>engine</c> subcommand.</param>
	/// <param name="optimize">The <c>optimize</c> subcommand.</param>
	/// <param name="restore">The <c>restore</c> subcommand.</param>
	public MySqlCommand(BackupCommand backup, CharsetCommand charset, EngineCommand engine, OptimizeCommand optimize, RestoreCommand restore):
	base("mysql", "Manage MariaDB/MySQL databases.") {
		Subcommands.Add(backup);
		Subcommands.Add(charset);
		Subcommands.Add(engine);
		Subcommands.Add(optimize);
		Subcommands.Add(restore);
		Options.Add(dsnOption);
	}
}

/// <summary>
/// Provides the connection string of a data source.
/// </summary>
internal class DsnOption: Option<Uri> {

	/// <summary>
	/// The list of supported schemes.
	/// </summary>
	private static readonly string[] allowedSchemes = ["mariadb", "mysql"];

	/// <summary>
	/// Creates a new option.
	/// </summary>
	public DsnOption(): base("--dsn", ["-d"]) {
		Description = "The connection string.";
		HelpName = "uri";
		Recursive = true;
		Required = true;
		Validators.Add(Validate);
	}

	/// <summary>
	/// Validates the result produced when parsing this option.
	/// </summary>
	/// <param name="optionResult">The parsed result.</param>
	private void Validate(OptionResult result) {
		// TODO GetRequiredValue ????
		Console.WriteLine("dsn: VALIDATE"); // TODO remove!
		if (result.GetValue(this) is not Uri uri) {
			Console.WriteLine("dsn: NULL option"); // TODO remove!
			return;
		}

		Console.WriteLine("dsn: VALIDATING......"); // TODO remove!
		var schemes = string.Join(" or ", allowedSchemes.Select(scheme => $"'{scheme}'"));
		if (!uri.IsAbsoluteUri) result.AddError($"The '--{Name}' option requires an absolute URI.");
		else if (!allowedSchemes.Contains(uri.Scheme)) result.AddError($"The '--{Name}' option only supports the {schemes} scheme.");
		else if (!uri.UserInfo.Contains(':')) result.AddError($"The '--{Name}' option requires full credentials to be specified.");
	}
}
