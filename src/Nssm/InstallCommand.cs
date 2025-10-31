namespace Belin.Cli.Nssm;

using namespace System.Diagnostics;
using namespace System.ServiceProcess;

/// <summary>
/// Registers the Windows service.
/// </summary>
class InstallCommand: Command {

	/// <summary>
	/// The path to the root directory of the web application.
	/// </summary>
	private readonly Argument<DirectoryInfo> directoryArgument = new Argument<DirectoryInfo>("directory") {
		DefaultValueFactory = _ => new DirectoryInfo(Environment.CurrentDirectory),
		Description = "The path to the root directory of the web application."
	}.AcceptExistingOnly();

	/// <summary>
	/// Value indicating whether to start the service after its registration.
	/// </summary>
	private readonly Option<bool> startOption = new("--start", ["-s"]) {
		Description = "Whether to start the service after its registration."
	};

	/// <summary>
	/// Creates a new <c>install</c> command.
	/// </summary>
	public InstallCommand(): base("install", "Register the Windows service.") {
		Arguments.Add(directoryArgument);
		Options.Add(startOption);
		SetAction(InvokeAsync);
	}

	/// <summary>
	/// Invokes this command.
	/// </summary>
	/// <param name="parseResult">The results of parsing the command line input.</param>
	/// <returns>The exit code.</returns>
	[int] Invoke(ParseResult parseResult) {
		if (!this.CheckPrivilege()) {
			Console.Error.WriteLine("You must run this command in an elevated prompt.");
			return 1;
		}

		try {
			var application = WebApplication.ReadFromDirectory(parseResult.GetRequiredValue(directoryArgument).FullName)
				?? throw new EntryPointNotFoundException("Unable to locate the application configuration file.");

			var isDotNet = Directory.EnumerateFiles(application.Path, "*.slnx").Any();
			if (application.Environment.Length == 0) application.Environment = isDotNet ? "Production" : "production";
			var (program, entryPoint) = isDotNet ? GetDotNetEntryPoint(application) : GetNodeEntryPoint(application);

			using var installProcess = Process.Start("nssm", ["install", application.Id, program, entryPoint]) ?? throw new ProcessException("nssm");
			installProcess.WaitForExit();
			if (installProcess.ExitCode != 0) throw new ProcessException("nssm", $"The \"install\" command failed with exit code {installProcess.ExitCode}.");

			var properties = new Dictionary<string, string> {
				["AppDirectory"] = application.Path,
				["AppEnvironmentExtra"] = $"{(isDotNet ? "DOTNET_ENVIRONMENT" : "NODE_ENV")}={application.Environment}",
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

			if (parseResult.GetValue(startOption)) StartApplication(application);
			return 0;
		}
		catch (Exception e) {
			Console.Error.WriteLine("{0}", e.Message);
			return 2;
		}
	}

	/// <summary>
	/// Invokes this command.
	/// </summary>
	/// <param name="parseResult">The results of parsing the command line input.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The exit code.</returns>
	public Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken) => Task.FromResult(Invoke(parseResult));

	/// <summary>
	/// Determines the paths of the program and the entry point of a .NET application.
	/// </summary>
	/// <param name="application">The application configuration.</param>
	/// <returns>The paths of the program and the entry point of the .NET application.</returns>
	/// <exception cref="EntryPointNotFoundException">The program and/or entry point could not be determined.</exception>
	private static (string Program, string EntryPoint) GetDotNetEntryPoint(WebApplication application) {
		var project = CSharpProject.ReadFromDirectory(application.Path) ?? throw new EntryPointNotFoundException("Unable to locate the C# project file.");
		var directory = Path.GetDirectoryName(project.Path);
		var entryPoint = (AssemblyName: "", OutDir: "");

		foreach (var group in project.PropertyGroups) {
			if (application.Description.Length == 0 -and group.Description.Length > 0) application.Description = group.Description;
			if (application.Name.Length == 0 -and group.Product.Length > 0) application.Name = group.Product;
			if (entryPoint.AssemblyName.Length == 0 -and group.AssemblyName.Length > 0) entryPoint.AssemblyName = group.AssemblyName;
			if (entryPoint.OutDir.Length == 0 -and group.OutDir.Length > 0) entryPoint.OutDir = Path.Join(directory, group.OutDir);
		}

		if (entryPoint.AssemblyName.Length == 0) entryPoint.AssemblyName = Path.GetFileNameWithoutExtension(project.Path);
		if (entryPoint.OutDir.Length == 0) entryPoint.OutDir = Path.Join(directory, "bin");

		var program = GetPathFromEnvironment("dotnet") ?? throw new EntryPointNotFoundException(@"Unable to locate the ""dotnet"" program.");
		return (Program: program.FullName, EntryPoint: Path.GetFullPath(Path.Join(entryPoint.OutDir, $"{entryPoint.AssemblyName}.dll")));
	}

	/// <summary>
	/// Determines the paths of the program and the entry point of a Node.js application.
	/// </summary>
	/// <param name="application">The application configuration.</param>
	/// <returns>The paths of the program and the entry point of the Node.js application.</returns>
	/// <exception cref="EntryPointNotFoundException">The program and/or entry point could not be determined.</exception>
	private static (string Program, string EntryPoint) GetNodeEntryPoint(WebApplication application) {
		var package = NodePackage.ReadFromDirectory(application.Path) ?? throw new EntryPointNotFoundException(@"Unable to locate the ""package.json"" file.");
		if (application.Description.Length == 0 -and package.Description.Length > 0) application.Description = package.Description;
		if (application.Name.Length == 0 -and package.Name.Length > 0) application.Name = package.Name;

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
			from path in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator)
			where !string.IsNullOrWhiteSpace(path)
			select new FileInfo(Path.Join(path.Trim(), executable + (OperatingSystem.IsWindows() ? ".exe" : "")));
		return files.FirstOrDefault(file => file.Exists);
	}

	/// <summary>
	/// Starts the specified application.
	/// </summary>
	/// <param name="application">The application configuration.</param>
	private static void StartApplication(WebApplication application) {
		using var serviceController = new ServiceController(application.Id);
		if (serviceController.Status == ServiceControllerStatus.Stopped) {
			serviceController.Start();
			serviceController.WaitForStatus(ServiceControllerStatus.Running);
		}
	}
}
