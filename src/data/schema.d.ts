/**
 * Provides the metadata of a database schema.
 */
export class Schema {

	/**
	 * The name of the database table associated with this class.
	 */
	static readonly table: string;

	/**
	 * The default character set.
	 */
	charset: string;

	/**
	 * The default collation.
	 */
	collation: string;

	/**
	 * The schema name.
	 */
	name: string;

	/**
	 * Creates a new schema.
	 * @param options An object providing values to initialize this instance.
	 */
	constructor(options?: SchemaOptions);

	/**
	 * Creates a new schema from the specified database record.
	 * @param record A database record providing values to initialize the instance.
	 * @returns The newly created schema.
	 */
	static ofRecord(record: SchemaRecord): Schema;
}

/**
 * Defines the options of a {@link Schema} instance.
 */
export type SchemaOptions = Partial<{

	/**
	 * The default character set.
	 */
	charset: string;

	/**
	 * The default collation.
	 */
	collation: string;

	/**
	 * The schema name.
	 */
	name: string;
}>;

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
