namespace Hramos.API.Options;

/// <summary>
/// Contains all the API constants
/// </summary>
public static class Constants
{
    // App Constants...
    public const string AppTitle = @"Hramos.API";
    public const string AppVersion = @"v1";
    public const string SwaggerEndpoint = @"/swagger/v1/swagger.json";

    // Path Constants...
    public const string PluginsDirectory = @"Plugins";

    // Default Language...
    public const string DefaultLanguage = @"es-ES";

    // HTTP Client Constants...
    internal static class HttpClients
    {
        public static readonly string Qdrant = @"Qdrant";

        /// <summary>
        /// The name of the HTTP header used to pass an API key to a secure Qdrant endpoint.
        /// </summary>
        public static readonly string QdrantApiKeyHeader = @"api-key";
    }
}
