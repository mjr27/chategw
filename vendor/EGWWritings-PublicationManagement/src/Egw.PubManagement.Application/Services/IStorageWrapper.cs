using Egw.PubManagement.Storage;

namespace Egw.PubManagement.Application.Services;

/// <summary>
/// Storage wrapper
/// </summary>
public interface IStorageWrapper
{
    /// <summary>
    /// Blob storage
    /// </summary>
    IStorage Covers { get; }

    /// <summary>
    /// Blob storage
    /// </summary>
    IStorage Exports { get; }

    /// <summary>
    /// Blob storage
    /// </summary>
    IStorage Mp3 { get; }
}