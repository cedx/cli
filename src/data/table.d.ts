/**
 * Provides the metadata of a database table.
 */
export class Table {

	/**
	 * Creates a new table from the specified database record.
	 * @param record A database record providing values to initialize the instance.
	 * @returns The newly created table.
	 */
	static ofRecord(record: TableRecord): Table;
}

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
