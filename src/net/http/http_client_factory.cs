namespace Belin.Net.Http;

using System.Reflection;

/// <summary>
/// Creates HTTP client instances.
/// </summary>
public static class HttpClientFactory {

	/// <summary>
	/// Creates a new HTTP client.
	/// </summary>
	/// <returns>The newly created HTTP client.</returns>
	public static HttpClient CreateClient() {
		var fileVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>();
		var userAgent = $".NET/{Environment.Version} | Belin.io/{new Version(fileVersion!.Version).ToString(3)}";
		var httpClient = new HttpClient();
		httpClient.DefaultRequestHeaders.Add("user-agent", userAgent);
		return httpClient;
	}
}
