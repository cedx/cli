namespace Belin.Cli.Setup;

using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.CommandLine.Invocation;
using System.Net.Http.Json;
using System.ServiceProcess;

/// <summary>
/// Downloads and installs the latest PHP release.
/// </summary>
public class PhpCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public PhpCommand(): base("php", "Set up the latest PHP release.") =>
		Add(new OutputOption(new DirectoryInfo(OperatingSystem.IsWindows() ? @"C:\Program Files\PHP" : "/usr/local")));

	/// <summary>
	/// The command handler.
	/// </summary>
	/// <param name="logger">The logging service.</aparam>
	/// <param name="httpClient">The HTTP client.</param>
	public class CommandHandler(ILogger<PhpCommand> logger, HttpClient httpClient): ICommandHandler {

		/// <summary>
		/// The path to the output directory.
		/// </summary>
		public required DirectoryInfo Out { get; set; }

		/// <summary>
		/// Invokes this command.
		/// </summary>
		/// <param name="context">The invocation context.</param>
		/// <returns>The exit code.</returns>
		public int Invoke(InvocationContext context) => InvokeAsync(context).Result;

		/// <summary>
		/// Invokes this command.
		/// </summary>
		/// <param name="context">The invocation context.</param>
		/// <returns>The exit code.</returns>
		public async Task<int> InvokeAsync(InvocationContext context) {
			if (!this.CheckPrivilege()) {
				logger.LogError("You must run this command in an elevated prompt.");
				return 1;
			}

			var version = await FetchLatestVersion();
			if (version is null) {
				logger.LogError("Unable to fetch the list of PHP releases.");
				return 2;
			}

			var path = await DownloadArchive(version);
			using var serviceController = new ServiceController("W3SVC");
			StopWebServer(serviceController);
			logger.LogInformation(@"Extracting file ""{Input}"" into directory ""{Output}""...", path.Name, Out.Name);
			path.ExtractTo(Out);
			StartWebServer(serviceController);
			RegisterEventLog(version);

			logger.LogInformation("{Version}", this.GetExecutableVersion(Out, "php"));
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
			logger.LogInformation(@"Downloading file ""{File}""...", file);

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
			logger.LogInformation("Fetching the list of PHP releases...");
			var releases = await httpClient.GetFromJsonAsync<Dictionary<string, PhpRelease>>("https://www.php.net/releases/?json");
			var latestRelease = releases?.FirstOrDefault().Value;
			return latestRelease is not null ? new Version(latestRelease.Version) : null;
		}

		/// <summary>
		/// Registers the PHP interpreter with the event log.
		/// </summary>
		/// <param name="version">The version number of the PHP release to register.</param>
		private void RegisterEventLog(Version version) {
			logger.LogInformation("Registering the PHP interpreter with the event log...");
			var key = $"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\EventLog\\Application\\PHP-{version}";
			Registry.SetValue(key, "EventMessageFile", Path.Join(Out.FullName, $"php{version.Major}.dll"), RegistryValueKind.String);
			Registry.SetValue(key, "TypesSupported", 7, RegistryValueKind.DWord);
		}

		/// <summary>
		/// Starts the IIS web server.
		/// </summary>
		/// <param name="serviceController">The service controller.</param>
		private void StartWebServer(ServiceController serviceController) {
			if (serviceController.Status == ServiceControllerStatus.Stopped) {
				logger.LogInformation("Starting the IIS web server...");
				serviceController.Start();
				serviceController.WaitForStatus(ServiceControllerStatus.Running);
			}
		}

		/// <summary>
		/// Stops the IIS web server.
		/// </summary>
		/// <param name="serviceController">The service controller.</param>
		private void StopWebServer(ServiceController serviceController) {
			if (serviceController.Status == ServiceControllerStatus.Running) {
				logger.LogInformation("Stopping the IIS web server...");
				serviceController.Stop();
				serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
			}
		}
	}
}

/// <summary>
/// Represents a PHP release.
/// </summary>
/// <param name="Version">The version number.</param>
internal record PhpRelease(string Version);
