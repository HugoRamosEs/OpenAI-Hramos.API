namespace Hramos.API.Abstractions;

public interface IQdrantSnapshotHandler
{
    /// <summary>
    /// Creates a snapshot of the specified collection.
    /// </summary>
    /// <param name="collectionName">Collection name to take a snapshot of.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous snapshot creation.</returns>
    Task CreateCollectionSnapshotAsync(string collectionName, CancellationToken cancellationToken);
}
