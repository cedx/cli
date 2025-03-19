namespace Belin.Cli.Nssm;

using System.Diagnostics;
using System.ServiceProcess;

/// <summary>
/// Registers the Windows service.
/// </summary>
public sealed class InstallCommand: Command {

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

		try {
			var application = ApplicationConfiguration.ReadFromDirectory(directory)
				?? throw new NotSupportedException("Unable to locate the application configuration file.");

			var (program, entryPoint) = Directory.EnumerateFiles(directory.FullName, "*.slnx").Any()
				? GetDotNetApplicationPaths(directory)
				: GetNodeApplicationPaths(directory);

			using var installProcess = Process.Start("nssm", ["install", application.Id, program, entryPoint])
				?? throw new NotSupportedException(@"The ""nssm"" program could not be started.");
			installProcess.WaitForExit();

			var properties = new Dictionary<string, string> {
				["AppDirectory"] = directory.FullName,
				["AppNoConsole"] = "1",
				["AppStderr"] = Path.Join(directory.FullName, @"var\stderr.log"),
				["AppStdout"] = Path.Join(directory.FullName, @"var\stdout.log"),
				["Description"] = application.Description,
				["DisplayName"] = application.Name,
				["Start"] = "SERVICE_AUTO_START"
			};

			foreach (var (key, value) in properties) {
				using var setProcess = Process.Start("nssm", ["set", application.Id, key, value])
					?? throw new NotSupportedException(@"The ""nssm"" program could not be started.");
				setProcess.WaitForExit();
			}

			if (start) {
				using var serviceController = new ServiceController(application.Id);
				if (serviceController.Status == ServiceControllerStatus.Stopped) {
					serviceController.Start();
					serviceController.WaitForStatus(ServiceControllerStatus.Running);
				}
			}
		}
		catch (NotSupportedException e) {
			Console.WriteLine(e.Message);
			return 2;
		}

		return await Task.FromResult(0);
	}

	/// <summary>
	/// Determines the paths of the program and the entry point of a .NET application.
	/// </summary>
	/// <param name="directory">The path to the application root directory.</param>
	/// <returns>The paths of the program and the entry point of the .NET application.</returns>
	/// <exception cref="NotSupportedException">TODO</exception>
	private static (string, string) GetDotNetApplicationPaths(DirectoryInfo directory) {
		var package = PackageJsonFile.ReadFromDirectory(directory.FullName) ?? throw new NotSupportedException(@"Unable to locate the ""package.json"" file.");
		var binary = package.Bin?.FirstOrDefault().Value ?? throw new NotSupportedException("Unable to determine the application entry point.");
		var dotnet = GetPathFromEnvironment("dotnet") ?? throw new NotSupportedException(@"Unable to locate the ""dotnet"" program.");
		return (dotnet.FullName, Path.GetFullPath(Path.Join(directory.FullName, binary)));
	}

	/// <summary>
	/// Determines the paths of the program and the entry point of a Node.js application.
	/// </summary>
	/// <param name="directory">The path to the application root directory.</param>
	/// <returns>The paths of the program and the entry point of the Node.js application.</returns>
	/// <exception cref="NotSupportedException">TODO</exception>
	private static (string, string) GetNodeApplicationPaths(DirectoryInfo directory) {
		var package = PackageJsonFile.ReadFromDirectory(directory.FullName) ?? throw new NotSupportedException(@"Unable to locate the ""package.json"" file.");
		var binary = package.Bin?.FirstOrDefault().Value ?? throw new NotSupportedException("Unable to determine the application entry point.");
		var node = GetPathFromEnvironment("node") ?? throw new NotSupportedException(@"Unable to locate the ""node"" program.");
		return (node.FullName, Path.GetFullPath(Path.Join(directory.FullName, binary)));
	}

	/// <summary>
	/// Gets the full path of the specified executable, by looking at the <c>PATH</c> environment variable.
	/// </summary>
	/// <param name="executable">The executable name.</param>
	/// <returns>The full path of the specified executable or <see langword="null"/> if not found.</returns>
	private static FileInfo? GetPathFromEnvironment(string executable) {
		var files =
			from path in (Environment.GetEnvironmentVariable("PATH") ?? string.Empty).Split(Path.PathSeparator)
			where !string.IsNullOrWhiteSpace(path)
			select new FileInfo(Path.Join(path.Trim(), executable + (OperatingSystem.IsWindows() ? ".exe" : string.Empty)));
		return files.FirstOrDefault(file => file.Exists);
	}
}
