import {transcode} from "node:buffer"
import console from "node:console"
import {readdir, readFile, stat, writeFile} from "node:fs/promises"
import {extname, join, resolve} from "node:path"
import {parseArgs} from "node:util"
import textExtensions from "text-extensions"

# The usage information.
usage = """
Convert the encoding of input files.

Usage:
	npx @cedx/cli iconv [options] <fileOrDirectory>

Arguments:
	fileOrDirectory        The path to the file or directory to process.

Options:
	-f, --from <encoding>  The input charset. Defaults to "latin1".
	-t, --to <encoding>    The output charset. Defaults to "utf8".
	-r, --recursive        Whether to process the directory recursively.
	-h, --help             Display this help.
"""

# Converts the encoding of input files.
export class IconvCommand

	# Creates a new command.
	constructor: (options = {}) ->

		# The input charset.
		@from = options.from ? "latin1"

		# Value indicating whether to process the directory recursively.
		@recursive = options.recursive ? no

		# The output charset.
		@to = options.to ? "utf8"

		# The text file extensions.
		@_textExtensions = new Set textExtensions

	# Runs this command.
	run: (input) ->
		unless (await stat input).isDirectory() then await @_transcodeFile resolve input
		else
			files = await readdir input, recursive: @recursive, withFileTypes: yes
			await @_transcodeFile join file.parentPath, file.name for await file from files when file.isFile()

	# Converts the encoding of the specified file.
	_transcodeFile: (file) ->
		unless @_textExtensions.has extname(file)[1..].toLowerCase() then return
		console.log "Converting: #{file}"
		await writeFile file, transcode await readFile(file), @from, @to

# Converts the encoding of input files.
export default (args) ->
	{positionals, values} = parseArgs allowPositionals: yes, args: args, options:
		from: {short: "f", type: "string", default: "latin1"}
		help: {short: "h", type: "boolean", default: off}
		recursive: {short: "r", type: "boolean", default: off}
		to: {short: "t", type: "string", default: "utf8"}

	switch
		when values.help then return Promise.resolve console.log usage.replaceAll "\t", "  "
		when not positionals.length then throw Error 'The required argument "fileOrDirectory" is missing.'

	new IconvCommand(values).run positionals[0]
