namespace Belin.Cli;

using Belin.Cli.Setup;
using System.Diagnostics;

/// <summary>
/// Downloads and installs a runtime environment.
/// </summary>
public class SetupCommand: Command {

	/// <summary>
	/// Creates a new <c>setup</c> command.
	/// </summary>
	/// <param name="jdk">The <c>jdk</c> subcommand.</param>
	/// <param name="node">The <c>node</c> subcommand.</param>
	/// <param name="php">The <c>php</c> subcommand.</param>
	public SetupCommand(JdkCommand jdk, NodeCommand node, PhpCommand php): base("setup", "Download and install a runtime environment.") {
		Subcommands.Add(jdk);
		Subcommands.Add(node);
		Subcommands.Add(php);
	}
}

/// <summary>
/// Provides extension methods for the <c>setup</c> subcommands.
/// </summary>
internal static class SetupCommandExtensions {

	/// <summary>
	/// Runs the specified executable with the <c>--version</c> argument.
	/// </summary>
	/// <param name="command">The current command.</param>
	/// <param name="output">The path to the output directory.</param>
	/// <param name="executable">The executable path, relative to the output directory.</param>
	/// <returns>The standard output of the underlying process.</returns>
	/// <exception cref="ProcessException">An error occurred when starting the underlying process.</exception>
	public static string GetExecutableVersion(this Command command, DirectoryInfo output, string executable) {
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
}
