using Egw.PubManagement.EpubExport.Extensions;
using Egw.PubManagement.Persistence.Entities;

namespace Egw.PubManagement.EpubExport.Creators;

/// <summary>
/// StructureCreator creates the basic structure of an epub file.
/// </summary>
public class StructureCreator
{
    private readonly string _baseDirectory;

    /// <summary> Default constructor. </summary>
    public StructureCreator(string baseDirectory)
    {
        _baseDirectory = baseDirectory;
    }

    /// <summary>
    /// Creates file structure for epub file.
    /// </summary>
    /// <param name="publication"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task Create(Publication publication, CancellationToken ct)
    {
        await CreateStructure(publication, ct);

        await File.WriteAllTextAsync(
            Path.Combine(_baseDirectory, "EPUB", "css", "bookstyles.css"),
            await GetType().Assembly.ReadTextResource("bookstyles.css", ct),
            ct
        );
    }

    private async Task CreateStructure(Publication publication, CancellationToken ct)
    {
        if (!Directory.Exists(Path.Combine(_baseDirectory, publication.PublicationId.ToString())))
        {
            Directory.CreateDirectory(Path.Combine(_baseDirectory, publication.PublicationId.ToString()));
        }

        Directory.CreateDirectory(Path.Combine(_baseDirectory, "META-INF"));
        Directory.CreateDirectory(Path.Combine(_baseDirectory, "EPUB"));
        Directory.CreateDirectory(Path.Combine(_baseDirectory, "EPUB", "css"));
        Directory.CreateDirectory(Path.Combine(_baseDirectory, "EPUB", "img"));
        Directory.CreateDirectory(Path.Combine(_baseDirectory, "EPUB", "xhtml"));
        await File.WriteAllTextAsync(Path.Combine(_baseDirectory, "EPUB", "toc.ncx"), "", ct);
        await File.WriteAllTextAsync(Path.Combine(_baseDirectory, "mimetype"), "application/epub+zip", ct);
        await File.WriteAllTextAsync(Path.Combine(_baseDirectory, "META-INF", "container.xml"),
            """<?xml version="1.0" encoding="UTF-8"?><container xmlns="urn:oasis:names:tc:opendocument:xmlns:container" version="1.0"><rootfiles><rootfile full-path="EPUB/package.opf" media-type="application/oebps-package+xml"/></rootfiles></container>""",
            ct);
    }
}