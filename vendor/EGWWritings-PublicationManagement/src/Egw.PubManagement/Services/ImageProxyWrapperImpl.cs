using Egw.PubManagement.Application.Models.Enums;
using Egw.PubManagement.Application.Services;

using ImgProxy;
namespace Egw.PubManagement.Services;

/// <inheritdoc />
public class ImageProxyWrapperImpl : IImageProxyWrapper
{
    private readonly string _key;
    private readonly string _salt;
    private readonly string _endpoint;

    /// <summary>
    /// Default constructor
    /// </summary>
    public ImageProxyWrapperImpl(Uri endpoint)
    {
        var builder = new UriBuilder(endpoint);
        _key = builder.UserName;
        _salt = builder.Password;
        builder.Password = "";
        builder.UserName = "";
        _endpoint = builder.Uri.AbsoluteUri;
    }

    /// <summary>
    /// Default constructor
    /// </summary>
    public ImageProxyWrapperImpl(string endpoint) : this(new Uri(endpoint, UriKind.Absolute))
    {
    }

    /// <inheritdoc />
    public Uri GetUri(Uri source, int width, int height, ResizeTypeEnum type)
    {
        ImgProxyBuilder builder = ImgProxyBuilder.New
            .WithEndpoint(_endpoint)
            .WithCredentials(_key, _salt)
            .WithFormat(Formats.JPG)
            .WithResize(type switch
            {
                ResizeTypeEnum.Fill => ResizingTypes.Fill,
                ResizeTypeEnum.Fit => ResizingTypes.Fit,
                _ => ResizingTypes.Auto,
            }, width, height, false);
        return new Uri(builder.Build(source.ToString()));
    }
}