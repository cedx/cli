namespace Belin.Cli.Nssm;

using System.Diagnostics;
using System.ServiceProcess;

/// <summary>
/// Unregisters the Windows service.
/// </summary>
public class RemoveCommand: Command {

	/// <summary>
	/// The path to the root directory of the .NET or Node.js application.
	/// </summary>
	private readonly Argument<DirectoryInfo> directoryArgument = new Argument<DirectoryInfo>("directory") {
		DefaultValueFactory = _ => new DirectoryInfo(Environment.CurrentDirectory),
		Description = "The path to the root directory of the .NET or Node.js application."
	}.AcceptExistingOnly();

	/// <summary>
	/// Creates a new <c>remove</c> command.
	/// </summary>
	public RemoveCommand(): base("remove", "Unregister the Windows service.") {
		Arguments.Add(directoryArgument);
		SetAction(InvokeAsync);
	}

	/// <summary>
	/// Invokes this command.
	/// </summary>
	/// <param name="parseResult">The results of parsing the command line input.</param>
	/// <returns>The exit code.</returns>
	public int Invoke(ParseResult parseResult) {
		if (!this.CheckPrivilege()) {
			Console.Error.WriteLine("You must run this command in an elevated prompt.");
			return 1;
		}

		var application = WebApplication.ReadFromDirectory(parseResult.GetRequiredValue(directoryArgument).FullName);
		if (application is null) {
			Console.Error.WriteLine("Unable to locate the application configuration file.");
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
			Console.Error.WriteLine("{0}", e.Message);
			return 3;
		}
	}

	/// <summary>
	/// Invokes this command.
	/// </summary>
	/// <param name="parseResult">The results of parsing the command line input.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The exit code.</returns>
	public Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken) => Task.FromResult(Invoke(parseResult));
}
