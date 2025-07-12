namespace Belin.Cli;

using Belin.Cli.MySql;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

/// <summary>
/// Configures the dependency container.
/// </summary>
public static class Container {

	/// <summary>
	/// Registers the application commands.
	/// </summary>
	/// <param name="services">The collection of service descriptors.</param>
	public static void AddCommands(this IServiceCollection services) => services
		.AddTransient<RootCommand>()
		.AddTransient<IconvCommand>();

	/// <summary>
	/// Registers the application services.
	/// </summary>
	/// <param name="services">The collection of service descriptors.</param>
	public static void AddServices(this IServiceCollection services) => services
		.AddSingleton<InformationSchema>()
		.AddSingleton(_ => {
			var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(".NET", Environment.Version.ToString(3)));
			return httpClient;
		});
}
