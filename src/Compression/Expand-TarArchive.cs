namespace Belin.Cli.Compression;

using Belin.Cli.Validation;
using System.Diagnostics;
using System.Globalization;

/// <summary>
/// Extracts the specified TAR archive into a given directory.
/// </summary>
[Cmdlet(VerbsData.Expand, "TarArchive"), OutputType(typeof(void))]
public class ExpandTarArchiveCommand: Cmdlet {
	
	/// <summary>
	/// The path to the output directory.
	/// </summary>
	[Parameter(Mandatory = true, Position = 1), ValidatePath("The specified output path is invalid.")]
	public required string DestinationPath { get; set; }

	/// <summary>
	/// The path to the input TAR archive.
	/// </summary>
	[Parameter(Mandatory = true, Position = 0), ValidateFile("The specified input file does not exist.")]
	public required string Path { get; set; }

	/// <summary>
	/// The number of leading directory components to remove from file names on extraction.
	/// </summary>
	[Parameter, ValidateRange(ValidateRangeKind.NonNegative)]
	public int Skip { get; set; }

	/// <summary>
	/// Performs execution of this command.
	/// </summary>
	/// <exception cref="ApplicationFailedException">An error occurred while executing a native command.</exception>
	protected override void ProcessRecord() {
		Directory.CreateDirectory(DestinationPath);

		var arguments = new[] {
			"--directory", DestinationPath,
			"--extract",
			"--file", Path,
			"--strip-components", Skip.ToString(CultureInfo.InvariantCulture)
		};

		var startInfo = new ProcessStartInfo("tar", arguments) { CreateNoWindow = true };
		using var process = Process.Start(startInfo) ?? throw new ApplicationFailedException(startInfo.FileName);
		process.WaitForExit();
		if (process.ExitCode != 0) throw new ApplicationFailedException(startInfo.FileName);
	}
}
