namespace Belin.Cli;

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
	public static void AddCommands(this IServiceCollection services) {
		var commands =
			from type in typeof(Program).Assembly.DefinedTypes
			where !type.IsAbstract && type.IsSubclassOf(typeof(Command))
			select type.AsType();

		foreach (var command in commands) services.AddTransient(command);
	}

	/// <summary>
	/// Registers the application services.
	/// </summary>
	/// <param name="services">The collection of service descriptors.</param>
	public static void AddServices(this IServiceCollection services) => services
		.AddSingleton<MySql.InformationSchema>()
		.AddTransient(_ => {
			var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(".NET", Environment.Version.ToString(3)));
			return httpClient;
		});
}
