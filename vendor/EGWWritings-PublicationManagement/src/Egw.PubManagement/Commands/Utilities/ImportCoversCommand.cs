using System.Text.Json;

using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

using Egw.PubManagement.Application.Messaging.Covers;
using Egw.PubManagement.Core.Problems;

using MediatR;

namespace Egw.PubManagement.Commands.Utilities;

/// <summary>
/// Migration command
/// </summary>
[Command("import covers", Description = "Imports covers from a folder")]
public class ImportCoversCommand : ICommand
{
    /// <summary>
    /// List of books to process
    /// </summary>
    [CommandParameter(0, Description = "Folder to import from", IsRequired = true)]
    public DirectoryInfo Folder { get; init; } = null!;

    private readonly WebApplication _application;

    /// <summary>
    /// Default constructor
    /// </summary>
    public ImportCoversCommand(WebApplication application)
    {
        _application = application;
    }

    private class FileSystemFile : IFile
    {
        private readonly FileInfo _file;

        public FileSystemFile(FileInfo file)
        {
            _file = file;
        }

        public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = new())
        {
            await using Stream f = OpenReadStream();
            await f.CopyToAsync(target, cancellationToken);
        }

        public Stream OpenReadStream() => _file.OpenRead();

        public string Name => _file.Name;
        public long? Length => _file.Length;
        public string? ContentType => null;
    }

    /// <inheritdoc />
    public async ValueTask ExecuteAsync(IConsole console)
    {
        CancellationToken ct = console.RegisterCancellationHandler();
        using IServiceScope scope = _application.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        IEnumerable<FileInfo> files = Folder.EnumerateFiles("*_k.jpg").OrderBy(r => r.Name);
        foreach (FileInfo file in files)
        {
            string[] fileChunks = file.Name.Split('_', 2);
            if (!int.TryParse(fileChunks[0], out int publicationId))
            {
                continue;
            }

            var cmd = new UploadCoverInput
            {
                File = new FileSystemFile(file), Type = "web", PublicationId = publicationId, Id = Guid.NewGuid()
            };
            await console.Output.WriteLineAsync($"Importing {file.FullName}");
            try
            {
                await mediator.Send(cmd, ct);
            }
            catch (ValidationProblemDetailsException e)
            {
                await console.Output.WriteLineAsync(e.Message);
                await console.Output.WriteLineAsync(JsonSerializer.Serialize(e.Details));
            }
        }
    }
}