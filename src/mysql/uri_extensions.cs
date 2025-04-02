namespace Belin.Cli.MySql;

using System.Collections.Specialized;

/// <summary>
/// Provides extension methods for <see cref="Uri"/> instances.
/// </summary>
public static class UriExtensions {

	/// <summary>
	/// Parses the query information included in the specified URI into a name-value collection.
	/// </summary>
	/// <param name="uri">The URI to parse.</param>
	/// <returns>The name-value collection corresponding to the query information included in the specified URI.</returns>
	public static NameValueCollection ParseQueryString(this Uri uri) {
		var collection = new NameValueCollection();
		var query = uri.Query.StartsWith('?') ? uri.Query[1..] : uri.Query;

		if (query.Length > 0) foreach (var param in query.Split('&')) {
			var parts = param.Split('=');
			if (parts.Length > 0) collection.Add(Uri.UnescapeDataString(parts[0]), parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : null);
		}

		return collection;
	}
}
