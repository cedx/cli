namespace Belin.Cli;

using Belin.Cli.Setup;
using Microsoft.Extensions.Hosting;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.Diagnostics;

/// <summary>
/// Downloads and installs a runtime environment.
/// </summary>
public class SetupCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public SetupCommand(): base("setup", "Download and install a runtime environment.") {
		Add(new JdkCommand());
		Add(new NodeCommand());
		Add(new PhpCommand());
	}
}

/// <summary>
/// Provides extension methods for the <c>setup</c> command.
/// </summary>
internal static class SetupCommandExtensions {

	/// <summary>
	/// Runs the specified executable with the <c>--version</c> argument.
	/// </summary>
	/// <param name="_">The current command handler.</param>
	/// <param name="output">The path to the output directory.</param>
	/// <param name="executable">The executable path, relative to the output directory.</param>
	/// <returns>The standard output of the underlying process.</returns>
	/// <exception cref="ProcessException">An error occurred when starting the underlying process.</exception>
	public static string GetExecutableVersion(this ICommandHandler _, DirectoryInfo output, string executable) {
		var startInfo = new ProcessStartInfo(Path.Join(output.FullName, executable), "--version") {
			CreateNoWindow = true,
			RedirectStandardOutput = true
		};

		using var process = Process.Start(startInfo) ?? throw new ProcessException(startInfo.FileName);
		var standardOutput = process.StandardOutput.ReadToEnd().Trim();
		process.WaitForExit();
		if (process.ExitCode != 0) throw new ProcessException(startInfo.FileName, $"The process failed with exit code {process.ExitCode}.");
		return standardOutput;
	}

	/// <summary>
	/// Specifies the command handlers to be used by the host.
	/// </summary>
	/// <param name="builder">The host builder to configure.</param>
	/// <returns>The host builder.</returns>
	public static IHostBuilder UseSetupHandlers(this IHostBuilder builder) => builder
		.UseCommandHandler<JdkCommand, JdkCommand.CommandHandler>()
		.UseCommandHandler<NodeCommand, NodeCommand.CommandHandler>()
		.UseCommandHandler<PhpCommand, PhpCommand.CommandHandler>();
}
