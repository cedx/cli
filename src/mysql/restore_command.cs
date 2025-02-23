namespace Belin.Cli.MySql;

using System.Text;

/// <summary>
/// Restores a set of MariaDB/MySQL tables.
/// </summary>
public class RestoreCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public RestoreCommand(DsnOption dsnOption): base("restore", "Restore a set of MariaDB/MySQL tables.") {
		var fileOrDirectoryArgument = new Argument<DirectoryInfo>("fileOrDirectory", "The path to a file or directory to process.");
		var recursiveOption = new Option<bool>(["-r", "--recursive"], "Whether to process the directory recursively.");

		Add(fileOrDirectoryArgument);
		Add(recursiveOption);
		this.SetHandler(Execute, dsnOption, fileOrDirectoryArgument, recursiveOption);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <param name="dsn">The connection string.</param>
	/// <param name="fileOrDirectory">The path to a file or directory to process.</param>
	/// <param name="recursive">Value indicating whether to process the directory recursively.</param>
	/// <returns>The exit code.</returns>
	public static async Task<int> Execute(Uri dsn, FileSystemInfo fileOrDirectory, bool recursive = false) {
		if (!fileOrDirectory.Exists) {
			Console.WriteLine("Unable to locate the specified file or directory.");
			return 1;
		}

		var files = fileOrDirectory switch {
			DirectoryInfo directory => directory.EnumerateFiles("*.sql", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly),
			FileInfo file => [file],
			_ => []
		};

		foreach (var file in files) ImportFile(file);
		return await Task.FromResult(0);
	}

	/// <summary>
	/// Imports a SQL dump into the database.
	/// </summary>
	/// <param name="file">The path of the file to be restored.</param>
	private static void ImportFile(FileInfo file) {
		// TODO
	}
}
