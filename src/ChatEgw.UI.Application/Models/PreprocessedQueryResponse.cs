namespace ChatEgw.UI.Application.Models;

public record PreprocessedEntity(string Type, string Text);

public record PreprocessedQueryResponse(bool IsQuestion, string NormalizedQuery, params PreprocessedEntity[] Entities);