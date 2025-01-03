# Provides the metadata of a database table.
export class Table

	# The name of the database table associated with this class.
	@table = "TABLES"

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
		@type = options.type ? TableType.baseTable

	# Gets the fully qualified name.
	qualifiedName: (escape = (identifier) -> identifier) -> "#{escape @schema}.#{escape @name}"

	# Creates a new table from the specified database record.
	@ofRecord: (record) -> new @
		collation: record.TABLE_COLLATION
		engine: record.ENGINE
		name: record.TABLE_NAME
		schema: record.TABLE_SCHEMA
		type: record.TABLE_TYPE

# Defines the storage engine of a table.
export TableEngine = Object.freeze

	# The table does not use any storage engine.
	none: ""

	# The storage engine is Aria.
	aria: "Aria"

	# The storage engine is InnoDB.
	innoDb: "InnoDB"

	# The storage engine is MyISAM.
	myIsam: "MyISAM"

# Defines the type of a table.
export TableType = Object.freeze

	# A base table.
	baseTable: "BASE TABLE"

	# A view.
	view: "VIEW"
