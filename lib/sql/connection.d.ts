import {Connection} from "mariadb";
import {Column} from "../data/column.js";
import {Schema} from "../data/schema.js";
import {Table} from "../data/table.js";

/**
 * Creates a new MariaDB connection.
 * @param connectionUri The connection string of the data source.
 * @returns The newly created connection.
 */
export function createConnection(connectionUri: URL): Promise<Connection & ConnectionMixin>;

/**
 * Provides methods for inspecting the information schema.
 */
export type ConnectionMixin = {

	/**
	 * Gets the list of columns contained in the specified table.
	 * @param table The database table.
	 * @returns The columns contained in the specified table.
	 */
	getColumns(table: Table): Promise<Array<Column>>;

	/**
	 * Gets the list of schemas hosted by this database.
	 * @returns The schemas hosted by the database.
	 */
	getSchemas(): Promise<Array<Schema>>;

	/**
	 * Gets the list of tables contained in the specified schema.
	 * @param schema The database schema.
	 * @returns The tables contained in the specified schema.
	 */
	getTables(schema: Schema): Promise<Array<Table>>;
};
