using Belin.Cli;
using MySql = Belin.Cli.MySql;
using Nssm = Belin.Cli.Nssm;
using Setup = Belin.Cli.Setup;
using Microsoft.Extensions.Hosting;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;

// Configure the command line.
var rootCommand = new RootCommand("Command line interface of Cédric Belin, full stack developer.") {
	new IconvCommand(),
	new MySqlCommand(),
	new NssmCommand(),
	new SetupCommand()
};

var commandLineBuilder = new CommandLineBuilder(rootCommand)
	.UseDefaults()
	.UseHost(_ => Host.CreateDefaultBuilder(args).UseContentRoot(AppContext.BaseDirectory), builder => builder
		.ConfigureServices(Container.AddServices)
		.UseCommandHandler<IconvCommand, IconvCommand.CommandHandler>()
		.UseCommandHandler<MySql.BackupCommand, MySql.BackupCommand.CommandHandler>()
		.UseCommandHandler<MySql.CharsetCommand, MySql.CharsetCommand.CommandHandler>()
		.UseCommandHandler<MySql.EngineCommand, MySql.EngineCommand.CommandHandler>()
		.UseCommandHandler<MySql.OptimizeCommand, MySql.OptimizeCommand.CommandHandler>()
		.UseCommandHandler<MySql.RestoreCommand, MySql.RestoreCommand.CommandHandler>()
		.UseCommandHandler<Nssm.InstallCommand, Nssm.InstallCommand.CommandHandler>()
		.UseCommandHandler<Nssm.RemoveCommand, Nssm.RemoveCommand.CommandHandler>()
		.UseCommandHandler<Setup.JdkCommand, Setup.JdkCommand.CommandHandler>()
		.UseCommandHandler<Setup.NodeCommand, Setup.NodeCommand.CommandHandler>()
		.UseCommandHandler<Setup.PhpCommand, Setup.PhpCommand.CommandHandler>());

// Start the application.
if (args.Length == 0) args = ["--help"];
return await commandLineBuilder.Build().InvokeAsync(args);
