namespace Belin.Cli;

using Belin.Cli.Nssm;
using Microsoft.Extensions.Hosting;
using System.CommandLine.Hosting;

/// <summary>
/// Registers a .NET or Node.js application as a Windows service.
/// </summary>
public class NssmCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public NssmCommand(): base("nssm", "Register a .NET or Node.js application as a Windows service.") {
		Add(new InstallCommand());
		Add(new RemoveCommand());
	}
}

/// <summary>
/// Provides extension methods for the <c>nssm</c> command.
/// </summary>
internal static class NssmCommandExtensions {

	/// <summary>
	/// Specifies the command handlers to be used by the host.
	/// </summary>
	/// <param name="builder">The host builder to configure.</param>
	/// <returns>The host builder.</returns>
	public static IHostBuilder UseNssmHandlers(this IHostBuilder builder) => builder
		.UseCommandHandler<InstallCommand, InstallCommand.CommandHandler>()
		.UseCommandHandler<RemoveCommand, RemoveCommand.CommandHandler>();
}
