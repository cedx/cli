namespace Belin.Cli.MySql;

using System.Diagnostics;
using System.IO;

/// <summary>
/// Restores a set of MariaDB/MySQL tables.
/// </summary>
public sealed class RestoreCommand: Command {

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
	public async Task<int> Execute(Uri dsn, FileSystemInfo fileOrDirectory, bool recursive = false) {
		if (!fileOrDirectory.Exists) {
			Console.WriteLine("Unable to locate the specified file or directory.");
			return 1;
		}

		var files = fileOrDirectory switch {
			DirectoryInfo directory => directory.EnumerateFiles("*.sql", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly),
			FileInfo file => [file],
			_ => []
		};

		foreach (var file in files) ImportFile(dsn, file);
		return await Task.FromResult(0);
	}

	/// <summary>
	/// Imports a SQL dump into the database.
	/// </summary>
	/// <param name="dsn">The connection string.</param>
	/// <param name="file">The path of the file to be restored.</param>
	/// <exception cref="ProcessException">An error occurred when starting the underlying process.</exception>
	private void ImportFile(Uri dsn, FileInfo file) {
		var entity = Path.GetFileNameWithoutExtension(file.FullName);
		Console.WriteLine($"Importing: {entity}");

		var query = this.ParseQueryString(dsn.Query);
		var userInfo = dsn.UserInfo.Split(':').Select(Uri.UnescapeDataString).ToArray();
		var args = new List<string> {
			$"--default-character-set={query["charset"] ?? "utf8mb4"}",
			$"--execute=USE {entity.Split('.').First()}; SOURCE {file.FullName.Replace('\\', '/')};",
			$"--host={dsn.Host}",
			$"--password={userInfo[1]}",
			$"--port={(dsn.IsDefaultPort ? 3306 : dsn.Port)}",
			$"--user={userInfo[0]}"
		};

		var hosts = new[] { "::1", "127.0.0.1", "localhost" };
		if (!hosts.Contains(dsn.Host)) args.Add("--compress");

		var startInfo = new ProcessStartInfo("mysql", args) { CreateNoWindow = true, RedirectStandardError = true };
		using var process = Process.Start(startInfo) ?? throw new ProcessException(startInfo.FileName);

		var stderr = process.StandardError.ReadToEnd().Trim();
		process.WaitForExit();
		if (process.ExitCode != 0) throw new ProcessException(startInfo.FileName, stderr);
	}
}
