import mariadb from "mariadb"
import {Column} from "../data/column.js"
import {Schema} from "../data/schema.js"
import {Table, TableType} from "../data/table.js"

# Creates a new MariaDB connection.
export createConnection = (dsn) ->
	dsn.searchParams.set "connectionLimit", "1" unless dsn.searchParams.has "connectionLimit"
	pool = mariadb.createPool dsn.href
	Promise.resolve Object.assign pool,
		getColumns: getColumns.bind pool
		getSchemas: getSchemas.bind pool
		getTables: getTables.bind pool

# Gets the list of columns contained in the specified table.
getColumns = (table) ->
	query = "
		SELECT *
		FROM information_schema.COLUMNS
		WHERE TABLE_SCHEMA = ? AND TABLE_NAME = ?
		ORDER BY ORDINAL_POSITION"

	recordset = await @query query, [table.schema, table.name]
	recordset.map (record) -> Column.ofRecord record

# Gets the list of schemas hosted by a database.
getSchemas = ->
	query = "
		SELECT *
		FROM information_schema.SCHEMATA
		WHERE SCHEMA_NAME NOT IN ('information_schema', 'mysql', 'performance_schema', 'sys')
		ORDER BY SCHEMA_NAME"

	recordset = await @query query
	recordset.map (record) -> Schema.ofRecord record

# Gets the list of tables contained in the specified schema.
getTables = (schema) ->
	query = "
		SELECT *
		FROM information_schema.TABLES
		WHERE TABLE_SCHEMA = ? AND TABLE_TYPE = ?
		ORDER BY TABLE_NAME"

	recordset = await @query query, [schema.name, TableType.baseTable]
	recordset.map (record) -> Table.ofRecord record
