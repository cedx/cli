using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using static System.IO.File;

var release = HasArgument("r") || HasArgument("release");
var target = Argument<string>("t", null) ?? Argument("target", "default");
var version = Context.Configuration.GetValue("package_version");

Task("assets")
	.Description("Deploys the assets.")
	.DoesForEach(["Binary", "Text"], type => {
		var file = $"{type.ToLowerInvariant()}-extensions";
		var path = $"sindresorhus/{file}/refs/heads/main/{file}.json";
		DownloadFile($"https://raw.githubusercontent.com/{path}", $"res/{type}Extensions.json");
	});

Task("build")
	.Description("Builds the project.")
	.IsDependentOn("assets")
	.Does(() => DotNetBuild("Cli.slnx", new() { Configuration = release ? "Release" : "Debug" }));

Task("clean")
	.Description("Deletes all generated files.")
	.DoesForEach(["bin", "src/obj"], folder => EnsureDirectoryDoesNotExist(folder))
	.Does(() => CleanDirectory("var", fileSystemInfo => fileSystemInfo.Path.Segments[^1] != ".gitkeep"));

Task("format")
	.Description("Formats the source code.")
	.Does(() => DotNetFormat("src"));

Task("outdated")
	.Description("Checks for outdated dependencies.")
	.Does(() => StartProcess("dotnet", new ProcessSettings { Arguments = "list package --outdated" }));

Task("publish")
	.Description("Publishes the package.")
	.WithCriteria(release, @"the ""Release"" configuration must be enabled")
	.IsDependentOn("default")
	.DoesForEach(["tag", "push origin"], action => StartProcess("git", $"{action} v{version}"));

Task("setup")
	.Description("Builds the Windows installer.")
	.WithCriteria(release, @"the ""Release"" configuration must be enabled")
	.IsDependentOn("default")
	.Does(() => InnoSetup("Setup.iss"));

Task("version")
	.Description("Updates the version number in the sources.")
	.Does(() => ReplaceInFile("ReadMe.md", @"project/v\d+(\.\d+){2}", $"project/v{version}"))
	.Does(() => ReplaceInFile("Setup.iss", @"version ""\d+(\.\d+){2}""", $"version \"{version}\""))
	.Does(() => ReplaceInFile("src/Cli.csproj", @"<Version>\d+(\.\d+){2}</Version>", $"<Version>{version}</Version>"));

Task("watch")
	.Description("Watches for file changes.")
	.IsDependentOn("assets")
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
static void ReplaceInFile(FilePath file, [StringSyntax(StringSyntaxAttribute.Regex)] string pattern, string replacement, RegexOptions options = RegexOptions.None) =>
	WriteAllText(file.FullPath, Regex.Replace(ReadAllText(file.FullPath), pattern, replacement, options));
