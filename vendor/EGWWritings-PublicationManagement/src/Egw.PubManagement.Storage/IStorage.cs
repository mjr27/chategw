namespace Egw.PubManagement.Storage;

/// <summary>
/// File storage abstraction
/// </summary>
public interface IStorage
{
    /// <summary>
    /// Reads file from storage
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Stream?> Read(string path, CancellationToken cancellationToken);

    /// <summary>
    /// Writes file to storage
    /// </summary>
    /// <param name="path"></param>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Write(string path, Stream stream, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes file from storage
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Delete(string path, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if file exists in storage
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> Exists(string path, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if file exists in storage
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<IStorageBlob> ListObjects(string path, CancellationToken cancellationToken);

    /// <summary>
    /// Gets URI for file in storage
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    Uri GetUri(string path);
}