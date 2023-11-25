// #define USE_DEBUG

using ChatEgw.UI.Indexer.LinkExporter;
using ChatEgw.UI.Persistence;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Egw.PubManagement.Persistence;
using Microsoft.EntityFrameworkCore;
using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;

namespace ChatEgw.UI.Indexer.ConsoleCommands;

[Command("export dataset links", Description = "Exports TSV file for python postprocessing")]
public class ExportLinkDataset : ConsoleCommandBase
{
    private readonly LinkExtractor _extractor = new();

    [CommandOption(
        "from", 'f', Description = "Database to convert from", IsRequired = true,
        EnvironmentVariable = "EGW_EXPORT_FROM"
    )]
    public required string FromDsn { get; init; }

    [CommandParameter(0, Description = "File name", IsRequired = false)]
    public FileInfo OutputFile { get; init; } = new FileInfo("references.tsv");

    protected override ValueTask Execute(IConsole console, SearchDbContext db,
        CancellationToken cancellationToken)
    {
        using var fromContext = new PublicationDbContext(
            CreateDbContext<PublicationDbContext>(
                FromDsn, o => { o.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); }
            )
        );

        using FileStream f = OutputFile.Open(FileMode.Create);
        using var writer = new StreamWriter(f);
        foreach ((OutputRow row, ParaId paraId) in FetchRows(FetchParagraphs(fromContext)))
        {
            if (row.Content.Length < 50 || row.Links.Count == 0)
            {
                continue;
            }

            writer.Write(paraId);
            writer.Write('\t');
            writer.Write(row.Content);
            foreach (Range link in row.Links)
            {
                writer.Write($"\t{link.Start.Value}-{link.End.Value}-{row.Content[link]}");
            }

            writer.WriteLine();
        }

        writer.Flush();
        f.Flush();

        return ValueTask.CompletedTask;
    }

    private IEnumerable<ParagraphDto> FetchParagraphs(PublicationDbContext db)
    {
        return db.Paragraphs
            .Where(r => r.Publication.LanguageCode == "eng")
            .Where(r => r.Publication.Type != PublicationType.ScriptureIndex &&
                        r.Publication.Type != PublicationType.TopicalIndex)
            .OrderBy(r => r.Publication.PublicationId)
            .ThenBy(r => r.Order)
            .Select(r => new ParagraphDto(r.ParaId, r.Content));
    }

    private IEnumerable<(OutputRow row, ParaId paraId)> FetchRows(IEnumerable<ParagraphDto> paragraphs)
    {
        foreach ((ParaId paraId, string content) in paragraphs)
        {
            // Console.WriteLine(paraId);
            foreach (OutputRow row in _extractor.ParseContent(content))
            {
                yield return (row, paraId);
            }
        }
    }

    private record ParagraphDto(ParaId ParaId, string Content);
}