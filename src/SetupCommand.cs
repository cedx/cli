namespace Belin.Cli;

using Belin.Cli.Setup;
using System.Diagnostics;
using System.Net.Http.Headers;

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

	/// <summary>
	/// Runs the specified executable with the <c>--version</c> argument.
	/// </summary>
	/// <param name="output">The path to the output directory.</param>
	/// <param name="executable">The executable path, relative to the output directory.</param>
	/// <returns>The standard output of the underlying process.</returns>
	/// <exception cref="ProcessException">An error occurred when starting the underlying process.</exception>
	internal static string GetExecutableVersion(DirectoryInfo output, string executable) {
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
