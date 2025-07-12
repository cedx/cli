namespace Belin.Cli.Setup;

using Microsoft.Win32;
using System.Net.Http.Json;
using System.ServiceProcess;

/// <summary>
/// Downloads and installs the latest PHP release.
/// </summary>
public class PhpCommand: Command {

	/// <summary>
	/// The path to the output directory.
	/// </summary>
	private readonly OutputOption outputOption = new(new DirectoryInfo(OperatingSystem.IsWindows() ? @"C:\Program Files\PHP" : "/usr/local"));

	/// <summary>
	/// The HTTP client.
	/// </summary>
	private readonly HttpClient httpClient;

	/// <summary>
	/// Creates a new <c>php</c> command.
	/// </summary>
	/// <param name="httpClient">The HTTP client.</param>
	public PhpCommand(HttpClient httpClient): base("php", "Set up the latest PHP release.") {
		this.httpClient = httpClient;
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
		if (!this.CheckPrivilege()) {
			Console.Error.WriteLine("You must run this command in an elevated prompt.");
			return 1;
		}

		var version = await FetchLatestVersion();
		if (version is null) {
			Console.Error.WriteLine("Unable to fetch the list of PHP releases.");
			return 2;
		}

		var path = await DownloadArchive(version);
		using var serviceController = new ServiceController("W3SVC");
		StopWebServer(serviceController);

		var output = parseResult.GetRequiredValue(outputOption);
		Console.WriteLine(@"Extracting file ""{0}"" into directory ""{1}""...", path.Name, output.Name);
		path.ExtractTo(output);

		StartWebServer(serviceController);
		RegisterEventLog(version, output);

		Console.WriteLine("{0}", this.GetExecutableVersion(output, "php"));
		return 0;
	}

	/// <summary>
	/// Downloads the PHP release corresponding to the specified version.
	/// </summary>
	/// <param name="version">The version number of the PHP release to download.</param>
	/// <returns>The path to the downloaded ZIP archive.</returns>
	private async Task<FileInfo> DownloadArchive(Version version) {
		var vs = version >= new Version("8.4.0") ? "vs17" : "vs16";
		var file = $"php-{version}-nts-Win32-{vs}-x64.zip";
		Console.WriteLine(@"Downloading file ""{0}""...", file);

		var bytes = await httpClient.GetByteArrayAsync($"https://windows.php.net/downloads/releases/{file}");
		var path = Path.Join(Path.GetTempPath(), file);
		File.WriteAllBytes(path, bytes);
		return new FileInfo(path);
	}

	/// <summary>
	/// Fetches the latest version number of the PHP releases.
	/// </summary>
	/// <returns>The version number of the latest PHP release, or <see langword="null"/> if not found.</returns>
	private async Task<Version?> FetchLatestVersion() {
		Console.WriteLine("Fetching the list of PHP releases...");
		var releases = await httpClient.GetFromJsonAsync<Dictionary<string, PhpRelease>>("https://www.php.net/releases/?json");
		var latestRelease = releases?.FirstOrDefault().Value;
		return latestRelease is not null ? new Version(latestRelease.Version) : null;
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
/// <param name="Version">The version number.</param>
internal record PhpRelease(string Version);
