using Amazon.S3.Model;

namespace Egw.PubManagement.Storage;

/// <summary>
/// Storage Blob
/// </summary>
internal class S3StorageBlob : IStorageBlob
{
    /// <summary>
    /// Path to file
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// File name
    /// </summary>
    public string FileName => System.IO.Path.GetFileName(Path);

    /// <summary>
    /// Last modified date
    /// </summary>
    public DateTime LastModified { get; }

    /// <summary>
    /// File size
    /// </summary>
    public long Size { get; }

    /// <summary> Create a new StorageBlob from S3Object </summary>
    /// <param name="s3Object">S3 Object</param>
    /// <param name="prefix">Storage prefix</param>
    internal S3StorageBlob(S3Object s3Object, string? prefix)
    {
        Path = s3Object.Key;
        if (!string.IsNullOrWhiteSpace(prefix) && Path.StartsWith(prefix))
        {
            Path = Path[prefix.Length..].TrimStart('/');
        }

        LastModified = s3Object.LastModified;
        Size = s3Object.Size;
    }
}