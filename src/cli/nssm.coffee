import {which} from "@cedx/which"
import {execFile} from "node:child_process"
import console from "node:console"
import {access} from "node:fs/promises"
import {join, resolve} from "node:path"
import {env, platform} from "node:process"
import {pathToFileURL} from "node:url"
import {parseArgs, promisify} from "node:util"

# The usage information.
usage = """
Register a Node.js application as a Windows service.

Usage:
	npx @cedx/cli nssm [options] <subcommand> <directory>

Arguments:
	subcommand  The command to run.
	directory   The path to the root directory of the Node.js application.

Options:
	-h, --help  Display this help.

Subcommands:
	install     Register the Windows service.
	remove      Unregister the Windows service.
"""

# Spawns a new process using the specified command.
run = promisify execFile

# Registers a Node.js application as a Windows service.
export class NssmCommand

	# Creates a new command.
	constructor: (directory) ->

		# The path to the root directory of the Node.js application.
		@_directory = resolve directory

	# Registers the Windows service.
	installService: ->
		throw Error "This command only supports the Windows platform." unless platform is "win32"

		try {default: pkg} = await import((@_getFileUri "package.json").href, with: type: "json")
		catch then throw Error 'Unable to access the "package.json" file.'
		binaries = Object.values pkg.bin ? {}
		throw Error "Unable to determine the application entry point." unless binaries.length

		config = await @_loadConfiguration()
		[node, nssm] = await Promise.all [which("node").first(), which("nssm").first()]
		await run nssm, ["install", config.id, node, join(@_directory, binaries[0])]
		await run nssm, ["set", config.id, key, value] for [key, value] from [
			["AppDirectory", @_directory]
			["AppNoConsole", "1"]
			["AppStderr", join(@_directory, "var/stderr.log")]
			["AppStdout", join(@_directory, "var/stdout.log")]
			["Description", pkg.description ? ""]
			["DisplayName", config.name]
			["Start", "SERVICE_AUTO_START"]
		]

	# Unregisters the Windows service.
	removeService: ->
		throw Error "This command only supports the Windows platform." unless platform is "win32"

		config = await @_loadConfiguration()
		nssm = await which("nssm").first()
		await run join(env.windir ? "C:/Windows", "System32/net.exe"), ["stop", config.id]
		await run nssm, ["remove", config.id, "confirm"]

	# Returns the absolute file URI corresponding to the specified relative path.
	_getFileUri: (path) -> pathToFileURL join @_directory, path

	# Reads the configuration file of the Node.js application.
	_loadConfiguration: ->
		for folder from ["lib/server", "lib", "src/server", "src"]
			try return (await import(@_getFileUri("#{folder}/config.g.js").href)).default catch
		throw Error "Unable to find the application configuration file."

# Registers a Node.js application as a Windows service.
export default (args) ->
	{positionals, values} = parseArgs allowPositionals: yes, args: args, options:
		help: {short: "h", type: "boolean", default: off}

	switch
		when values.help then return Promise.resolve console.log usage.replaceAll "\t", "  "
		when not positionals.length then throw Error 'The required argument "subcommand" is missing.'
		when not ["install", "remove"].includes positionals[0] then throw Error "Unknown subcommand \"#{positionals[0]}\"."
		when positionals.length < 2 then throw Error 'The required argument "directory" is missing.'

	command = new NssmCommand positionals[1]
	if positionals[0] is "remove" then command.removeService() else command.installService()
