namespace Belin.Cli.Nssm;

using System.Diagnostics;
using System.ServiceProcess;

/// <summary>
/// Unregisters the Windows service.
/// </summary>
public sealed class RemoveCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public RemoveCommand(): base("remove", "Unregister the Windows service.") {
		var directoryArgument = new Argument<DirectoryInfo>(
			name: "directory",
			description: "The path to the root directory of the Node.js application.",
			getDefaultValue: () => new DirectoryInfo(Environment.CurrentDirectory)
		);

		Add(directoryArgument);
		this.SetHandler(Execute, directoryArgument);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <param name="directory">The path to the root directory of the Node.js application.</param>
	/// <returns>The exit code.</returns>
	public async Task<int> Execute(DirectoryInfo directory) {
		if (!this.CheckPrivilege()) return 1;

		var application = ApplicationConfiguration.ReadFromDirectory(directory);
		if (application is null) {
			Console.WriteLine("Unable to locate the application configuration file.");
			return 2;
		}

		using var serviceController = new ServiceController(application.Id);
		if (serviceController.Status == ServiceControllerStatus.Running) {
			serviceController.Stop();
			serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
		}

		using var process = Process.Start("nssm", ["remove", application.Id, "confirm"]);
		if (process is not null) process.WaitForExit();
		else {
			Console.WriteLine(@"The ""nssm"" program could not be started.");
			return 3;
		}

		return await Task.FromResult(process.ExitCode);
	}
}
