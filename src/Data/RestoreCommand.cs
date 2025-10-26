namespace Belin.Cli.MySql;

using namespace System.Diagnostics;
using namespace System.IO;
using namespace System.Web;

/// <summary>
/// Restores a set of MariaDB/MySQL tables.
/// </summary>
class RestoreCommand: Command {

	/// <summary>
	/// The path to the file or directory to process.
	/// </summary>
	private readonly Argument<FileSystemInfo> fileOrDirectoryArgument = new Argument<FileSystemInfo>("fileOrDirectory") {
		Description = "The path to the file or directory to process."
	}.AcceptExistingOnly();

	/// <summary>
	/// Value indicating whether to process the directory recursively.
	/// </summary>
	private readonly Option<bool> recursiveOption = new("--recursive", ["-r"]) {
		Description = "Whether to process the directory recursively."
	};

	/// <summary>
	/// Creates a new <c>restore</c> command.
	/// </summary>
	public RestoreCommand(): base("restore", "Restore a set of MariaDB/MySQL tables.") {
		Arguments.Add(fileOrDirectoryArgument);
		Options.Add(recursiveOption);
		SetAction(InvokeAsync);
	}

	/// <summary>
	/// Invokes this command.
	/// </summary>
	/// <param name="parseResult">The results of parsing the command line input.</param>
	/// <returns>The exit code.</returns>
	[int] Invoke(ParseResult parseResult) {
		var files = parseResult.GetRequiredValue(fileOrDirectoryArgument) switch {
			DirectoryInfo directory => directory.EnumerateFiles("*.sql", parseResult.GetValue(recursiveOption) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly),
			FileInfo file => [file],
			_ => []
		};

		var dsn = new Uri(parseResult.GetRequiredValue(MySqlCommand.dsnOption));
		foreach (var file in files) ImportFile(file, dsn);
		return 0;
	}

	/// <summary>
	/// Invokes this command.
	/// </summary>
	/// <param name="parseResult">The results of parsing the command line input.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The exit code.</returns>
	public Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken) => Task.FromResult(Invoke(parseResult));

	/// <summary>
	/// Imports a SQL dump into the database.
	/// </summary>
	/// <param name="file">The path of the file to be restored.</param>
	/// <param name="dsn">The connection string.</param>
	/// <exception cref="ProcessException">An error occurred when starting the underlying process.</exception>
	private static void ImportFile(FileInfo file, Uri dsn) {
		var entity = Path.GetFileNameWithoutExtension(file.FullName);
		Console.WriteLine("Importing: {0}", entity);

		var userInfo = dsn.UserInfo.Split(':').Select(Uri.UnescapeDataString);
		var args = new List<string> {
			$"--default-character-set={HttpUtility.ParseQueryString(dsn.Query)["charset"] ?? "utf8mb4"}",
			$"--execute=USE {entity.Split('.').First()}; SOURCE {file.FullName.Replace('\\', '/')};",
			$"--host={dsn.Host}",
			$"--password={userInfo.Last()}",
			$"--port={(dsn.IsDefaultPort ? 3306 : dsn.Port)}",
			$"--user={userInfo.First()}"
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
