namespace Belin.Cli.CommandLine;

using System.Text;

/// <summary>
/// Converts the encoding of input files.
/// </summary>
public class IconvCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public IconvCommand(): base("iconv", "Convert the encoding of input files.") {
		var fileOrDirectoryArgument = new Argument<FileSystemInfo>(
			name: "fileOrDirectory",
			description: "The path to the file or directory to process."
		);

		var fromOption = new EncodingOption(["-f", "--from"], Encoding.UTF8.WebName, "The input encoding.");
		var toOption = new EncodingOption(["-t", "--to"], Encoding.Latin1.WebName, "The output encoding.");
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
	public async Task<int> Execute(FileSystemInfo fileOrDirectory, string from, string to, bool recursive = false) {
		if (!fileOrDirectory.Exists) {
			Console.WriteLine("Unable to locate the specified file or directory.");
			return 2;
		}

		var fromEncoding = Encoding.GetEncoding(from)!;
		var toEncoding = Encoding.GetEncoding(to)!;

		switch (fileOrDirectory) {
			case DirectoryInfo directory:
				Console.WriteLine(fileOrDirectory.GetType());
				Console.WriteLine("TODO directory");
				break;
			case FileInfo file:
				Console.WriteLine(fileOrDirectory.GetType());
				Console.WriteLine("TODO file");
				break;
			default:
				Console.WriteLine(fileOrDirectory.GetType());
				Console.WriteLine("TODO default");
				break;
		}

		return await Task.FromResult(0);
	}

	/// <summary>
	/// TODO
	/// </summary>
	/// <param name="file">The path to the file to be converted.</param>
	/// <param name="from">The input encoding.</param>
	/// <param name="to">The output encoding.</param>
	private static void ConvertEncoding(FileInfo file, Encoding from, Encoding to) {
		var bytes = File.ReadAllBytes(file.FullName);
		if (bytes.Length == 0) return;

		var isBinary = Array.IndexOf(bytes, '\0', 0, Math.Min(bytes.Length, 8_000)) > 0;
		if (!isBinary) {
			Console.WriteLine($"Converting: {file}");
			File.WriteAllBytes(file.FullName, Encoding.Convert(from, to, bytes));
		}
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
