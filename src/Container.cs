namespace Belin.Cli;

using Belin.Cli.MySql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;

/// <summary>
/// Configures the dependency container.
/// </summary>
public static class Container {

	/// <summary>
	/// Registers the application services.
	/// </summary>
	/// <param name="_">The host builder context.</param>
	/// <param name="services">The collection of service descriptors.</param>
	public static void AddServices(HostBuilderContext _, IServiceCollection services) => services
		.AddSingleton<InformationSchema>()
		.AddScoped(_ => {
			var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(".NET", Environment.Version.ToString(3)));
			return httpClient;
		});
}
