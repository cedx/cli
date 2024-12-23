# Provides the metadata of a table column.
export class Column

	# The name of the database table associated with this class.
	@table = "COLUMNS"

	# Creates a new column.
	constructor: (options = {}) ->

		# The column name.
		@name = options.name ? ""

		# The column position.
		@position = options.position ? 0

		# The schema containing this column.
		@schema = options.schema ? ""

		# The table containing this column.
		@table = options.table ? ""

	# Creates a new column from the specified database record.
	@ofRecord: (record) -> new @
		name: record.COLUMN_NAME
		position: record.ORDINAL_POSITION
		schema: record.TABLE_SCHEMA
		table: record.TABLE_NAME
