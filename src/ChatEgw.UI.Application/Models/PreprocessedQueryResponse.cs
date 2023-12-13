namespace ChatEgw.UI.Application.Models;

public record PreprocessedQueryResponse(
    bool IsQuestion,
    string NormalizedQuery,
    List<PreprocessedEntity> Entities,
    List<PreprocessedPublicationReference> References
);