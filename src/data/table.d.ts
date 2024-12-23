/**
 * Provides the metadata of a database table.
 */
export class Table {

	/**
	 * The name of the database table associated with this class.
	 */
	static readonly table: string;

	/**
	 * The default collation.
	 */
	collation: string;

	/**
	 * The storage engine.
	 */
	engine: TableEngine;

	/**
	 * The table name.
	 */
	name: string;

	/**
	 * The schema containing this table.
	 */
	schema: string;

	/**
	 * The table type.
	 */
	type: string;

	/**
	 * Creates a new table.
	 * @param options An object providing values to initialize this instance.
	 */
	constructor(options: TableOptions);

	/**
	 * Creates a new table from the specified database record.
	 * @param record A database record providing values to initialize the instance.
	 * @returns The newly created table.
	 */
	static ofRecord(record: TableRecord): Table;
}

/**
 * Defines the storage engine of a table.
 */
export const TableEngine: Readonly<{

	/**
	 * The table does not use any storage engine.
	 */
	none: "",

	/**
	 * The storage engine is Aria.
	 */
	aria: "Aria",

	/**
	 * The storage engine is InnoDB.
	 */
	innoDb: "InnoDB",

	/**
	 * The storage engine is MyISAM.
	 */
	myIsam: "MyISAM"
}>;

/**
 * Defines the storage engine of a table.
 */
export type TableEngine = typeof TableEngine[keyof typeof TableEngine];

/**
 * Defines the options of a {@link Table} instance.
 */
export type TableOptions = Partial<{

	/**
	 * The default collation.
	 */
	collation: string;

	/**
	 * The storage engine.
	 */
	engine: TableEngine;

	/**
	 * The table name.
	 */
	name: string;

	/**
	 * The schema containing this table.
	 */
	schema: string;

	/**
	 * The table type.
	 */
	type: string;
}>;

/**
 * Defines the data of a {@link Table} record.
 */
export type TableRecord = Partial<{

	/**
	 * The storage engine.
	 */
	ENGINE: TableEngine|null;

	/**
	 * The default collation.
	 */
	TABLE_COLLATION: string|null;

	/**
	 * The table name.
	 */
	TABLE_NAME: string;

	/**
	 * The schema containing this table.
	 */
	TABLE_SCHEMA: string;

	/**
	 * The table type.
	 */
	TABLE_TYPE: string;
}>;
