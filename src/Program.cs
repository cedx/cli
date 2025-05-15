using Belin.Cli;
using Belin.Cli.MySql;
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
		.UseCommandHandler<BackupCommand, BackupCommand.CommandHandler>()
		.UseCommandHandler<IconvCommand, IconvCommand.CommandHandler>());

// Start the application.
if (args.Length == 0) args = ["--help"];
return await commandLineBuilder.Build().InvokeAsync(args);
