namespace Belin.Cli.CommandLine.Nssm;

using System.Diagnostics;
using System.ServiceProcess;

/// <summary>
/// Registers the Windows service.
/// </summary>
public class InstallCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public InstallCommand(): base("install", "Register the Windows service.") {
		var directoryArgument = new Argument<DirectoryInfo>(
			name: "directory",
			description: "The path to the root directory of the Node.js application.",
			getDefaultValue: () => new DirectoryInfo(Environment.CurrentDirectory)
		);

		var startOption = new Option<bool>(["-s", "--start"], "Whether to start the service after its registration.");
		Add(directoryArgument);
		Add(startOption);
		this.SetHandler(Execute, directoryArgument, startOption);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <param name="directory">The path to the root directory of the Node.js application.</param>
	/// <param name="start">Value indicating whether to start the service after its registration.</param>
	/// <returns>The exit code.</returns>
	public async Task<int> Execute(DirectoryInfo directory, bool start = false) {
		if (!this.CheckPrivilege()) return 1;

		var package = PackageJsonFile.ReadFromDirectory(directory);
		if (package is null) {
			Console.WriteLine(@"Unable to locate the ""package.json"" file.");
			return 2;
		}

		var binary = package.Bin?.FirstOrDefault().Value;
		if (binary is null) {
			Console.WriteLine("Unable to determine the application entry point.");
			return 3;
		}

		var config = ApplicationConfiguration.ReadFromDirectory(directory);
		if (config is null) {
			Console.WriteLine("Unable to locate the application configuration file.");
			return 4;
		}

		var node = GetPathFromEnvironment("node.exe");
		if (node is null) {
			Console.WriteLine(@"Unable to locate the ""node.exe"" program.");
			return 5;
		}

		using var installProcess = Process.Start("nssm.exe", ["install", config.Id, node.FullName, Path.Join(directory.FullName, binary)]);
		if (installProcess is not null) await installProcess.WaitForExitAsync();
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
			if (setProcess is not null) await setProcess.WaitForExitAsync();
			else {
				Console.WriteLine(@"The ""nssm.exe"" program could not be started.");
				return 7;
			}
		}

		if (start) {
			using var serviceController = new ServiceController(config.Id);
			if (serviceController.Status == ServiceControllerStatus.Stopped) {
				serviceController.Start();
				serviceController.WaitForStatus(ServiceControllerStatus.Running);
			}
		}

		return 0;
	}

	/// <summary>
	/// Gets the full path of the specified executable, by looking at the <c>Path</c> environment variable.
	/// </summary>
	/// <param name="executable">The executable name.</param>
	/// <returns>The full path of the specified executable or <see langword="null"/> if not found.</returns>
	private static FileInfo? GetPathFromEnvironment(string executable) {
		var files = from path in (Environment.GetEnvironmentVariable("Path") ?? string.Empty).Split(';')
			where !string.IsNullOrWhiteSpace(path)
			select new FileInfo(Path.Join(path.Trim(), executable));
		return files.FirstOrDefault(file => file.Exists);
	}
}
