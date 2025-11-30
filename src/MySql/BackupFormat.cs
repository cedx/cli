namespace Belin.Cli.MySql;

/// <summary>
/// Defines the format of the output files.
/// </summary>
public static class BackupFormat {

	/// <summary>
	/// The JSON Lines format.
	/// </summary>
	public const string JsonLines = "jsonl";

	/// <summary>
	/// The SQL format.
	/// </summary>
	public const string SqlDump = "sql";
}
