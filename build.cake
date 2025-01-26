using static System.IO.File;
using System.Text.RegularExpressions;

var release = HasArgument("r") || HasArgument("release");
var target = Argument<string>("t", null) ?? Argument("target", "default");
var version = Context.Configuration.GetValue("package_version");

Task("build")
	.Description("Builds the project.")
	.Does(() => DotNetBuild("cli.csproj", new() { Configuration = release ? "Release" : "Debug" }));

Task("clean")
	.Description("Deletes all generated files.")
	.DoesForEach(["bin", "obj"], dir => EnsureDirectoryDoesNotExist(dir, new() { Recursive = true }))
	.Does(() => CleanDirectory("var", fileSystemInfo => fileSystemInfo.Path.Segments[^1] != ".gitkeep"));

Task("format")
	.Description("Formats the source code.")
	.Does(() => DotNetFormat("cli.csproj"));

Task("publish")
	.Description("Publishes the package.")
	.WithCriteria(release, @"the ""Release"" configuration must be enabled")
	.IsDependentOn("default")
	.DoesForEach(["tag", "push origin"], action => StartProcess("git", $"{action} v{version}"));

Task("version")
	.Description("Updates the version number in the sources.")
	.DoesForEach(GetFiles("*/*.csproj"), file => {
		var pattern = new Regex(@"<Version>\d+(\.\d+){2}</Version>");
		WriteAllText(file.FullPath, pattern.Replace(ReadAllText(file.FullPath), $"<Version>{version}</Version>"));
	});

Task("default")
	.Description("The default task.")
	.IsDependentOn("clean")
	.IsDependentOn("version")
	.IsDependentOn("build");

RunTarget(target);
