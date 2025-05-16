namespace Belin.Cli.Setup;

using Belin.Cli.Nssm;
using Microsoft.Extensions.Logging;
using System.CommandLine.Invocation;
using System.Net.Http.Json;
using System.ServiceProcess;

/// <summary>
/// Downloads and installs the latest Node.js release.
/// </summary>
public class NodeCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public NodeCommand(): base("node", "Set up the latest Node.js release.") {
		Add(new Option<FileInfo>(["-c", "--config"], "The path to the NSSM configuration file."));
		Add(new OutputOption(new DirectoryInfo(OperatingSystem.IsWindows() ? @"C:\Program Files\Node.js" : "/usr/local")));
	}

	/// <summary>
	/// The command handler.
	/// </summary>
	/// <param name="logger">The logging service.</aparam>
	public class CommandHandler(ILogger<NodeCommand> logger): ICommandHandler {

		/// <summary>
		/// The path to the NSSM configuration file.
		/// </summary>
		public FileInfo? Config { get; set; }

		/// <summary>
		/// The path to the output directory.
		/// </summary>
		public required DirectoryInfo Output { get; set; }

		/// <summary>
		/// The identifiers of the NSSM services to restart.
		/// </summary>
		private readonly List<string> services = [];

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
			services.Clear();
			if (Config is not null) {
				var nssm = NssmConfiguration.ReadFromFile(Config.FullName);
				if (nssm is not null) services.AddRange(nssm.Machines
					.Where(machine => machine.Name == Environment.MachineName)
					.SelectMany(machine => machine.Services.Select(service => service.Id)));
				else {
					logger.LogError("Unable to locate or parse the specified configuration file.");
					return 1;
				}
			}

			if (!this.CheckPrivilege(services.Count > 0 ? null : Output)) {
				logger.LogCritical("You must run this command in an elevated prompt.");
				return 2;
			}

			using var httpClient = SetupCommand.CreateHttpClient();
			var version = await FetchLatestVersion(httpClient);
			if (version is null) {
				logger.LogError("Unable to fetch the list of Node.js releases.");
				return 3;
			}

			var path = await DownloadArchive(httpClient, version);
			StopServices();
			logger.LogInformation(@"Extracting file ""{Input}"" into directory ""{Output}""...", path.Name, Output.Name);
			path.ExtractTo(Output, strip: 1);
			StartServices();

			logger.LogInformation("{Version}", SetupCommand.GetExecutableVersion(Output, "node"));
			return 0;
		}

		/// <summary>
		/// Downloads the PHP release corresponding to the specified version.
		/// </summary>
		/// <param name="httpClient">The HTTP client.</param>
		/// <param name="version">The version number of the PHP release to download.</param>
		/// <returns>The path to the downloaded ZIP archive.</returns>
		private async Task<FileInfo> DownloadArchive(HttpClient httpClient, Version version) {
			var (operatingSystem, fileExtension) = true switch {
				true when OperatingSystem.IsMacOS() => ("darwin", "tar.gz"),
				true when OperatingSystem.IsWindows() => ("win", "zip"),
				_ => ("linux", "tar.xz")
			};

			var file = $"node-v{version}-{operatingSystem}-x64.{fileExtension}";
			logger.LogInformation(@"Downloading file ""{File}""...", file);

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
		private async Task<Version?> FetchLatestVersion(HttpClient httpClient) {
			logger.LogInformation("Fetching the list of Node.js releases...");
			var releases = await httpClient.GetFromJsonAsync<List<NodeRelease>>("https://nodejs.org/dist/index.json");
			var latestRelease = releases?.FirstOrDefault();
			return latestRelease is not null ? new Version(latestRelease.Version[1..]) : null;
		}

		/// <summary>
		/// Starts all hosted NSSM services
		/// </summary>
		private void StartServices() {
			if (services.Count > 0) logger.LogInformation("Starting the NSSM services...");
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
			if (services.Count > 0) logger.LogInformation("Stopping the NSSM services...");
			foreach (var service in services) {
				using var serviceController = new ServiceController(service);
				if (serviceController.Status == ServiceControllerStatus.Running) {
					serviceController.Stop();
					serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
				}
			}
		}
	}
}

/// <summary>
/// Represents a Node.js release.
/// </summary>
/// <param name="Version">The version number.</param>
internal record NodeRelease(string Version);
