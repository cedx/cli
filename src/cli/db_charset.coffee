import console from "node:console"
import {parseArgs} from "node:util"
import {Schema} from "../data/schema.js"
import {Table} from "../data/table.js"
import {createConnection} from "../sql/connection.js"

# The usage information.
usage = """
Alter the character set of MariaDB tables.

Usage:
	npx @cedx/cli db-charset [options] <collation>

Arguments:
	collation            The name of the new character set.

Options:
	-d, --dsn <uri>      The connection string.
	-s, --schema <name>  The schema name.
	-t, --table <name>   The table names (requires a schema).
	-h, --help           Display this help.
"""

# Alters the character set of MariaDB tables.
export class DbCharsetCommand

	# Creates a new command.
	constructor: (options = {}) ->
		dsn = options.dsn ? "root@localhost"

		# The connection string.
		@dsn = new URL if dsn.startsWith("mariadb:") then dsn else "mariadb://#{dsn}"

		# The schema name.
		@schema = options.schema ? ""

		# The table names (requires a schema).
		@table = options.table ? []
		throw Error "The table \"#{@table[0]}\" requires that a schema be specified." if @table.length and not @schema

		# The database connection.
		@_db = null

	# Runs this command.
	run: (collation) ->
		@_db = await createConnection new URL "/information_schema", @dsn
		schemas = if @schema then [new Schema name: @schema] else await @_db.getSchemas()
		tables = await Promise.all schemas.map (schema) =>
			if @table.length then Promise.resolve @table.map (item) -> new Table name: item, schema: schema.name
			else @_db.getTables schema

		normalizedCollation = collation.toLowerCase()
		await @_db.query "SET foreign_key_checks = 0"
		await @_alterTable table, collation for table from tables.flat() when table.collation.toLowerCase() isnt normalizedCollation
		await @_db.query "SET foreign_key_checks = 1"
		await @_db.end()

	# Alters the specified database table.
	_alterTable: (table, collation) ->
		qualifiedName = table.qualifiedName (identifier) => @_db.escapeId identifier
		console.log "Processing: #{qualifiedName}"
		await @_db.query "ALTER TABLE #{qualifiedName} CONVERT TO CHARACTER SET #{collation.split("_").at 0} COLLATE #{collation}"

# Alters the character set of MariaDB tables.
export default (args) ->
	{positionals, values} = parseArgs allowPositionals: yes, args: args, options:
		dsn: {short: "d", type: "string"}
		help: {short: "h", type: "boolean", default: off}
		schema: {short: "s", type: "string", default: ""}
		table: {short: "t", type: "string", default: [], multiple: yes}

	switch
		when values.help then return Promise.resolve console.log usage.replaceAll "\t", "  "
		when not positionals.length then throw Error 'The required argument "collation" is missing.'
		when not values.dsn then throw Error 'The required option "--dsn" is missing.'

	new DbCharsetCommand(values).run positionals[0]
