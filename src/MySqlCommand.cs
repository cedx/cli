namespace Belin.Cli;

using Belin.Cli.MySql;
using Microsoft.Extensions.Hosting;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;

/// <summary>
/// Manages MariaDB/MySQL databases.
/// </summary>
public class MySqlCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public MySqlCommand(): base("mysql", "Manage MariaDB/MySQL databases.") {
		Add(new BackupCommand());
		Add(new CharsetCommand());
		Add(new EngineCommand());
		Add(new OptimizeCommand());
		Add(new RestoreCommand());
		Add(new DsnOption());
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
		var uri = result.GetValue(this);
		if (uri is not null) {
			var schemes = string.Join(" or ", allowedSchemes.Select(scheme => $"'{scheme}'"));
			if (!uri.IsAbsoluteUri) result.AddError($"The '--{Name}' option requires an absolute URI.");
			else if (!allowedSchemes.Contains(uri.Scheme)) result.AddError($"The '--{Name}' option only supports the {schemes} scheme.");
			else if (!uri.UserInfo.Contains(':')) result.AddError($"The '--{Name}' option requires full credentials to be specified.");
		}
	}
}

/// <summary>
/// Provides extension methods for the <c>mysql</c> command.
/// </summary>
internal static class MySqlCommandExtensions {

	/// <summary>
	/// Specifies the command handlers to be used by the host.
	/// </summary>
	/// <param name="builder">The host builder to configure.</param>
	/// <returns>The host builder.</returns>
	public static IHostBuilder UseMySqlHandlers(this IHostBuilder builder) => builder
		.UseCommandHandler<BackupCommand, BackupCommand.CommandHandler>()
		.UseCommandHandler<CharsetCommand, CharsetCommand.CommandHandler>()
		.UseCommandHandler<EngineCommand, EngineCommand.CommandHandler>()
		.UseCommandHandler<OptimizeCommand, OptimizeCommand.CommandHandler>()
		.UseCommandHandler<RestoreCommand, RestoreCommand.CommandHandler>();
}
