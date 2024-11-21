import decompress from "decompress"
import {execFile, spawn} from "node:child_process"
import console from "node:console"
import {cp, open, rm, writeFile} from "node:fs/promises"
import {hostname, tmpdir} from "node:os"
import {basename, dirname, join, resolve} from "node:path"
import {argv, env, execPath, platform} from "node:process"
import {pathToFileURL} from "node:url"
import {parseArgs, promisify} from "node:util"

# The usage information.
usage = """
Download and install the latest Node.js release.

Usage:
  npx @cedx/cli node [options]

Options:
  -c, --config <file>    The path to the NSSM configuration file.
  -o, --out <directory>  The path to the output directory.
  -h, --help             Display this help.
"""

# The default installation directory.
defaultDirectory = if platform is "win32" then "C:/Program Files/Node.js" else "/usr/local"

# Spawns a new process using the specified command.
run = promisify execFile

# Downloads and installs the latest Node.js release.
export class NodeCommand

	# Creates a new command.
	constructor: (options = {}) ->

		# The URL of the NSSM configuration file.
		@config = if options.config then pathToFileURL resolve options.config else null

		# The path to the output directory.
		@output = options.output ? defaultDirectory

		# The identifiers of the applications to restart.
		@_applications = []

	# Runs this command.
	run: ->
		if resolve(@output) is dirname(execPath) then await @_detachProcess()
		else
			if @config?
				{default: config} = await import(@config.href)
				host = hostname()
				@_applications.push ...config[host] if host of config

			await @_extractArchive await @_downloadArchive await @_fetchLatestVersion()
			console.log "Node.js #{(await run join(@output, "node"), ["--version"]).stdout}"

	# Runs this command in a detached process.
	_detachProcess: ->
		console.log "This command will run in detached mode."
		output = join tmpdir(), basename(execPath)
		logFile = "#{output}.log"
		console.log "Logging to file \"#{logFile}\"..."

		await cp execPath, output
		handle = await open logFile, "w"
		spawn output, argv[1..], detached: yes, stdio: ["ignore", handle.fd, handle.fd]
			.on "close", -> handle.close()
			.unref()

	# Downloads the Node.js release corresponding to the specified version.
	_downloadArchive: (version) ->
		isWindows = platform is "win32"
		path = join tmpdir(), "node-v#{version}-#{if isWindows then "win" else platform}-x64.#{if isWindows then "zip" else "tar.gz"}"
		file = basename path

		console.log "Downloading file \"#{file}\"..."
		response = await fetch "https://nodejs.org/dist/v#{version}/#{file}"
		throw Error "Unable to download the Node.js archive." unless response.ok

		await writeFile path, new Uint8Array(await response.arrayBuffer())
		path

	# Extracts the archive located at the specified path.
	_extractArchive: (path) ->
		console.log "Extracting file \"#{basename(path)}\" into directory \"#{resolve(@output)}\"..."
		await @_stopServices()
		if path.toLowerCase().endsWith ".zip" then await decompress path, @output, strip: 1
		else await execFile "tar", ["--directory=#{@output}", "--extract", "--file=#{path}", "--strip-components=1"]
		await rm file for file from ["CHANGELOG.md", "LICENSE", "README.md"].map (file) => join @output, file unless platform is "win32"
		await @_startServices()

	# Fetches the latest version number of the Node.js releases.
	_fetchLatestVersion: ->
		console.log "Fetching the list of Node.js releases..."
		response = await fetch "https://nodejs.org/dist/index.json"
		throw Error "Unable to fetch the list of Node.js releases." unless response.ok

		releases = await response.json()
		throw Error "Unable to fetch the list of Node.js releases." unless releases.length
		releases[0].version[1..]

	# Stops all hosted NSSM services.
	_stopServices: ->
		unless @_applications.length then Promise.resolve()
		else
			console.log "Stopping the NSSM services..."
			net = join env.windir ? "C:/Windows", "System32/net.exe"
			await execFile net, ["stop", id], windowsHide: yes for id from @_applications

	# Starts all hosted NSSM services.
	_startServices: ->
		unless @_applications.length then Promise.resolve()
		else
			console.log "Starting the NSSM services..."
			net = join env.windir ? "C:/Windows", "System32/net.exe"
			await execFile net, ["start", id], windowsHide: yes for id from @_applications

# Downloads and installs the latest Node.js release.
export default (args) ->
	{values} = parseArgs args: args, options:
		config: {short: "c", type: "string", default: ""}
		help: {short: "h", type: "boolean", default: off}
		out: {short: "o", type: "string", default: defaultDirectory}

	if values.help then Promise.resolve console.log usage.replaceAll "\t", "  "
	else new NodeCommand(config: values.config, output: values.out).run()
