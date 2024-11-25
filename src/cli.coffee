import console from "node:console"
import {access} from "node:fs/promises"
import {join} from "node:path"
import process from "node:process"
import {parseArgs} from "node:util"

# The usage information.
usage = """
Command line interface of Cédric Belin, full stack developer.

Usage:
	npx @cedx/cli [options] <command>

Options:
	-h, --help                              Display this help.
	-v, --version                           Output the version number.

Commands:
	db-backup [options] <directory>         Backup a set of MariaDB tables.
	db-charset [options] <collation>        Alter the character set of MariaDB tables.
	db-engine [options] <engine>            Alter the storage engine of MariaDB tables.
	db-optimize [options]                   Optimize a set of MariaDB tables.
	db-restore [options] <fileOrDirectory>  Restore a set of MariaDB tables.
	iconv [options] <fileOrDirectory>       Convert the encoding of input files.
	jdk [options]                           Download and install the latest OpenJDK release.
	node [options]                          Download and install the latest Node.js release.
	nssm install|remove <directory>         Register a Node.js application as a Windows service.
	php [options]                           Download and install the latest PHP release.
"""

# Start the application.
try
	process.title = "Belin.io CLI"

	# Parse the command line arguments.
	{positionals, tokens, values} = parseArgs allowPositionals: yes, strict: no, tokens: yes, options:
		help: {short: "h", type: "boolean", default: off}
		version: {short: "v", type: "boolean", default: off}

	# Print the usage.
	if values.version
		{default: {version}} = await import("../package.json", with: type: "json")
		console.log version
		process.exit()

	if not positionals.length or (values.help and not positionals.length)
		console.log usage.replaceAll "\t", "  "
		process.exit()

	# Run the requested command.
	try
		[command] = positionals
		path = "./cli/#{command.replaceAll "-", "_"}.js"
		await access join import.meta.dirname, path
	catch
		console.error "Unknown command \"#{command}\"."
		process.exit 400

	{default: run} = await import(path)
	{index} = tokens.find ({kind}) -> kind is "positional"
	await run process.argv[index + 3..]

catch error
	console.error if error instanceof Error then error.message else error
	process.exit 500
