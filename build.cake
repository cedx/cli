using static System.IO.File;
using System.Text.RegularExpressions;

var release = HasArgument("r") || HasArgument("release");
var target = Argument<string>("t", null) ?? Argument("target", "default");
var version = Context.Configuration.GetValue("package_version");

Task("build")
	.Description("Builds the project.")
	.Does(() => DotNetBuild("cli.sln", new() { Configuration = release ? "Release" : "Debug" }));

Task("clean")
	.Description("Deletes all generated files.")
	.DoesForEach(["bin", "src/obj"], folder => EnsureDirectoryDoesNotExist(folder))
	.Does(() => CleanDirectory("var", fileSystemInfo => fileSystemInfo.Path.Segments[^1] != ".gitkeep"));

Task("format")
	.Description("Formats the source code.")
	.Does(() => DotNetFormat("cli.sln"));

Task("publish")
	.Description("Publishes the package.")
	.WithCriteria(release, @"the ""Release"" configuration must be enabled")
	.IsDependentOn("default")
	.DoesForEach(["tag", "push origin"], action => StartProcess("git", $"{action} v{version}"));

Task("version")
	.Description("Updates the version number in the sources.")
	.Does(() => ReplaceInFile("res/setup.iss", @"version ""\d+(\.\d+){2}""", $"version \"{version}\""))
	.DoesForEach(GetFiles("src/*.csproj"), file => ReplaceInFile(file, @"<Version>\d+(\.\d+){2}</Version>", $"<Version>{version}</Version>"));

Task("watch")
	.Description("Watches for file changes.")
	.Does(() => StartProcess("dotnet", new ProcessSettings { Arguments = "watch build", WorkingDirectory = "src" }));

Task("default")
	.Description("The default task.")
	.IsDependentOn("clean")
	.IsDependentOn("version")
	.IsDependentOn("build");

RunTarget(target);

/// <summary>
/// Replaces the specified pattern in a given file.
/// </summary>
/// <param name="file">The path of the file to be processed.</param>
/// <param name="pattern">The regular expression to find.</param>
/// <param name="replacement">The replacement text.</param>
/// <param name="options">The regular expression options to use.</param>
void ReplaceInFile(FilePath file, string pattern, string replacement, RegexOptions options = RegexOptions.None) {
	WriteAllText(file.FullPath, new Regex(pattern, options).Replace(ReadAllText(file.FullPath), replacement));
}
