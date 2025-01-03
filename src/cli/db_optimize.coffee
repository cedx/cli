import console from "node:console"
import {parseArgs} from "node:util"
import {Schema} from "../data/schema.js"
import {Table} from "../data/table.js"
import {createConnection} from "../sql/connection.js"

# The usage information.
usage = """
Optimize a set of MariaDB tables.

Usage:
	npx @cedx/cli db-optimize [options]

Options:
	-d, --dsn <uri>      The connection string.
	-s, --schema <name>  The schema name.
	-t, --table <name>   The table names (requires a schema).
	-h, --help           Display this help.
"""

# Optimizes a set of MariaDB tables.
export class DbOptimizeCommand

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
	run: ->
		@_db = await createConnection new URL "/information_schema", @dsn
		schemas = if @schema then [new Schema name: @schema] else await @_db.getSchemas()
		tables = await Promise.all schemas.map (schema) =>
			if @table.length then Promise.resolve @table.map (item) -> new Table name: item, schema: schema.name
			else @_db.getTables schema

		await @_optimizeTable table for table from tables.flat()
		await @_db.end()

	# Optimizes the specified database table.
	_optimizeTable: (table) ->
		qualifiedName = table.qualifiedName (identifier) => @_db.escapeId identifier
		console.log "Optimizing: #{qualifiedName}"
		await @_db.query "OPTIMIZE TABLE #{qualifiedName}"

# Optimizes a set of MariaDB tables.
export default (args) ->
	{values} = parseArgs args: args, options:
		dsn: {short: "d", type: "string"}
		help: {short: "h", type: "boolean", default: off}
		schema: {short: "s", type: "string", default: ""}
		table: {short: "t", type: "string", default: [], multiple: yes}

	switch
		when values.help then return Promise.resolve console.log usage.replaceAll "\t", "  "
		when not values.dsn then throw Error 'The required option "--dsn" is missing.'

	new DbOptimizeCommand(values).run()
