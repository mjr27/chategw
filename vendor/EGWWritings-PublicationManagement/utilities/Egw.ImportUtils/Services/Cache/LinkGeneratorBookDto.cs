namespace Egw.ImportUtils.Services.Cache;

public record LinkGeneratorBookDto(
    int BookId,
    uint IdPub,
    string PubType,
    string SubType,
    string Language,
    string PubCode,
    string PubName,
    string Publisher
);