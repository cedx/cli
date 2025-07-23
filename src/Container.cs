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
		.AddMySqlCommands()
		.AddNssmCommands()
		.AddSetupCommands()
		.AddTransient<IconvCommand>()
		.AddTransient<RootCommand>();

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

	/// <summary>
	/// Registers the MySQL commands.
	/// </summary>
	/// <param name="services">The collection of service descriptors.</param>
	/// <returns>A reference to the specified service collection.</returns>
	private static IServiceCollection AddMySqlCommands(this IServiceCollection services) => services
		.AddTransient<MySqlCommand>()
		.AddTransient<MySql.BackupCommand>()
		.AddTransient<MySql.CharsetCommand>()
		.AddTransient<MySql.EngineCommand>()
		.AddTransient<MySql.OptimizeCommand>()
		.AddTransient<MySql.RestoreCommand>();

	/// <summary>
	/// Registers the NSSM commands.
	/// </summary>
	/// <param name="services">The collection of service descriptors.</param>
	/// <returns>A reference to the specified service collection.</returns>
	private static IServiceCollection AddNssmCommands(this IServiceCollection services) => services
		.AddTransient<NssmCommand>()
		.AddTransient<Nssm.InstallCommand>()
		.AddTransient<Nssm.RemoveCommand>();

	/// <summary>
	/// Registers the setup commands.
	/// </summary>
	/// <param name="services">The collection of service descriptors.</param>
	/// <returns>A reference to the specified service collection.</returns>
	private static IServiceCollection AddSetupCommands(this IServiceCollection services) => services
		.AddTransient<SetupCommand>()
		.AddTransient<Setup.JdkCommand>()
		.AddTransient<Setup.NodeCommand>()
		.AddTransient<Setup.PhpCommand>();
}
