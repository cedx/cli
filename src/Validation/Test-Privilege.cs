namespace Belin.Cli.Validation;

using System.IO;

/// <summary>
/// Checks whether the current process is privileged.
/// </summary>
[Cmdlet(VerbsDiagnostic.Test, "Privilege"), OutputType(typeof(bool))]
public class TestPrivilegeCommand: Cmdlet {

	/// <summary>
	/// The path to a directory used to verify if the process has sufficient permissions.
	/// </summary>
	[Parameter(Position = 0), ValidatePath("The specified path is invalid.")]
	public string Path { get; set; } = "";

	/// <summary>
	/// Performs execution of this command.
	/// </summary>
	protected override void ProcessRecord() {
		var isPrivileged = Environment.IsPrivilegedProcess;
		if (!string.IsNullOrWhiteSpace(Path)) {
			var homeDirectory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
			var targetDirectory = new DirectoryInfo(Path);
			if (targetDirectory.Root.Name != homeDirectory.Root.Name || targetDirectory.FullName.StartsWith(homeDirectory.FullName)) isPrivileged = true;
		}

		WriteObject(isPrivileged);
	}
}
