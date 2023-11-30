namespace ChatEgw.UI.Application.Models;

public record PreprocessedQueryResponse(bool IsQuestion,
    string NormalizedQuery,
    List<string> References,
    List<PreprocessedEntity> Entities);