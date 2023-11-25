namespace Egw.ImportUtils.Services.Cache;

public record LinkGeneratorParagraphLinks(string LanguageCode, int BookId, int ParagraphId, IReadOnlyCollection<string> RefCodes);