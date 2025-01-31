namespace Belin.Cli.Commands;

using Belin.Diagnostics;
using Belin.Net.Http;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO.Compression;
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
		var outputOption = new OutputOption(new DirectoryInfo(@"C:\Program Files\PHP"));
		Add(outputOption);
		this.SetHandler(Execute, outputOption);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <param name="output">The path to the output directory.</param>
	private async Task Execute(DirectoryInfo output) {
		using var httpClient = HttpClientFactory.CreateClient();
		var version = await FetchLatestVersion(httpClient);
		var path = await DownloadArchive(httpClient, version);
		ExtractArchive(path, output);
		RegisterEventLog(version, output);

		using var process = new Process() { StartInfo = { FileName = Path.Join(output.FullName, "php.exe") } };
		Console.WriteLine(process.GetVersion());
		return 0;
	}

	/// <summary>
	/// Downloads the PHP release corresponding to the specified version.
	/// </summary>
	/// <param name="httpClient">The HTTP client.</param>
	/// <param name="version">The version number of the PHP release to download.</param>
	/// <returns>The path to the downloaded ZIP archive.</returns>
	private static async Task<string> DownloadArchive(HttpClient httpClient, Version version) {
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
	private static void ExtractArchive(string path, DirectoryInfo output) {
		using var serviceController = new ServiceController("W3SVC");
		StopWebServer(serviceController);

		Console.WriteLine($"Extracting file \"{Path.GetFileName(path)}\" into directory \"{output.FullName}\"...");
		using var zipArchive = ZipFile.OpenRead(path);
		zipArchive.ExtractToDirectory(output.FullName, overwriteFiles: true);

		StartWebServer(serviceController);
	}

	/// <summary>
	/// Fetches the latest version number of the PHP releases.
	/// </summary>
	/// <param name="httpClient">The HTTP client.</param>
	/// <returns>The version number of the latest PHP release.</returns>
	private static async Task<Version> FetchLatestVersion(HttpClient httpClient) {
		Console.WriteLine("Fetching the list of PHP releases...");
		using var jsonDocument = JsonDocument.Parse(await httpClient.GetStringAsync("https://www.php.net/releases/?json"));
		return new Version(jsonDocument.RootElement.EnumerateObject().FirstOrDefault().Value.GetProperty("version").ToString());
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
