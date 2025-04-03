namespace Belin.Cli.Setup;

using System.IO.Compression;

/// <summary>
/// Provides extension methods for <see cref="FileInfo"/> instances.
/// </summary>
public static class FileInfoExtensions {

	/// <summary>
	/// Extracts the specified ZIP file into a given directory.
	/// </summary>
	/// <param name="input">The path to the input ZIP file.</param>
	/// <param name="output">The path to the output directory.</param>
	/// <param name="strip">The number of leading directory components to remove from file names on extraction.</param>
	public static void ExtractTo(this FileInfo input, DirectoryInfo output, int strip = 0) {
		Console.WriteLine($"Extracting file \"{input.Name}\" into directory \"{output.FullName}\"...");

		using var zipArchive = ZipFile.OpenRead(input.FullName);
		if (strip == 0) zipArchive.ExtractToDirectory(output.FullName, overwriteFiles: true);
		else foreach (var entry in zipArchive.Entries) {
			var path = string.Join('/', entry.FullName.Split('/').Skip(strip));
			if (path.Length == 0) path = "/";

			var fullPath = Path.Join(output.FullName, path);
			if (fullPath[^1] == '/') Directory.CreateDirectory(fullPath);
			else entry.ExtractToFile(fullPath, overwrite: true);
		}
	}
}
