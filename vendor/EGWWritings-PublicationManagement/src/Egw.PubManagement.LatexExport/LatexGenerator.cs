using Egw.PubManagement.LatexExport.Converter;
using Egw.PubManagement.LatexExport.Models;
using Egw.PubManagement.LatexExport.Nodes;

namespace Egw.PubManagement.LatexExport;

/// <summary>
/// Pdf exporter
/// </summary>
public class LatexGenerator
{
    private readonly ILatexPublicationRepository _repository;
    private readonly ILatexCoverRepository _coverRepository;
    private readonly ILatexHeadingTransformer[] _transformers;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="repository">Publication reader</param>
    /// <param name="coverRepository">Cover reader</param>
    /// <param name="transformers">Heading transformers</param>
    public LatexGenerator(ILatexPublicationRepository repository,
        ILatexCoverRepository coverRepository,
        IEnumerable<ILatexHeadingTransformer> transformers)
    {
        _repository = repository;
        _coverRepository = coverRepository;
        _transformers = transformers.ToArray();
    }

    /// <summary>
    /// Exports publication in latex format
    /// </summary>
    /// <param name="publicationId">Publication id</param>
    /// <param name="options">Conversion options</param>
    /// <param name="output">Output stream</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task ExportLatex(int publicationId, LatexPublicationOptions options,
        FileInfo output,
        CancellationToken cancellationToken)
    {
        LatexPublicationDto publication =
            await _repository.GetPublication(publicationId, cancellationToken) ??
            throw new ArgumentException("Publication not found");
        byte[]? coverData = await _coverRepository.ReadCover(publicationId, cancellationToken);
        FileInfo? cover = null;
        if (coverData is not null && coverData.Length > 0)
        {
            cover = new FileInfo(Path.ChangeExtension(output.FullName, ".jpg"));
            await using FileStream coverStream = cover.OpenWrite();
            await coverStream.WriteAsync(coverData, cancellationToken);
        }

        var builder = new LatexBuilder(publication, cover, options, _transformers);
        await using FileStream f = output.OpenWrite();
        await using var writer = new StreamWriter(f);
        await writer.WriteLineAsync("%!TEX program=lualatex");
        await foreach (ILatexNode node in builder.ConvertDocument(
                           _repository.GetParagraphs(publicationId, cancellationToken),
                           cancellationToken))
        {
            await writer.WriteLineAsync(node.ToString());
        }

        await writer.FlushAsync();
    }
}