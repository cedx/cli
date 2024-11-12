import mariadb from "mariadb"
import {Column} from "../data/column.js"
import {Schema} from "../data/schema.js"
import {Table} from "../data/table.js"

# Creates a new MariaDB connection.
export createConnection = (connectionUri) ->
	connection = await mariadb.createConnection connectionUri.href
	Object.assign connection,
		getColumns: getColumns.bind connection
		getSchemas: getSchemas.bind connection
		getTables: getTables.bind connection

# Gets the list of columns contained in the specified table.
getColumns = (table) ->
	query = "
		SELECT * FROM information_schema.COLUMNS
		WHERE TABLE_SCHEMA = ? AND TABLE_NAME = ?
		ORDER BY ORDINAL_POSITION"

	rows = await @query query, [table.schema, table.name]
	rows.map (row) -> Column.ofRecord row

# Gets the list of schemas hosted by a database.
getSchemas = ->
	query = "
		SELECT * FROM information_schema.SCHEMATA
		WHERE SCHEMA_NAME NOT IN ('information_schema', 'mysql', 'performance_schema', 'sys')
		ORDER BY SCHEMA_NAME"

	rows = await @query query
	rows.map (row) -> Schema.ofRecord row

# Gets the list of tables contained in the specified schema.
getTables = (schema) ->
	query = "
		SELECT * FROM information_schema.TABLES
		WHERE TABLE_SCHEMA = ? AND TABLE_TYPE = 'BASE TABLE'
		ORDER BY TABLE_NAME"

	rows = await @query query, schema.name
	rows.map (row) -> Table.ofRecord row
