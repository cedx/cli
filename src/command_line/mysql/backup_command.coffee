import {Buffer} from "node:buffer"
import {execFile} from "node:child_process"
import console from "node:console"
import {mkdir, open} from "node:fs/promises"
import {EOL} from "node:os"
import {join} from "node:path"
import {parseArgs, promisify} from "node:util"
import {Schema} from "../data/schema.js"
import {Table} from "../data/table.js"
import {createConnection} from "../sql/connection.js"

# Defines the format of the output files.
export BackupFormat = Object.freeze

	# The JSON Lines format.
	jsonLines: "jsonl"

	# The SQL format.
	sqlDump: "sql"

# The usage information.
usage = """
Backup a set of MariaDB/MySQL tables.

Usage:
	npx @cedx/cli db-backup [options] <directory>

Arguments:
	directory              The path to the destination directory.

Options:
	-d, --dsn <uri>        The connection string.
	-f, --format <format>  The format of the output files. Choices: #{Object.values(BackupFormat).map(JSON.stringify).join ", "}.
	-s, --schema <name>    The schema name.
	-t, --table <name>     The table names (requires a schema).
	-h, --help             Display this help.
"""

# Spawns a new process using the specified command.
run = promisify execFile

# Backups a set of MariaDB/MySQL tables.
export class DbBackupCommand

	# Creates a new command.
	constructor: (options = {}) ->
		dsn = options.dsn ? "root@localhost"

		# The connection string.
		@dsn = new URL if dsn.startsWith("mariadb:") then dsn else "mariadb://#{dsn}"

		# The format of the output files.
		@format = options.format ? BackupFormat.sqlDump

		# The schema name.
		@schema = options.schema ? ""

		# The table names (requires a schema).
		@table = options.table ? []
		throw Error "The table \"#{@table[0]}\" requires that a schema be specified." if @table.length and not @schema

		# The database connection.
		@_db = null

	# Executes this command.
	run: (output) ->
		console.log 'Warning: the "JSON Lines" format does not export INVISIBLE columns.' if @format is BackupFormat.jsonLines
		@_db = await createConnection new URL "/information_schema", @dsn
		schemas = if @schema then [new Schema name: @schema] else await @_db.getSchemas()

		await mkdir output, recursive: yes
		exportTo = if @format is BackupFormat.jsonLines then @_exportToJsonLines.bind @ else @_exportToSqlDump.bind @
		await exportTo schema, output for schema from schemas
		await @_db.end()

	# Exports a data source to a set of JSON Lines files in the specified directory.
	_exportToJsonLines: (schema, output) ->
		entity = if @table.length is 1 then "#{schema.name}.#{@table[0]}" else schema.name
		console.log "Exporting: #{entity}"

		tables = if @table.length then @table.map (table) -> new Table name: table, schema: schema.name else await @_db.getTables schema
		for table from tables
			file = await open join(output, "#{schema.name}.#{table.name}.#{BackupFormat.jsonLines}"), "w"
			stream = @_db.queryStream "SELECT * FROM #{@_db.escapeId schema.name}.#{@_db.escapeId table.name}"
			await file.write JSON.stringify(row, @_replacer) + EOL for await row from stream
			await file.close()

	# Exports a data source to a SQL dump in the specified directory.
	_exportToSqlDump: (schema, output) ->
		entity = if @table.length is 1 then "#{schema.name}.#{@table[0]}" else schema.name
		console.log "Exporting: #{entity}"

		hosts = new Set ["::1", "127.0.0.1", "localhost"]
		await run "mariadb-dump", [
			(if hosts.has @dsn.hostname then [] else ["--compress"])...
			"--default-character-set=#{@dsn.searchParams.get("charset") or "utf8mb4"}"
			"--host=#{@dsn.hostname}"
			"--password=#{decodeURIComponent @dsn.password}"
			"--port=#{@dsn.port or 3306}"
			"--result-file=#{join output, "#{entity}.#{BackupFormat.sqlDump}"}"
			"--user=#{decodeURIComponent @dsn.username}"
			schema.name
			@table...
		]

	# Optionally serializes binary data into data URLs.
	_replacer: (key, value) ->
		isBuffer = (val) -> typeof val is "object" and val and val.type is "Buffer"
		switch
			when typeof value is "bigint" then Number value
			when isBuffer value then "data:application/octet-stream;base64,#{Buffer.from(value.data).toString "base64"}"
			else value

# Backups a set of MariaDB/MySQL tables.
export default (args) ->
	{positionals, values} = parseArgs allowPositionals: yes, args: args, options:
		dsn: {short: "d", type: "string"}
		format: {short: "f", type: "string", default: BackupFormat.sqlDump}
		help: {short: "h", type: "boolean", default: off}
		schema: {short: "s", type: "string", default: ""}
		table: {short: "t", type: "string", default: [], multiple: yes}

	switch
		when values.help then return Promise.resolve console.log usage.replaceAll "\t", "  "
		when not positionals.length then throw Error 'The required argument "directory" is missing.'
		when not values.dsn then throw Error 'The required option "--dsn" is missing.'

	new DbBackupCommand(values).run positionals[0]
