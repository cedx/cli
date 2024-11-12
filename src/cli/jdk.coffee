import decompress from "decompress"
import {execFile} from "node:child_process"
import console from "node:console"
import {writeFile} from "node:fs/promises"
import {tmpdir} from "node:os"
import {basename, join, resolve} from "node:path"
import {platform} from "node:process"
import {parseArgs, promisify} from "node:util"

# The usage information.
usage = """
Download and install the latest OpenJDK release.

Usage:
	npx @cedx/cli jdk [options]

Options:
	-j, --java <version>   The major version of the Java development kit.
	-o, --out <directory>  The path to the output directory.
	-h, --help             Display this help.
"""

# The default installation directory.
defaultDirectory = if platform is "win32" then "C:/Program Files/OpenJDK" else "/opt/openjdk"

# Spawns a new process using the specified command.
run = promisify execFile

# Downloads and installs the latest OpenJDK release.
export class JdkCommand

	# Creates a new command.
	constructor: (options = {}) ->

		# The major version of the Java development kit.
		@java = options.java ? 21

		# The path to the output directory.
		@output = options.output ? defaultDirectory

	# Runs this command.
	run: ->
		path = await @_downloadArchive()
		await @_extractArchive path
		console.log (await run "java", ["--version"]).stdout

	# Downloads the OpenJDK release corresponding to the requested Java version.
	_downloadArchive: ->
		os = switch platform
			when "darwin" then "macOS"
			when "win32" then "windows"
			else "linux"

		path = join tmpdir(), "microsoft-jdk-#{@java}-#{os}-x64.#{if platform is "win32" then "zip" else "tar.gz"}"
		file = basename path

		console.log "Downloading file \"#{file}\"..."
		response = await fetch "https://aka.ms/download-jdk/#{file}"
		throw Error "Unable to download the OpenJDK archive." unless response.ok

		await writeFile path, new Uint8Array(await response.arrayBuffer())
		path

	# Extracts the archive located at the specified path.
	_extractArchive: (path) ->
		console.log "Extracting file \"#{basename path}\" into directory \"#{resolve @output}\"..."
		if path.toLowerCase().endsWith ".zip" then decompress path, @output, strip: 1
		else execFile "tar", ["--directory=#{@output}", "--extract", "--file=#{path}", "--strip-components=1"]

# Downloads and installs the latest OpenJDK release.
export default (args) ->
	{values} = parseArgs args: args, options:
		help: {short: "h", type: "boolean", default: off}
		java: {short: "j", type: "string", default: "21"}
		out: {short: "o", type: "string", default: defaultDirectory}

	if values.help then Promise.resolve console.log usage.replaceAll "\t", "  "
	else new JdkCommand(java: Number(values.java), output: values.out).run()
