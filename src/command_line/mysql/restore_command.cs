namespace Belin.Cli.CommandLine.MySql;

/// <summary>
/// Restores a set of MariaDB/MySQL tables.
/// </summary>
public class RestoreCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public RestoreCommand(DsnOption dsnOption): base("restore", "Restore a set of MariaDB/MySQL tables.") {
		var fileOrDirectoryArgument = new Argument<DirectoryInfo>("fileOrDirectory", "The path to a file or directory to process.");
		this.SetHandler(Execute, dsnOption, fileOrDirectoryArgument);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <param name="dsn">The connection string.</param>
	/// <param name="fileOrDirectory">The path to a file or directory to process.</param>
	/// <returns>The exit code.</returns>
	public static Task<int> Execute(Uri dsn, FileSystemInfo fileOrDirectory) {
		return Task.FromResult(0);
	}
}
