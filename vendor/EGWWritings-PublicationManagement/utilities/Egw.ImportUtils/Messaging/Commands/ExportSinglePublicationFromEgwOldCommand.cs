using MediatR;
namespace Egw.ImportUtils.Messaging.Commands;

/// <summary>
/// Export old publications to target directory
/// </summary>
/// <param name="ConnectionString">Connection string</param> 
/// <param name="OutputStream">Stream to write publication to</param> 
/// <param name="PublicationId">Publication ID</param>
public record ExportSinglePublicationFromEgwOldCommand(
        string ConnectionString,
        Stream OutputStream,
        int PublicationId)
    : IRequest;