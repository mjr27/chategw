namespace Egw.PubManagement.Services.Latex;

/// <summary>
/// PDF Export Client
/// </summary>
public class PdfExportClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Create a new PDF Export Client
    /// </summary>
    public PdfExportClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromMinutes(15);
    }

    /// <summary> Export a PDF file </summary>
    public async Task ExportPdfFile(FileInfo outputFile, FileInfo texFile, FileInfo? coverFile,
        CancellationToken cancellationToken)
    {
        var uri = new Uri("http://localhost:6080/latex");
        using var content = new MultipartFormDataContent();

        content.Add(
            await LoadContent(texFile, cancellationToken),
            "latex",
            texFile.Name
        );
        if (coverFile is not null)
        {
            content.Add(await LoadContent(coverFile, cancellationToken), "cover", coverFile.Name);
        }

        using var request = new HttpRequestMessage();
        request.Method = HttpMethod.Post;
        request.RequestUri = uri;
        request.Content = content;
        using HttpResponseMessage message = await _httpClient.SendAsync(request, cancellationToken);
        message.EnsureSuccessStatusCode();
        byte[] pdf = await message.Content.ReadAsByteArrayAsync(cancellationToken);
        await using FileStream f = outputFile.OpenWrite();
        await f.WriteAsync(pdf, cancellationToken);
        // await message.Content.CopyToAsync(f, cancellationToken);
        // await f.FlushAsync(cancellationToken);
    }

    private static async Task<ByteArrayContent> LoadContent(
        FileInfo fileInfo,
        CancellationToken cancellationToken)
    {
        byte[] data = new byte[fileInfo.Length];
        await using FileStream texF = fileInfo.OpenRead();
        _ = await texF.ReadAsync(data, cancellationToken);
        return new ByteArrayContent(data);
    }
}