using Path = System.IO.Path;

namespace Egw.PubManagement.Application.Services;

/// <summary>
/// Disposable folder
/// </summary>
public sealed class DisposableFolder : IDisposable
{
    /// <summary>
    /// Folder
    /// </summary>
    public DirectoryInfo Folder { get; }

    /// <summary>
    /// Creates disposable folder
    /// </summary>
    /// <param name="rootPath">Root folder path. Defaults to a temp path</param>
    public DisposableFolder(string? rootPath = null)
    {
        string folderName = Path.Combine(rootPath ?? Path.GetTempPath(), Path.GetRandomFileName());
        Folder = new DirectoryInfo(folderName);
        Folder.Create();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Folder.Delete(true);
    }
}