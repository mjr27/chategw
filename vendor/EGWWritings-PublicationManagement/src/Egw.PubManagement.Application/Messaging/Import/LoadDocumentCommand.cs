using Egw.PubManagement.Core.Messaging;
using Egw.PubManagement.Persistence;

using MediatR;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Application.Messaging.Import;

/// <summary>
/// Imports weml document
/// </summary>
public record LoadDocumentCommand(WemlDocument Document, bool Recalculate, bool SkipExisting) : IRequest,
    ITransactionalCommand<PublicationDbContext>;