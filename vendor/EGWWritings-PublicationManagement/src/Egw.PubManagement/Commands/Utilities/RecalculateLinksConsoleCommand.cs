using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

using Egw.PubManagement.Application.Messaging.Import;

using MediatR;

namespace Egw.PubManagement.Commands.Utilities;

/// <summary>
/// Recalculates TOC
/// </summary>
[Command("recalc links", Description = "Recalculates Links")]
public class RecalculateLinksConsoleCommand : ICommand
{
    /// <summary>
    /// List of books to process
    /// </summary>
    [CommandParameter(0, Description = "List of books to process", IsRequired = true)]
    public int[] Books { get; init; } = Array.Empty<int>();

    private readonly WebApplication _application;


    /// <summary>
    /// Default constructor
    /// </summary>
    public RecalculateLinksConsoleCommand(WebApplication application)
    {
        _application = application;
    }

    /// <inheritdoc />
    public async ValueTask ExecuteAsync(IConsole console)
    {
        CancellationToken ct = console.RegisterCancellationHandler();
        using IServiceScope scope = _application.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        foreach (int publicationId in Books)
        {
            await mediator.Send(new RecalculateLinksCommand(publicationId), ct);
        }
    }
}