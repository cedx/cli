# Provides the metadata of a database table.
export class Table

	# Creates a new table from the specified database record.
	@ofRecord: (record) -> new @
		collation: record.TABLE_COLLATION
		engine: record.ENGINE
		name: record.TABLE_NAME
		schema: record.TABLE_SCHEMA
		type: record.TABLE_TYPE
