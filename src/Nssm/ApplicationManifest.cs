namespace Belin.Cli.Nssm;

/// <summary>
/// TODO
/// </summary>
public sealed class ApplicationManifest {

	/// <summary>
	/// The application description.
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// The application environment.
	/// </summary>
	public string? Environment { get; set; }

	/// <summary>
	/// The application identifier.
	/// </summary>
	public string? Id { get; set; }

	/// <summary>
	/// The application name.
	/// </summary>
	public string? Name { get; set; }
}
