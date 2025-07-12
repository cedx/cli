namespace Belin.Cli.Setup;

using Belin.Cli.Nssm;
using System.Net.Http.Json;
using System.ServiceProcess;

/// <summary>
/// Downloads and installs the latest Node.js release.
/// </summary>
public class NodeCommand: Command {

	/// <summary>
	/// The path to the NSSM configuration file.
	/// </summary>
	private readonly Option<FileInfo> configOption = new("--config", ["-c"]) {
		Description = "The path to the NSSM configuration file."
	};

	/// <summary>
	/// The path to the output directory.
	/// </summary>
	private readonly OutputOption outputOption = new(new DirectoryInfo(OperatingSystem.IsWindows() ? @"C:\Program Files\Node.js" : "/usr/local"));

	/// <summary>
	/// The HTTP client.
	/// </summary>
	private readonly HttpClient httpClient;

	/// <summary>
	/// The identifiers of the NSSM services to restart.
	/// </summary>
	private readonly List<string> services = [];

	/// <summary>
	/// Creates a new command.
	/// </summary>
	/// <param name="httpClient">The HTTP client.</param>
	public NodeCommand(HttpClient httpClient): base("node", "Set up the latest Node.js release.") {
		this.httpClient = httpClient;
		Options.Add(configOption);
		Options.Add(outputOption);
		SetAction(InvokeAsync);
	}

	/// <summary>
	/// Invokes this command.
	/// </summary>
	/// <param name="parseResult">The results of parsing the command line input.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The exit code.</returns>
	public async Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken) {
		services.Clear();
		if (parseResult.GetValue(configOption) is FileInfo config) {
			var nssm = NssmConfiguration.ReadFromFile(config.FullName);
			if (nssm is not null) services.AddRange(nssm.Machines
				.Where(machine => machine.Name == Environment.MachineName)
				.SelectMany(machine => machine.Services.Select(service => service.Id)));
			else {
				Console.Error.WriteLine("Unable to locate or parse the specified configuration file.");
				return 1;
			}
		}

		var output = parseResult.GetValue(outputOption)!;
		if (!this.CheckPrivilege(services.Count > 0 ? null : output)) {
			Console.Error.WriteLine("You must run this command in an elevated prompt.");
			return 2;
		}

		var version = await FetchLatestVersion();
		if (version is null) {
			Console.Error.WriteLine("Unable to fetch the list of Node.js releases.");
			return 3;
		}

		var path = await DownloadArchive(version);
		StopServices();
		Console.WriteLine(@"Extracting file ""{0}"" into directory ""{1}""...", path.Name, output.Name);
		path.ExtractTo(output, strip: 1);
		StartServices();

		Console.WriteLine("{0}", this.GetExecutableVersion(output, "node"));
		return 0;
	}

	/// <summary>
	/// Downloads the PHP release corresponding to the specified version.
	/// </summary>
	/// <param name="version">The version number of the PHP release to download.</param>
	/// <returns>The path to the downloaded ZIP archive.</returns>
	private async Task<FileInfo> DownloadArchive(Version version) {
		var (operatingSystem, fileExtension) = true switch {
			true when OperatingSystem.IsMacOS() => ("darwin", "tar.gz"),
			true when OperatingSystem.IsWindows() => ("win", "zip"),
			_ => ("linux", "tar.xz")
		};

		var file = $"node-v{version}-{operatingSystem}-x64.{fileExtension}";
		Console.WriteLine(@"Downloading file ""{0}""...", file);

		var bytes = await httpClient.GetByteArrayAsync($"https://nodejs.org/dist/v{version}/{file}");
		var path = Path.Join(Path.GetTempPath(), file);
		File.WriteAllBytes(path, bytes);
		return new FileInfo(path);
	}

	/// <summary>
	/// Fetches the latest version number of the Node.js releases.
	/// </summary>
	/// <returns>The version number of the latest Node.js release, or <see langword="null"/> if not found.</returns>
	private async Task<Version?> FetchLatestVersion() {
		Console.WriteLine("Fetching the list of Node.js releases...");
		var releases = await httpClient.GetFromJsonAsync<List<NodeRelease>>("https://nodejs.org/dist/index.json");
		var latestRelease = releases?.FirstOrDefault();
		return latestRelease is not null ? new Version(latestRelease.Version[1..]) : null;
	}

	/// <summary>
	/// Starts all hosted NSSM services
	/// </summary>
	private void StartServices() {
		if (services.Count > 0) Console.WriteLine("Starting the NSSM services...");
		foreach (var service in services) {
			using var serviceController = new ServiceController(service);
			if (serviceController.Status == ServiceControllerStatus.Stopped) {
				serviceController.Start();
				serviceController.WaitForStatus(ServiceControllerStatus.Running);
			}
		}
	}

	/// <summary>
	/// Stops all hosted NSSM services.
	/// </summary>
	private void StopServices() {
		if (services.Count > 0) Console.WriteLine("Stopping the NSSM services...");
		foreach (var service in services) {
			using var serviceController = new ServiceController(service);
			if (serviceController.Status == ServiceControllerStatus.Running) {
				serviceController.Stop();
				serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
			}
		}
	}
}

/// <summary>
/// Represents a Node.js release.
/// </summary>
/// <param name="Version">The version number.</param>
internal record NodeRelease(string Version);
