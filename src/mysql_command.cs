namespace Belin.Cli;

using Belin.Cli.MySql;
using System.Collections.Specialized;

/// <summary>
/// Manages MariaDB/MySQL databases.
/// </summary>
public sealed class MySqlCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public MySqlCommand(): base("mysql", "Manage MariaDB/MySQL databases.") {
		var dsnOption = new DsnOption();
		AddGlobalOption(dsnOption);
		Add(new BackupCommand(dsnOption));
		Add(new CharsetCommand(dsnOption));
		Add(new EngineCommand(dsnOption));
		Add(new OptimizeCommand(dsnOption));
		Add(new RestoreCommand(dsnOption));
	}

	/// <summary>
	/// Parses the specified query string into a name-value collection.
	/// </summary>
	/// <param name="query">The query string.</param>
	/// <returns>The name-value collection corresponding to the specified query string.</returns>
	internal static NameValueCollection ParseQueryString(string query) {
		if (query.StartsWith('?')) query = query[1..];

		var collection = new NameValueCollection();
		if (query.Length > 0) foreach (var param in query.Split('&')) {
			var parts = param.Split('=');
			if (parts.Length > 0) collection.Add(Uri.UnescapeDataString(parts[0]), parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : null);
		}

		return collection;
	}
}
