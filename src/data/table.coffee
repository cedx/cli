# Provides the metadata of a database table.
export class Table

	# Creates a new table.
	constructor: (options = {}) ->

		# The default collation.
		@collation = options.collation ? ""

		# The storage engine.
		@engine = options.engine ? TableEngine.none

		# The table name.
		@name = options.name ? ""

		# The schema containing this table.
		@schema = options.schema ? ""

		# The table type.
		@type = options.type ? "BASE TABLE"

	# Creates a new table from the specified database record.
	@ofRecord: (record) -> new @
		collation: record.TABLE_COLLATION
		engine: record.ENGINE
		name: record.TABLE_NAME
		schema: record.TABLE_SCHEMA
		type: record.TABLE_TYPE
