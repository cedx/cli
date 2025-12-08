namespace Belin.Cli.Compression;

using Belin.Cli.Validation;
using System.IO.Compression;
using IOPath = System.IO.Path;

/// <summary>
/// Extracts the specified ZIP archive into a given directory.
/// </summary>
[Cmdlet(VerbsData.Expand, "ZipArchive"), OutputType(typeof(void))]
public class ExpandZipArchiveCommand: Cmdlet {
	
	/// <summary>
	/// The path to the output directory.
	/// </summary>
	[Parameter(Mandatory = true, Position = 1), ValidatePath("The specified output path is invalid.")]
	public required string DestinationPath { get; set; }

	/// <summary>
	/// The path to the input ZIP archive.
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
	protected override void ProcessRecord() {
		using var zipArchive = ZipFile.OpenRead(Path);
		if (Skip == 0) zipArchive.ExtractToDirectory(DestinationPath, overwriteFiles: true);
		else foreach (var entry in zipArchive.Entries) {
			var path = string.Join('/', entry.FullName.Split('/').Skip(Skip));
			if (path.Length == 0) path = "/";

			var fullPath = IOPath.Join(DestinationPath, path);
			if (fullPath[^1] == '/') Directory.CreateDirectory(fullPath);
			else entry.ExtractToFile(fullPath, overwrite: true);
		}
	}
}
