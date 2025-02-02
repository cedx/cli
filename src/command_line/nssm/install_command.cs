namespace Belin.Cli.CommandLine.Nssm;

using System.Diagnostics;

/// <summary>
/// Registers the Windows service.
/// </summary>
public class InstallCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public InstallCommand(): base("install", "Register the Windows service.") {
		var workingDirectory = new DirectoryInfo(Environment.CurrentDirectory);
		var directoryArgument = new Argument<DirectoryInfo>(
			name: "directory",
			description: "The path to the root directory of the Node.js application.",
			getDefaultValue: () => workingDirectory
		);

		Add(directoryArgument);
		this.SetHandler(Execute, directoryArgument);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <param name="directory">The path to the root directory of the Node.js application.</param>
	/// <returns>The exit code.</returns>
	public async Task<int> Execute(DirectoryInfo directory) {
		if (!this.CheckPrivilege()) return 1;

		var package = PackageJsonFile.ReadFromDirectory(directory);
		if (package == null) {
			Console.WriteLine(@"Unable to locate the ""package.json"" file.");
			return 2;
		}

		var binary = package.Bin?.FirstOrDefault().Value;
		if (binary == null) {
			Console.WriteLine("Unable to determine the application entry point.");
			return 3;
		}

		var config = ApplicationConfiguration.ReadFromDirectory(directory);
		if (config == null) {
			Console.WriteLine("Unable to locate the application configuration file.");
			return 4;
		}

		var node = GetPathFromEnvironment("node.exe");
		if (node == null) {
			Console.WriteLine(@"Unable to locate the ""node.exe"" program.");
			return 5;
		}

		using var installProcess = Process.Start("nssm.exe", ["install", config.Id, node.FullName, Path.Join(directory.FullName, binary)]);
		if (installProcess != null) await installProcess.WaitForExitAsync();
		else {
			Console.WriteLine(@"The ""nssm.exe"" program could not be started.");
			return 6;
		}

		var properties = new Dictionary<string, string> {
			["AppDirectory"] = directory.FullName,
			["AppNoConsole"] = "1",
			["AppStderr"] = Path.Join(directory.FullName, @"var\stderr.log"),
			["AppStdout"] = Path.Join(directory.FullName, @"var\stdout.log"),
			["Description"] = package.Description ?? string.Empty,
			["DisplayName"] = config.Name,
			["Start"] = "SERVICE_AUTO_START"
		};

		foreach (var (key, value) in properties) {
			using var setProcess = Process.Start("nssm.exe", ["set", config.Id, key, value]);
			if (setProcess != null) await setProcess.WaitForExitAsync();
			else {
				Console.WriteLine(@"The ""nssm.exe"" program could not be started.");
				return 7;
			}
		}

		return 0;
	}

	/// <summary>
	/// TODO
	/// </summary>
	/// <param name="command"></param>
	/// <returns>TODO or <see langword="null"/> if not found.</returns>
	private static FileInfo? GetPathFromEnvironment(string command) {
		return Environment.GetEnvironmentVariable("Path")!
			.Split(";")
			.Where(path => !string.IsNullOrWhiteSpace(path))
			.Select(path => new FileInfo(Path.Join(path.Trim(), command)))
			.FirstOrDefault(file => file.Exists);
	}
}
