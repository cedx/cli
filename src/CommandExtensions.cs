namespace Belin.Cli;

/// <summary>
/// Provides extension methods for commands.
/// </summary>
public static class CommandExtensions {

	/// <summary>
	/// Checks whether this command should be executed in an elevated prompt.
	/// </summary>
	/// <param name="command">The current command.</param>
	/// <param name="output">The path to the output directory.</param>
	/// <returns><see langword="true"/> if this command should be executed in an elevated prompt, otherwise <see langword="false"/>.</returns>
	public static bool CheckPrivilege(this Command command, DirectoryInfo? output = null) {
		var isPrivileged = Environment.IsPrivilegedProcess;
		if (output is not null) {
			var home = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
			if (output.Root.Name != home.Root.Name || output.FullName.StartsWith(home.FullName)) isPrivileged = true;
		}

		return isPrivileged;
	}
}
