using Belin.Cli.Commands;
using System.Reflection;

// Configure the command line.
var program = new RootCommand("Command line interface of Cédric Belin, full stack developer.") {
	new IconvCommand(),
	new JdkCommand(),
	new MySqlCommand(),
	new NodeCommand(),
	new NssmCommand(),
	new PhpCommand()
};

program.SetHandler(() => {
	var assembly = Assembly.GetExecutingAssembly();
	var product = assembly.GetCustomAttribute<AssemblyProductAttribute>();
	var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
	Console.Write($"{product!.Product} {version!.InformationalVersion}");
});

// Start the application.
return program.Invoke(args);
