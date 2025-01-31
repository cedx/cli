namespace Belin.Cli.Commands;

using Microsoft.Win32;
using System.IO.Compression;
using System.Reflection;
using System.ServiceProcess;
using System.Text.Json;
using System.Threading.Tasks;

/// <summary>
/// Downloads and installs the latest PHP release.
/// </summary>
public class PhpCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public PhpCommand(): base("php", "Download and install the latest PHP release.") {
		var outputOption = new OutputOption(OperatingSystem.IsWindows() ? @"C:\Program Files\PHP" : "/usr/local");
		Add(outputOption);
		this.SetHandler(Execute, outputOption);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <exception cref="PlatformNotSupportedException">This command only supports the Windows platform.</exception>
	private async Task Execute(string output) {
		if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException("This command only supports the Windows platform.");

		var fileVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>();
		var userAgent = $".NET/{Environment.Version} | Belin.io/{new Version(fileVersion!.Version).ToString(3)}";

		using var httpClient = new HttpClient();
		httpClient.DefaultRequestHeaders.Add("user-agent", userAgent);

		var version = await FetchLatestVersion(httpClient);
		var path = await DownloadArchive(httpClient, version);
		ExtractArchive(path, output);
		RegisterEventLog(version, output);
	}

	/// <summary>
	/// Downloads the PHP release corresponding to the specified version.
	/// </summary>
	/// <param name="httpClient">The HTTP client.</param>
	/// <param name="version">The version number of the PHP release to download.</param>
	/// <returns>The path to the downloaded ZIP archive.</returns>
	private async Task<string> DownloadArchive(HttpClient httpClient, Version version) {
		var vs = version >= new Version("8.4.0") ? "vs17" : "vs16";
		var path = Path.Join(Path.GetTempPath(), $"php-{version}-nts-Win32-{vs}-x64.zip");
		var file = Path.GetFileName(path);

		Console.WriteLine($"Downloading file \"{file}\"...");
		var bytes = await httpClient.GetByteArrayAsync($"https://windows.php.net/downloads/releases/{file}");
		File.WriteAllBytes(path, bytes);
		return path;
	}

	/// <summary>
	/// Extracts the ZIP archive located at the specified path.
	/// </summary>
	/// <param name="path">The path to the downloaded ZIP archive.</param>
	/// <param name="output">The path to the output directory.</param>
	private void ExtractArchive(string path, string output) {
		using var serviceController = new ServiceController("W3SVC");
		StopWebServer(serviceController);
		Console.WriteLine($"Extracting file \"{Path.GetFileName(path)}\" into directory \"{Path.GetFullPath(output)}\"...");
		// TODO ???? remove the destination directory??? Beware of customizations: php.ini, *.ps1 files, etc....
		ZipFile.ExtractToDirectory(path, output);
		StartWebServer(serviceController);
	}

	/// <summary>
	/// Fetches the latest version number of the PHP releases.
	/// </summary>
	/// <param name="httpClient">The HTTP client.</param>
	/// <returns>The version number of the latest PHP release.</returns>
	private async Task<Version> FetchLatestVersion(HttpClient httpClient) {
		Console.WriteLine("Fetching the list of PHP releases...");
		using var jsonDocument = JsonDocument.Parse(await httpClient.GetStringAsync("https://www.php.net/releases/?json"));
		return new Version(jsonDocument.RootElement.EnumerateObject().FirstOrDefault().Value.GetProperty("version").ToString());
	}

	/// <summary>
	/// Registers the PHP interpreter with the event log.
	/// </summary>
	/// <param name="version">The version number of the PHP release to register.</param>
	/// <param name="output">The path to the output directory.</param>
	private void RegisterEventLog(Version version, string output) {
		Console.WriteLine("Registering the PHP interpreter with the event log...");
		var key = $"HKLM\\SYSTEM\\CurrentControlSet\\Services\\Eventlog\\Application\\PHP-{version}";
		Registry.SetValue(key, "EventMessageFile", Path.Join(output, $"php{version.Major}.dll"), RegistryValueKind.String);
		Registry.SetValue(key, "TypesSupported", 7, RegistryValueKind.DWord);
	}

	/// <summary>
	/// Starts the IIS web server.
	/// </summary>
	/// <param name="serviceController">The service controller.</param>
	private void StartWebServer(ServiceController serviceController) {
		if (serviceController.Status != ServiceControllerStatus.Stopped) {
			serviceController.Start();
			serviceController.WaitForStatus(ServiceControllerStatus.Running);
		}
	}

	/// <summary>
	/// Stops the IIS web server.
	/// </summary>
	/// <param name="serviceController">The service controller.</param>
	private void StopWebServer(ServiceController serviceController) {
		if (serviceController.Status != ServiceControllerStatus.Running) {
			Console.WriteLine("Stopping the IIS web server...");
			serviceController.Stop();
			serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
		}
	}
}
