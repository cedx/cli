import console from "node:console"
import {execFile} from "node:child_process"
import {readdir, stat} from "node:fs/promises"
import {extname, join, parse, resolve} from "node:path"
import {parseArgs, promisify} from "node:util"

# The usage information.
usage = """
Restore a set of MariaDB/MySQL tables.

Usage:
	npx @cedx/cli db-restore [options] <fileOrDirectory>

Arguments:
	fileOrDirectory  The path to a file or directory to process.

Options:
	-d, --dsn <uri>  The connection string.
	-h, --help       Display this help.
"""

# Spawns a new process using the specified command.
run = promisify execFile

# Restores a set of MariaDB/MySQL tables.
export class DbRestoreCommand

	# Creates a new command.
	constructor: (dsn = "root@localhost") ->

		# The connection string.
		@dsn = new URL if dsn.startsWith("mariadb:") then dsn else "mariadb://#{dsn}"

	# Executes this command.
	run: (input) ->
		unless (await stat input).isDirectory() then await @_importFile resolve input
		else
			files = await readdir input, withFileTypes: yes
			await @_importFile join file.parentPath, file.name for file from files when file.isFile() and extname(file.name).toLowerCase() is ".sql"

	# Imports a SQL dump into the database.
	_importFile: (file) ->
		entity = parse(file).name
		console.log "Importing: #{entity}"

		hosts = new Set ["::1", "127.0.0.1", "localhost"]
		await run "mariadb", [
			(if hosts.has @dsn.hostname then [] else ["--compress"])...
			"--default-character-set=#{@dsn.searchParams.get("charset") or "utf8mb4"}"
			"--execute=USE #{entity.split(".").at 0}; SOURCE #{file.replaceAll "\\", "/"};"
			"--host=#{@dsn.hostname}"
			"--password=#{decodeURIComponent @dsn.password}"
			"--port=#{@dsn.port or 3306}"
			"--user=#{decodeURIComponent @dsn.username}"
		]

# Restores a set of MariaDB/MySQL tables.
export default (args) ->
	{positionals, values} = parseArgs allowPositionals: yes, args: args, options:
		dsn: {short: "d", type: "string"}
		help: {short: "h", type: "boolean", default: off}

	switch
		when values.help then return Promise.resolve console.log usage.replaceAll "\t", "  "
		when not positionals.length then throw Error 'The required argument "fileOrDirectory" is missing.'
		when not values.dsn then throw Error 'The required option "--dsn" is missing.'

	new DbRestoreCommand(values.dsn).run positionals[0]
