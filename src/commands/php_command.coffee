import decompress from "decompress"
import {execFile} from "node:child_process"
import console from "node:console"
import {writeFile} from "node:fs/promises"
import {tmpdir} from "node:os"
import {basename, join, resolve} from "node:path"
import {env, platform} from "node:process"
import {parseArgs, promisify} from "node:util"
import semver from "semver"

# The usage information.
usage = """
Download and install the latest PHP release.

Usage:
	npx @cedx/cli php [options]

Options:
	-o, --out <directory>  The path to the output directory.
	-h, --help             Display this help.
"""

# The default installation directory.
defaultDirectory = if platform is "win32" then "C:/Program Files/PHP" else "/usr/local"

# Spawns a new process using the specified command.
run = promisify execFile

# Downloads and installs the latest PHP release.
export class PhpCommand

	# Creates a new command.
	constructor: (options = {}) ->

		# The path to the output directory.
		@output = options.output ? defaultDirectory

	# Executes this command.
	run: ->
		throw Error "This command only supports the Windows platform." unless platform is "win32"

		version = await @_fetchLatestVersion()
		path = await @_downloadArchive version
		await @_extractArchive path
		await @_registerEventLog version
		console.log (await run join(@output, "php"), ["--version"]).stdout

	# Downloads the PHP release corresponding to the specified version.
	_downloadArchive: (version) ->
		{default: pkg} = await import("../../package.json", with: type: "json")
		userAgent = "#{navigator.userAgent} | Belin.io/#{pkg.version}"

		vs = if semver.gte version, "8.4.0" then "vs17" else "vs16"
		path = join tmpdir(), "php-#{version}-nts-Win32-#{vs}-x64.zip"
		file = basename path

		console.log "Downloading file \"#{file}\"..."
		response = await fetch "https://windows.php.net/downloads/releases/#{file}", headers: {"user-agent": userAgent}
		throw Error "Unable to download the PHP archive." unless response.ok

		await writeFile path, new Uint8Array(await response.arrayBuffer())
		path

	# Extracts the archive located at the specified path.
	_extractArchive: (path) ->
		console.log "Extracting file \"#{basename path}\" into directory \"#{resolve @output}\"..."
		await @_runWebServerCommand "stop"
		await decompress path, @output
		await @_runWebServerCommand "start"

	# Fetches the latest version number of the PHP releases.
	_fetchLatestVersion: ->
		console.log "Fetching the list of PHP releases..."
		response = await fetch "https://www.php.net/releases/?json"
		throw Error "Unable to fetch the list of PHP releases." unless response.ok

		releases = await response.json()
		keys = Object.keys releases
		throw Error "Unable to fetch the list of PHP releases." unless keys.length
		releases[keys.at(-1)].version

	# Registers the PHP interpreter with the event log.
	_registerEventLog: (version) ->
		console.log "Registering the PHP interpreter with the event log..."
		key = "HKLM\\SYSTEM\\CurrentControlSet\\Services\\Eventlog\\Application\\PHP-#{version}"
		reg = join(env.windir or "C:/Windows", "System32/reg.exe")
		await run reg, ["add", key, "/v", "EventMessageFile", "/t", "REG_SZ", "/d", join(@output, "php#{version.split(".").at 0}.dll"), "/f"]
		await run reg, ["add", key, "/v", "TypesSupported", "/t", "REG_DWORD", "/d", "00000007", "/f"]

	# Starts or stops the IIS web server according to the specified command.
	_runWebServerCommand: (command) ->
		console.log "Running the \"#{command}\" IIS command..."
		await run join(env.windir or "C:/Windows", "System32/iisreset.exe"), ["/#{command}"]

# Downloads and installs the latest PHP release.
export default (args) ->
	{values} = parseArgs args: args, options:
		help: {short: "h", type: "boolean", default: off}
		out: {short: "o", type: "string", default: defaultDirectory}

	if values.help then Promise.resolve console.log usage.replaceAll "\t", "  "
	else new PhpCommand(output: values.out).run()
