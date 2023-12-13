using ChatEgw.UI.Persistence;
using ChatEgw.UI.Persistence.Utils;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pgvector;
using ShellProgressBar;

namespace ChatEgw.UI.Indexer.ConsoleCommands;

[Command("import embeddings", Description = "Imports english publications from EGW Writings database")]
public class ImportChunksConsoleCommand : ConsoleCommandBase
{
    [CommandParameter(0,
        Description = "Database to convert from", IsRequired = true
    )]
    public required FileInfo FromFilename { get; init; }

    [CommandOption("batch-size", 'b', Description = "Batch size", IsRequired = false)]
    public int BatchSize { get; init; } = 10_000;

    protected override async ValueTask Execute(IConsole console, SearchDbContext db,
        CancellationToken cancellationToken)
    {
        await db.Chunks.ExecuteDeleteAsync(cancellationToken);
        db.ChangeTracker.AutoDetectChangesEnabled = false;
        var paragraphs = db.Paragraphs.Select(r => r.Id).ToHashSet();
        foreach (SearchChunk[] chunks in ReadFile(paragraphs).Chunk(BatchSize))
        {
            db.Chunks.AddRange(chunks);
            await db.SaveChangesAsync(cancellationToken);
        }

        db.ChangeTracker.AutoDetectChangesEnabled = true;
    }

    // ReSharper disable once CognitiveComplexity
    private IEnumerable<SearchChunk> ReadFile(IReadOnlySet<long> paragraphs)
    {
        var totalCount = 0;
        using (StreamReader f1 = FromFilename.OpenText())
        {
            while (f1.ReadLine() is not null)
            {
                totalCount++;
            }
        }

        ILogger<ImportChunksConsoleCommand> logger = LoggingFactory.CreateLogger<ImportChunksConsoleCommand>();
        logger.LogInformation("Total lines: {LineCount}", totalCount);
        using StreamReader f = FromFilename.OpenText();
        using var pb = new ProgressBar(totalCount, "Importing chunks", new ProgressBarOptions
        {
            DisplayTimeInRealTime = true
        });
        var i = 0;
        while (f.ReadLine()?.Trim() is { } line)
        {
            i++;
            string[] a = line.Split('\t');
            var id = long.Parse(a[0]);
            string content = a[1];
            if (!paragraphs.Contains(id))
            {
                logger.LogError("Unknown paragraph {Id} for content {Content}", id, content);
                continue;
            }

            byte[] buf = Convert.FromBase64String(a[2]);
            var embeddingFloat = new float[buf.Length / sizeof(float)];
            Buffer.BlockCopy(buf, 0, embeddingFloat, 0, buf.Length);
            var entities = new List<SearchEntity>();
            foreach (string s in a.Skip(3))
            {
                string[] buf2 = s.Split('-', 2);
                var type = GetEntityType(buf2[0]);
                if (type is not null)
                {
                    entities.Add(new SearchEntity
                    {
                        Type = type.Value,
                        Content = EntityUtilities.NormalizeEntityValue(buf2[1])
                    });
                }
            }

            entities = entities.DistinctBy(r => (r.Type, r.Content)).ToList();

            var chunk = new SearchChunk
            {
                ParagraphId = id,
                Content = content,
                Embedding = new Vector(embeddingFloat)
            };
            if (entities.Any())
            {
                chunk.Entities = entities;
            }

            yield return chunk;

            if (i % 1000 == 0)
            {
                pb.Tick(i);
            }
        }
    }

    private SearchEntityTypeEnum? GetEntityType(string typeCode)
    {
        return typeCode switch
        {
            "PERSON" => SearchEntityTypeEnum.Person,
            "GPE" => SearchEntityTypeEnum.Place,
            "LOC" => SearchEntityTypeEnum.Place,
            _ => null
        };
    }
}