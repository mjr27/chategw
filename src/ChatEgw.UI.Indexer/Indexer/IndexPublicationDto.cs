using WhiteEstate.DocFormat.Enums;

namespace ChatEgw.UI.Indexer.Indexer;

public record IndexPublicationDto(
    int PublicationId,
    string Code,
    string Title,
    PublicationType Type,
    int? PublicationYear,
    bool IsEgw,
    string[] PublicationCodes
);