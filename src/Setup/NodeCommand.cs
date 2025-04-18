namespace Belin.Cli.Setup;

using System.Net.Http.Json;
using System.ServiceProcess;
using System.Text.Json;

/// <summary>
/// Downloads and installs the latest Node.js release.
/// </summary>
public sealed class NodeCommand: Command {

	/// <summary>
	/// The identifiers of the NSSM services to restart.
	/// </summary>
	private readonly List<string> services = [];

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public NodeCommand(): base("node", "Set up the latest Node.js release.") {
		var configOption = new Option<FileInfo>(["-c", "--config"], "The path to the NSSM configuration file.");
		var outputOption = new OutputOption(new DirectoryInfo(OperatingSystem.IsWindows() ? @"C:\Program Files\Node.js" : "/usr/local"));

		Add(configOption);
		Add(outputOption);
		this.SetHandler(Invoke, outputOption, configOption);
	}

	/// <summary>
	/// Invokes this command.
	/// </summary>
	/// <param name="output">The path to the output directory.</param>
	/// <param name="config">The path to the NSSM configuration file.</param>
	/// <returns>The exit code.</returns>
	public async Task<int> Invoke(DirectoryInfo output, FileInfo? config) {
		services.Clear();
		if (config is not null) {
			var serviceIds = ReadNssmConfiguration(config);
			if (serviceIds is not null) services.AddRange(serviceIds);
			else {
				Console.WriteLine("Unable to locate or parse the specified configuration file.");
				return 1;
			}
		}

		if (!this.CheckPrivilege(services.Count > 0 ? null : output)) return 2;

		using var httpClient = SetupCommand.CreateHttpClient();
		var version = await FetchLatestVersion(httpClient);
		if (version is null) {
			Console.WriteLine("Unable to fetch the list of Node.js releases.");
			return 3;
		}

		var path = await DownloadArchive(httpClient, version);
		StopServices();
		path.ExtractTo(output, strip: 1);
		StartServices();

		Console.WriteLine(SetupCommand.GetExecutableVersion(output, "node"));
		return 0;
	}

	/// <summary>
	/// Downloads the PHP release corresponding to the specified version.
	/// </summary>
	/// <param name="httpClient">The HTTP client.</param>
	/// <param name="version">The version number of the PHP release to download.</param>
	/// <returns>The path to the downloaded ZIP archive.</returns>
	private static async Task<FileInfo> DownloadArchive(HttpClient httpClient, Version version) {
		var (operatingSystem, fileExtension) = true switch {
			true when OperatingSystem.IsMacOS() => ("darwin", "tar.gz"),
			true when OperatingSystem.IsWindows() => ("win", "zip"),
			_ => ("linux", "tar.xz")
		};

		var file = $"node-v{version}-{operatingSystem}-x64.{fileExtension}";
		Console.WriteLine($"Downloading file \"{file}\"...");

		var bytes = await httpClient.GetByteArrayAsync($"https://nodejs.org/dist/v{version}/{file}");
		var path = Path.Join(Path.GetTempPath(), file);
		File.WriteAllBytes(path, bytes);
		return new FileInfo(path);
	}

	/// <summary>
	/// Fetches the latest version number of the Node.js releases.
	/// </summary>
	/// <param name="httpClient">The HTTP client.</param>
	/// <returns>The version number of the latest Node.js release, or <see langword="null"/> if not found.</returns>
	private static async Task<Version?> FetchLatestVersion(HttpClient httpClient) {
		Console.WriteLine("Fetching the list of Node.js releases...");
		var releases = await httpClient.GetFromJsonAsync<List<NodeRelease>>("https://nodejs.org/dist/index.json");
		var latestRelease = releases?.FirstOrDefault();
		return latestRelease is not null ? new Version(latestRelease.Version[1..]) : null;
	}

	/// <summary>
	/// Reads the NSSM configuration file located at the specified path.
	/// </summary>
	/// <param name="file">The path to the NSSM configuration file.</param>
	/// <returns>The list of parsed service identifiers or <see langword="null"/> if an error occurred.</returns>
	private static string[]? ReadNssmConfiguration(FileInfo file) {
		if (!file.Exists) return null;
		var map = JsonSerializer.Deserialize<Dictionary<string, string[]>>(File.ReadAllText(file.FullName));
		return (map?.TryGetValue(Environment.MachineName, out var serviceIds) ?? false) ? serviceIds : null;
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
internal sealed record NodeRelease(string Version);
