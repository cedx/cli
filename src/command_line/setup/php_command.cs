namespace Belin.Cli.CommandLine.Setup;

using Microsoft.Win32;
using System.Net.Http.Json;
using System.ServiceProcess;

/// <summary>
/// Downloads and installs the latest PHP release.
/// </summary>
public class PhpCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public PhpCommand(): base("php", "Set up the latest PHP release.") {
		var outputOption = new OutputOption(new DirectoryInfo(@"C:\Program Files\PHP"));
		Add(outputOption);
		this.SetHandler(Execute, outputOption);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <param name="output">The path to the output directory.</param>
	/// <returns>The exit code.</returns>
	public async Task<int> Execute(DirectoryInfo output) {
		if (!this.CheckPrivilege()) return 1;

		using var httpClient = this.CreateHttpClient();
		var version = await FetchLatestVersion(httpClient);
		if (version == null) {
			Console.WriteLine("Unable to fetch the list of PHP releases.");
			return 2;
		}

		var path = await DownloadArchive(httpClient, version);
		using var serviceController = new ServiceController("W3SVC");
		StopWebServer(serviceController);
		this.ExtractZipFile(path, output);
		StartWebServer(serviceController);
		RegisterEventLog(version, output);

		Console.WriteLine(this.GetExecutableVersion(output, "php.exe"));
		return 0;
	}

	/// <summary>
	/// Downloads the PHP release corresponding to the specified version.
	/// </summary>
	/// <param name="httpClient">The HTTP client.</param>
	/// <param name="version">The version number of the PHP release to download.</param>
	/// <returns>The path to the downloaded ZIP archive.</returns>
	private static async Task<FileInfo> DownloadArchive(HttpClient httpClient, Version version) {
		var vs = version >= new Version("8.4.0") ? "vs17" : "vs16";
		var file = $"php-{version}-nts-Win32-{vs}-x64.zip";
		Console.WriteLine($"Downloading file \"{file}\"...");

		var bytes = await httpClient.GetByteArrayAsync($"https://windows.php.net/downloads/releases/{file}");
		var path = Path.Join(Path.GetTempPath(), file);
		File.WriteAllBytes(path, bytes);
		return new FileInfo(path);
	}

	/// <summary>
	/// Fetches the latest version number of the PHP releases.
	/// </summary>
	/// <param name="httpClient">The HTTP client.</param>
	/// <returns>The version number of the latest PHP release, or <see langword="null"/> if not found.</returns>
	private static async Task<Version?> FetchLatestVersion(HttpClient httpClient) {
		Console.WriteLine("Fetching the list of PHP releases...");
		var releases = await httpClient.GetFromJsonAsync<Dictionary<string, PhpRelease>>("https://www.php.net/releases/?json");
		var latestRelease = releases?.FirstOrDefault().Value;
		return latestRelease != null ? new Version(latestRelease.Version) : null;
	}

	/// <summary>
	/// Registers the PHP interpreter with the event log.
	/// </summary>
	/// <param name="version">The version number of the PHP release to register.</param>
	/// <param name="output">The path to the output directory.</param>
	private static void RegisterEventLog(Version version, DirectoryInfo output) {
		Console.WriteLine("Registering the PHP interpreter with the event log...");
		var key = $"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\EventLog\\Application\\PHP-{version}";
		Registry.SetValue(key, "EventMessageFile", Path.Join(output.FullName, $"php{version.Major}.dll"), RegistryValueKind.String);
		Registry.SetValue(key, "TypesSupported", 7, RegistryValueKind.DWord);
	}

	/// <summary>
	/// Starts the IIS web server.
	/// </summary>
	/// <param name="serviceController">The service controller.</param>
	private static void StartWebServer(ServiceController serviceController) {
		if (serviceController.Status == ServiceControllerStatus.Stopped) {
			Console.WriteLine("Starting the IIS web server...");
			serviceController.Start();
			serviceController.WaitForStatus(ServiceControllerStatus.Running);
		}
	}

	/// <summary>
	/// Stops the IIS web server.
	/// </summary>
	/// <param name="serviceController">The service controller.</param>
	private static void StopWebServer(ServiceController serviceController) {
		if (serviceController.Status == ServiceControllerStatus.Running) {
			Console.WriteLine("Stopping the IIS web server...");
			serviceController.Stop();
			serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
		}
	}
}

/// <summary>
/// Represents a PHP release.
/// </summary>
internal class PhpRelease {

	/// <summary>
	/// The version number.
	/// </summary>
	public string Version { get; set; } = string.Empty;
}
