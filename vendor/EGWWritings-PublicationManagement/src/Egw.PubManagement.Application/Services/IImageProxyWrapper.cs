using Egw.PubManagement.Application.Models.Enums;
namespace Egw.PubManagement.Application.Services;

/// <summary>
/// Image proxy wrapper
/// </summary>
public interface IImageProxyWrapper
{
    /// <summary>
    /// Generates a resized image URL
    /// </summary>
    /// <param name="source"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    Uri GetUri(Uri source, int width, int height, ResizeTypeEnum type);
}