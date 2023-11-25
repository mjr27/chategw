using ChatEgw.UI.Persistence;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace ChatEgw.UI.Indexer.ConsoleCommands;

[Command("export tsv", Description = "Exports TSV file for python postprocessing")]
public class ExportTsvConsoleCommand : ConsoleCommandBase
{
    [CommandParameter(0, Description = "File name", IsRequired = false)]
    public FileInfo OutputFile { get; init; } = new FileInfo("paragraphs.tsv");

    protected override ValueTask Execute(IConsole console, SearchDbContext db, CancellationToken cancellationToken)
    {
        using FileStream f = OutputFile.Open(FileMode.Create);
        using var writer = new StreamWriter(f);
        foreach (SearchParagraph paragraph in db.Paragraphs.OrderBy(r => r.Id))
        {
            writer.Write(paragraph.Id);
            writer.Write('\t');
            writer.WriteLine(paragraph.Content);
        }

        return ValueTask.CompletedTask;
    }
}