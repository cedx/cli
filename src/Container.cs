namespace Belin.Cli;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// The dependency container.
/// </summary>
public sealed class Container {

	/// <summary>
	/// The singleton instance of the dependency container.
	/// </summary>
	public static IKeyedServiceProvider Instance { get; } = new Container().provider;

	/// <summary>
	/// The service provider.
	/// </summary>
	private readonly ServiceProvider provider;

	/// <summary>
	/// The service collection.
	/// </summary>
	private readonly ServiceCollection services = new();

	/// <summary>
	/// Creates a new container.
	/// </summary>
	private Container() {
		AddServices();
		provider = services.BuildServiceProvider();
	}

	/// <summary>
	/// Registers the services.
	/// </summary>
	private void AddServices() => services
		.AddTransient(_ => CreateHttpClient());

	/// <summary>
	/// Creates a new HTTP client.
	/// </summary>
	/// <returns>The newly created HTTP client.</returns>
	private HttpClient CreateHttpClient() {
		var httpClient = new HttpClient();
		var version = GetType().Assembly.GetName().Version!;
		httpClient.DefaultRequestHeaders.Add("User-Agent", $".NET/{Environment.Version.ToString(3)} | Belin.Cli/{version.ToString(3)}");
		return httpClient;
	}
}
