namespace Belin.Cli;

using Dapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

/// <summary>
/// Maps a column name to a property that may be annotated with a <see cref="ColumnAttribute"/> attribute.
/// </summary>
/// <typeparam name="T">The type of entity to which this type map applies.</typeparam>
public class ColumnAttributeTypeMap<T>(): SqlMapper.ITypeMap {

	/// <summary>
	/// The custom type map used to find the properties annotated with a <see cref="ColumnAttribute"/> attribute.
	/// </summary>
	private readonly CustomPropertyTypeMap customMapper = new(
		typeof(T),
		(type, column) => type.GetProperties().FirstOrDefault(property => property.GetCustomAttribute<ColumnAttribute>()?.Name == column)!
	);

	/// <summary>
	/// The default type map used when the custom type map was unable to find a matching property.
	/// </summary>
	private readonly DefaultTypeMap defaultMapper = new(typeof(T));

	/// <summary>
	/// Finds the best constructor.
	/// </summary>
	/// <param name="names">The column names.</param>
	/// <param name="types">The column types.</param>
	/// <returns>The matching constructor or the default one.</returns>
	public ConstructorInfo? FindConstructor(string[] names, Type[] types) {
		try { return customMapper.FindConstructor(names, types); }
		catch { return defaultMapper.FindConstructor(names, types); }
	}

	/// <summary>
	/// Finds the constructor which should always be used.
	/// </summary>
	/// <returns>The constructor which should always be used.</returns>
	public ConstructorInfo? FindExplicitConstructor() {
		var constructor = customMapper.FindExplicitConstructor();
		return constructor ?? defaultMapper.FindExplicitConstructor();
	}

	/// <summary>
	/// Gets the mapping for a constructor parameter.
	/// </summary>
	/// <param name="constructor">The constructor to resolve.</param>
	/// <param name="column">The column name.</param>
	/// <returns>The mapping implementation.</returns>
	public SqlMapper.IMemberMap? GetConstructorParameter(ConstructorInfo constructor, string column) {
		try { return customMapper.GetConstructorParameter(constructor, column); }
		catch { return defaultMapper.GetConstructorParameter(constructor, column); }
	}

	/// <summary>
	/// Gets the member mapping for a column.
	/// </summary>
	/// <param name="column">The column name.</param>
	/// <returns>The mapping implementation.</returns>
	public SqlMapper.IMemberMap? GetMember(string column) {
		try { return customMapper.GetMember(column); }
		catch { return defaultMapper.GetMember(column); }
	}
}
