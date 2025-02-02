namespace Belin.Cli.CommandLine.Nssm;

using System.Diagnostics;
using System.ServiceProcess;

/// <summary>
/// Unregisters the Windows service.
/// </summary>
public class RemoveCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public RemoveCommand(): base("remove", "Unregister the Windows service.") {
		var workingDirectory = new DirectoryInfo(Environment.CurrentDirectory);
		var directoryArgument = new Argument<DirectoryInfo>(
			name: "directory",
			description: "The path to the root directory of the Node.js application.",
			getDefaultValue: () => workingDirectory
		);

		Add(directoryArgument);
		this.SetHandler(Execute, directoryArgument);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <param name="directory">The path to the root directory of the Node.js application.</param>
	/// <returns>The exit code.</returns>
	/// <exception cref="Exception">An error occurred when starting the NSSM program.</exception>
	private Task<int> Execute(DirectoryInfo directory) {
		if (!this.CheckPrivilege()) return Task.FromResult(1);

		var config = ApplicationConfiguration.ReadFromDirectory(directory);
		using var serviceController = new ServiceController(config.Id);
		if (serviceController.Status == ServiceControllerStatus.Running) {
			serviceController.Stop();
			serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
		}

		using var process = Process.Start("nssm.exe", ["remove", config.Id, "confirm"])
			?? throw new Exception(@"The ""nssm.exe"" program could not be started.");
		process.WaitForExit();
		return Task.FromResult(process.ExitCode);
	}
}
