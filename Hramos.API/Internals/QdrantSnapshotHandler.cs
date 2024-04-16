using System.Text.Json;

using Microsoft.Extensions.Logging;

using Hramos.API.Abstractions;
using Hramos.API.Options;

namespace Hramos.API.Internals;

internal sealed class QdrantSnapshotHandler : IQdrantSnapshotHandler
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILogger logger;

    public QdrantSnapshotHandler(IHttpClientFactory httpClientFactory, ILogger<IQdrantSnapshotHandler> logger)
    {
        this.httpClientFactory = httpClientFactory;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public async Task CreateCollectionSnapshotAsync(string collectionName, CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(Constants.HttpClients.Qdrant);

        using var response = await httpClient.PostAsync($@"/collections/{collectionName}/snapshots", null, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var json = (await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken)).RootElement;
            var status = json.GetProperty(@"status").GetString();

            if (@"ok".Equals(status, StringComparison.OrdinalIgnoreCase))
            {
                var result = json.GetProperty(@"result");
                var snapshotName = result.GetProperty(@"name").GetString();
                var creationTime = result.GetProperty(@"creation_time").GetString();

                logger.LogInformation($@"Successfully created snapshot for collection '{collectionName}' on '{creationTime ?? DateTime.UtcNow.ToString(@"o")}'. Snapshot name is '{snapshotName}'.");
            }
            else
            {
                logger.LogError($@"Failed to create snapshot for collection '{collectionName}'. Returned status was: {status}!");
            }
        }
        else
        {
            logger.LogError($@"Failed to create snapshot for collection '{collectionName}'. Response status was '{response.StatusCode}' and error was: {await response.Content.ReadAsStringAsync(cancellationToken)}.");
        }
    }
}
