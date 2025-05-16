namespace Belin.Cli.Nssm;

using Microsoft.Extensions.Logging;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.ServiceProcess;

/// <summary>
/// Unregisters the Windows service.
/// </summary>
public class RemoveCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public RemoveCommand(): base("remove", "Unregister the Windows service.") =>
		Add(new Argument<DirectoryInfo>(
			name: "directory",
			description: "The path to the root directory of the .NET or Node.js application.",
			getDefaultValue: () => new DirectoryInfo(Environment.CurrentDirectory)
		));

	/// <summary>
	/// The command handler.
	/// </summary>
	/// <param name="logger">The logging service.</aparam>
	public class CommandHandler(ILogger<RemoveCommand> logger): ICommandHandler {

		/// <summary>
		/// The path to the root directory of the .NET or Node.js application.
		/// </summary>
		public required DirectoryInfo Directory { get; set; }

		/// <summary>
		/// Invokes this command.
		/// </summary>
		/// <param name="context">The invocation context.</param>
		/// <returns>The exit code.</returns>
		public int Invoke(InvocationContext context) {
			if (!this.CheckPrivilege()) {
				logger.LogError("You must run this command in an elevated prompt.");
				return 1;
			}

			var application = WebApplication.ReadFromDirectory(Directory.FullName);
			if (application is null) {
				logger.LogError("Unable to locate the application configuration file.");
				return 2;
			}

			using var serviceController = new ServiceController(application.Id);
			if (serviceController.Status == ServiceControllerStatus.Running) {
				serviceController.Stop();
				serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
			}

			try {
				using var process = Process.Start("nssm", ["remove", application.Id, "confirm"]) ?? throw new ProcessException("nssm");
				process.WaitForExit();
				if (process.ExitCode != 0) throw new ProcessException("nssm", $"The process failed with exit code {process.ExitCode}.");
				return 0;
			}
			catch (Exception e) {
				logger.LogError("{Message}", e.Message);
				return 3;
			}
		}

		/// <summary>
		/// Invokes this command.
		/// </summary>
		/// <param name="context">The invocation context.</param>
		/// <returns>The exit code.</returns>
		public Task<int> InvokeAsync(InvocationContext context) => Task.FromResult(Invoke(context));
	}
}
