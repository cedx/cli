namespace Belin.Cli.MySql;

using Microsoft.Extensions.Logging;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.IO;

/// <summary>
/// Restores a set of MariaDB/MySQL tables.
/// </summary>
public class RestoreCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public RestoreCommand(): base("restore", "Restore a set of MariaDB/MySQL tables.") {
		Add(new Argument<DirectoryInfo>("fileOrDirectory") { Description = "The path to a file or directory to process." });
		Add(new Option<bool>("--recursive", ["-r"]) { Description = "Whether to process the directory recursively." });
	}

	/// <summary>
	/// The command handler.
	/// </summary>
	/// <param name="logger">The logging service.</aparam>
	public class CommandHandler(ILogger<RestoreCommand> logger): ICommandHandler {

		/// <summary>
		/// The connection string.
		/// </summary>
		public required Uri Dsn { get; set; }

		/// <summary>
		/// The path to the file or directory to process.
		/// </summary>
		public required FileSystemInfo FileOrDirectory { get; set; }

		/// <summary>
		/// Value indicating whether to process the directory recursively.
		/// </summary>
		public bool Recursive { get; set; }

		/// <summary>
		/// Invokes this command.
		/// </summary>
		/// <param name="context">The invocation context.</param>
		/// <returns>The exit code.</returns>
		public int Invoke(InvocationContext context) {
			if (!FileOrDirectory.Exists) {
				logger.LogError("Unable to locate the specified file or directory.");
				return 1;
			}

			var files = FileOrDirectory switch {
				DirectoryInfo directory => directory.EnumerateFiles("*.sql", Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly),
				FileInfo file => [file],
				_ => []
			};

			foreach (var file in files) ImportFile(file);
			return 0;
		}

		/// <summary>
		/// Invokes this command.
		/// </summary>
		/// <param name="context">The invocation context.</param>
		/// <returns>The exit code.</returns>
		public Task<int> InvokeAsync(InvocationContext context) => Task.FromResult(Invoke(context));

		/// <summary>
		/// Imports a SQL dump into the database.
		/// </summary>
		/// <param name="file">The path of the file to be restored.</param>
		/// <exception cref="ProcessException">An error occurred when starting the underlying process.</exception>
		private void ImportFile(FileInfo file) {
			var entity = Path.GetFileNameWithoutExtension(file.FullName);
			logger.LogInformation("Importing: {Entity}", entity);

			var userInfo = Dsn.UserInfo.Split(':').Select(Uri.UnescapeDataString);
			var args = new List<string> {
				$"--default-character-set={Dsn.ParseQueryString()["charset"] ?? "utf8mb4"}",
				$"--execute=USE {entity.Split('.').First()}; SOURCE {file.FullName.Replace('\\', '/')};",
				$"--host={Dsn.Host}",
				$"--password={userInfo.Last()}",
				$"--port={(Dsn.IsDefaultPort ? 3306 : Dsn.Port)}",
				$"--user={userInfo.First()}"
			};

			var hosts = new[] { "::1", "127.0.0.1", "localhost" };
			if (!hosts.Contains(Dsn.Host)) args.Add("--compress");

			var startInfo = new ProcessStartInfo("mysql", args) { CreateNoWindow = true, RedirectStandardError = true };
			using var process = Process.Start(startInfo) ?? throw new ProcessException(startInfo.FileName);

			var stderr = process.StandardError.ReadToEnd().Trim();
			process.WaitForExit();
			if (process.ExitCode != 0) throw new ProcessException(startInfo.FileName, stderr);
		}
	}
}
