namespace Belin.Diagnostics;

using System.Diagnostics;

/// <summary>
/// Provides extension methods for the <see cref="Process"/> class.
/// </summary>
public static class ProcessExtensions {

	/// <summary>
	/// Starts the specified process with the `--version` argument.
	/// </summary>
	/// <param name="process">The process to start.</param>
	/// <returns>The standard process output.</returns>
	public static string GetVersion(this Process process) {
		process.StartInfo.Arguments = "--version";
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.RedirectStandardOutput = true;
		process.Start();

		var standardOutput = process.StandardOutput.ReadToEnd().Trim();
		process.WaitForExit();
		if (process.ExitCode != 0) {
			var executable = Path.GetFileName(process.StartInfo.FileName);
			throw new Exception($"The \"{executable}\" process failed with exit code {process.ExitCode}.");
		}

		return standardOutput;
	}
}
