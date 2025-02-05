# Provides the metadata of a database schema.
export class Schema

	# Creates a new schema from the specified database record.
	@ofRecord: (record) -> new @
		charset: record.DEFAULT_CHARACTER_SET_NAME
		collation: record.DEFAULT_COLLATION_NAME
		name: record.SCHEMA_NAME
