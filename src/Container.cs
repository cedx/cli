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
	public static void AddCommands(this IServiceCollection services) => services
		.AddTransient<IconvCommand>()
		.AddTransient<MySqlCommand>()
		.AddTransient<NssmCommand>()
		.AddTransient<RootCommand>()
		.AddTransient<SetupCommand>()
		.AddTransient<MySql.CharsetCommand>()
		.AddTransient<MySql.EngineCommand>()
		.AddTransient<MySql.OptimizeCommand>()
		.AddTransient<Nssm.InstallCommand>()
		.AddTransient<Nssm.RemoveCommand>()
		.AddTransient<Setup.JdkCommand>()
		.AddTransient<Setup.NodeCommand>()
		.AddTransient<Setup.PhpCommand>();

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
