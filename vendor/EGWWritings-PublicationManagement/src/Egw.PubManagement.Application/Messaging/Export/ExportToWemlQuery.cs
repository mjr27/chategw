using MediatR;

using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Application.Messaging.Export;

/// <summary>
/// Exports current publication to Weml
/// </summary>
/// <param name="PublicationId"></param>
public record ExportToWemlQuery(int PublicationId) : IRequest<WemlDocument>;