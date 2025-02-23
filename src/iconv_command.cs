namespace Belin.Cli;

using System.Text;
using System.Text.Json;

/// <summary>
/// Converts the encoding of input files.
/// </summary>
public class IconvCommand: Command {

	/// <summary>
	/// The list of binary file extensions.
	/// </summary>
	private readonly List<string> binaryExtensions = [];

	/// <summary>
	/// The list of text file extensions.
	/// </summary>
	private readonly List<string> textExtensions = [];

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public IconvCommand(): base("iconv", "Convert the encoding of input files.") {
		var fileOrDirectoryArgument = new Argument<FileSystemInfo>("fileOrDirectory", "The path to the file or directory to process.");
		var fromOption = new EncodingOption(["-f", "--from"], Encoding.Latin1.WebName, "The input encoding.");
		var toOption = new EncodingOption(["-t", "--to"], Encoding.UTF8.WebName, "The output encoding.");
		var recursiveOption = new Option<bool>(["-r", "--recursive"], "Whether to process the directory recursively.");

		Add(fileOrDirectoryArgument);
		Add(fromOption);
		Add(toOption);
		Add(recursiveOption);
		this.SetHandler(Execute, fileOrDirectoryArgument, fromOption, toOption, recursiveOption);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <param name="fileOrDirectory">The path to the file or directory to process.</param>
	/// <param name="from">The input encoding.</param>
	/// <param name="to">The output encoding.</param>
	/// <param name="recursive">Value indicating whether to process the directory recursively.</param>
	/// <returns>The exit code.</returns>
	public async Task<int> Execute(FileSystemInfo fileOrDirectory, string from, string to, bool recursive = false) {
		if (!fileOrDirectory.Exists) {
			Console.WriteLine("Unable to locate the specified file or directory.");
			return 2;
		}

		if (binaryExtensions.Count == 0 && textExtensions.Count == 0) {
			var resources = Path.GetFullPath(Path.Join(Path.GetDirectoryName(Environment.ProcessPath), "../res/file_extensions"));
			binaryExtensions.AddRange(JsonSerializer.Deserialize<string[]>(File.ReadAllText(Path.Join(resources, "binary.json"))) ?? []);
			textExtensions.AddRange(JsonSerializer.Deserialize<string[]>(File.ReadAllText(Path.Join(resources, "text.json"))) ?? []);
		}

		var files = fileOrDirectory switch {
			DirectoryInfo directory => directory.EnumerateFiles("*.*", SearchOption.AllDirectories),
			FileInfo file => [file],
			_ => []
		};

		var fromEncoding = Encoding.GetEncoding(from);
		var toEncoding = Encoding.GetEncoding(to);
		foreach (var file in files) ConvertFileEncoding(file, fromEncoding, toEncoding);
		return await Task.FromResult(0);
	}

	/// <summary>
	/// Converts the encoding of the specified file.
	/// </summary>
	/// <param name="file">The path to the file to be converted.</param>
	/// <param name="from">The input encoding.</param>
	/// <param name="to">The output encoding.</param>
	private void ConvertFileEncoding(FileInfo file, Encoding from, Encoding to) {
		var extension = file.Extension.ToLowerInvariant();
		var isBinary = extension.Length > 0 && binaryExtensions.Contains(extension[1..]);
		if (isBinary) return;

		var bytes = File.ReadAllBytes(file.FullName);
		var isText = extension.Length > 0 && textExtensions.Contains(extension[1..]);
		if (!isText && Array.IndexOf(bytes, '\0', 0, Math.Min(bytes.Length, 8_000)) > 0) return;

		Console.WriteLine($"Converting: {file}");
		File.WriteAllBytes(file.FullName, Encoding.Convert(from, to, bytes));
	}
}

/// <summary>
/// Provides the path to an output directory.
/// </summary>
internal class EncodingOption: Option<string> {

	/// <summary>
	/// Creates a new option.
	/// </summary>
	/// <param name="defaultValue">The default value for the option when it is not specified on the command line.</param>
	public EncodingOption(string[] aliases, string defaultValue, string description): base(aliases, () => defaultValue, description) {
		ArgumentHelpName = "encoding";
		this.FromAmong([Encoding.Latin1.WebName, Encoding.UTF8.WebName]);
	}
}
