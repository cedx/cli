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
	public Task<int> Execute(DirectoryInfo directory, bool start = false) {
		if (!this.CheckPrivilege()) return Task.FromResult(1);

		try {
			var application = Application.ReadFromDirectory(directory.FullName)
				?? throw new EntryPointNotFoundException("Unable to locate the application configuration file.");

			var (program, entryPoint) = Directory.EnumerateFiles(application.Path, "*.slnx").Any()
				? GetDotNetApplicationPaths(application)
				: GetNodeApplicationPaths(application);

			using var installProcess = Process.Start("nssm", ["install", application.Id, program, entryPoint]) ?? throw new ProcessException("nssm");
			installProcess.WaitForExit();
			if (installProcess.ExitCode != 0) throw new ProcessException("nssm", $"The \"install\" command failed with exit code {installProcess.ExitCode}.");

			var properties = new Dictionary<string, string> {
				["AppDirectory"] = application.Path,
				["AppNoConsole"] = "1",
				["AppStderr"] = Path.Join(application.Path, @"var\stderr.log"),
				["AppStdout"] = Path.Join(application.Path, @"var\stdout.log"),
				["Description"] = application.Description,
				["DisplayName"] = application.Name,
				["Start"] = "SERVICE_AUTO_START"
			};

			foreach (var (key, value) in properties) {
				using var setProcess = Process.Start("nssm", ["set", application.Id, key, value]) ?? throw new ProcessException("nssm");
				setProcess.WaitForExit();
				if (setProcess.ExitCode != 0) throw new ProcessException("nssm", $"The \"set\" command failed with exit code {setProcess.ExitCode}.");
			}

			if (start) StartApplication(application);
			return Task.FromResult(0);
		}
		catch (Exception e) {
			Console.WriteLine(e.Message);
			return Task.FromResult(2);
		}
	}

	/// <summary>
	/// Determines the paths of the program and the entry point of a .NET application.
	/// </summary>
	/// <param name="application">The application configuration.</param>
	/// <returns>The paths of the program and the entry point of the .NET application.</returns>
	/// <exception cref="EntryPointNotFoundException">The program and/or entry point could not be determined.</exception>
	private static (string Program, string EntryPoint) GetDotNetApplicationPaths(Application application) {
		var package = NodePackage.ReadFromDirectory(application.Path) ?? throw new EntryPointNotFoundException(@"Unable to locate the ""package.json"" file.");

		var entryPoint = package.Bin?.FirstOrDefault().Value ?? throw new EntryPointNotFoundException("Unable to determine the application entry point.");
		var program = GetPathFromEnvironment("dotnet") ?? throw new EntryPointNotFoundException(@"Unable to locate the ""dotnet"" program.");
		return (Program: program.FullName, EntryPoint: Path.GetFullPath(Path.Join(application.Path, entryPoint)));
	}

	/// <summary>
	/// Determines the paths of the program and the entry point of a Node.js application.
	/// </summary>
	/// <param name="application">The application configuration.</param>
	/// <returns>The paths of the program and the entry point of the Node.js application.</returns>
	/// <exception cref="EntryPointNotFoundException">The program and/or entry point could not be determined.</exception>
	private static (string Program, string EntryPoint) GetNodeApplicationPaths(Application application) {
		var package = NodePackage.ReadFromDirectory(application.Path) ?? throw new EntryPointNotFoundException(@"Unable to locate the ""package.json"" file.");
		if (!string.IsNullOrWhiteSpace(package.Description)) application.Description = package.Description;

		var entryPoint = package.Bin?.FirstOrDefault().Value ?? throw new EntryPointNotFoundException("Unable to determine the application entry point.");
		var program = GetPathFromEnvironment("node") ?? throw new EntryPointNotFoundException(@"Unable to locate the ""node"" program.");
		return (Program: program.FullName, EntryPoint: Path.GetFullPath(Path.Join(application.Path, entryPoint)));
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

	/// <summary>
	/// Starts the specified application.
	/// </summary>
	/// <param name="application">The application configuration.</param>
	private static void StartApplication(Application application) {
		using var serviceController = new ServiceController(application.Id);
		if (serviceController.Status == ServiceControllerStatus.Stopped) {
			serviceController.Start();
			serviceController.WaitForStatus(ServiceControllerStatus.Running);
		}
	}
}
