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
	var version = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
	Console.Write($"{product!.Product} {new Version(version!.Version).ToString(3)}");
});

// Start the application.
return program.Invoke(args);
