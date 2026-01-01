namespace Belin.Cli.Nssm;

using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// Represents the contents of a C# project file.
/// </summary>
[XmlRoot("Project")]
public class CSharpProject {

	/// <summary>
	/// The property groups.
	/// </summary>
	[XmlElement("PropertyGroup")]
	public CSharpPropertyGroup[] PropertyGroups { get; set; } = [];
}

/// <summary>
/// Represents a group of project properties.
/// </summary>
/// <param name="AssemblyName">The assembly name.</param>
/// <param name="OutDir">The relative path of the project assemblies.</param>
/// <param name="Product">The product name.</param>
/// <param name="Description">The project description.</param>
public record CSharpPropertyGroup(
	string AssemblyName = "",
	string OutDir = "",
	string Product = "",
	string Description = ""
);
