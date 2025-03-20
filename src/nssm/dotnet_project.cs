namespace Belin.Cli.Nssm;

using System.ComponentModel;
using System.Xml.Serialization;

/// <summary>
/// Represents the contents of a .NET project file.
/// </summary>
[DesignerCategory("code")]
[Serializable]
[XmlRoot(Namespace = "", IsNullable = false)]
[XmlType(AnonymousType = true)]
public partial class DotNetProject {

	// TODO [XmlElement("PropertyGroup")]


	/// <summary>
	///
	/// </summary>
	[XmlElement]
	public PropertyGroup[] PropertyGroup { get; set; } = [];
}

/// <remarks/>
[DesignerCategory("code")]
[Serializable]
[XmlType(AnonymousType = true)]
public partial class PropertyGroup {

	/// <summary>
	/// The project description.
	/// </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	///
	/// </summary>
	public string Product { get; set; } = string.Empty;

	/// <summary>
	///
	/// </summary>
	public string OutDir { get; set; } = string.Empty;
}
