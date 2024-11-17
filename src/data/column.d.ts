/**
 * Provides the metadata of a table column.
 */
export class Column {

	/**
	 * The column name.
	 */
	name: string;

	/**
	 * The column position.
	 */
	position: number;

	/**
	 * The schema containing this column.
	 */
	schema: string;

	/**
	 * The table containing this column.
	 */
	table: string;

	/**
	 * Creates a new column.
	 * @param options An object providing values to initialize this instance.
	 */
	constructor(options?: ColumnOptions);

	/**
	 * Creates a new column from the specified database record.
	 * @param record A database record providing values to initialize the instance.
	 * @returns The newly created column.
	 */
	static ofRecord(record: ColumnRecord): Column;
}

/**
 * Defines the options of a {@link Column} instance.
 */
export type ColumnOptions = Partial<{

	/**
	 * The column name.
	 */
	name: string;

	/**
	 * The column position.
	 */
	position: number;

	/**
	 * The schema containing this column.
	 */
	schema: string;

	/**
	 * The table containing this column.
	 */
	table: string;
}>;

/**
 * Defines the data of a {@link Column} record.
 */
export type ColumnRecord = Partial<{

	/**
	 * The column name.
	 */
	COLUMN_NAME: string;

	/**
	 * The column position.
	 */
	ORDINAL_POSITION: number;

	/**
	 * The schema containing this column.
	 */
	TABLE_SCHEMA: string;

	/**
	 * The table containing this column.
	 */
	TABLE_NAME: string;
}>;
