/**
 * Provides the metadata of a database schema.
 */
export class Schema {

	/**
	 * Creates a new schema from the specified database record.
	 * @param record A database record providing values to initialize the instance.
	 * @returns The newly created schema.
	 */
	static ofRecord(record: SchemaRecord): Schema;
}

/**
 * Defines the data of a {@link Schema} record.
 */
export type SchemaRecord = Partial<{

	/**
	 * The default character set.
	 */
	DEFAULT_CHARACTER_SET_NAME: string;

	/**
	 * The default collation.
	 */
	DEFAULT_COLLATION_NAME: string;

	/**
	 * The schema name.
	 */
	SCHEMA_NAME: string;
}>;
