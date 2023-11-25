namespace Egw.PubManagement.Storage;

/// <summary> Storage Blob </summary>
public interface IStorageBlob
{
    /// <summary> Path to file </summary>
    string Path { get; }

    /// <summary> File name </summary>
    string FileName { get; }

    /// <summary> Last modified date </summary>
    public DateTime LastModified { get; }

    /// <summary> File size </summary>
    public long Size { get; }
}