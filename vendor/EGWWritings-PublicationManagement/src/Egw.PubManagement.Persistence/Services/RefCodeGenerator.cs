using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Services.RefCodeGenerators;

using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.Persistence.Services;

internal class RefCodeGenerator : IRefCodeGenerator
{
    private readonly Dictionary<PublicationType, IRefCodeGenerator> _calculators;

    public RefCodeGenerator()
    {
        var bookRefCodeCalculator = new BookRcGenerator();
        var bibleRefCodeCalculator = new BibleRcGenerator();
        var periodicalPageBreakCalculator = new PeriodicalPageBreakRcGenerator();
        var dictionaryRefCodeCalculator = new DictionaryRcGenerator();
        var periodicalNoPageBreakCalculator = new PeriodicalNoPageRcGenerator();
        var manuscriptCalculator = new LtMsRcGenerator();
        _calculators = new Dictionary<PublicationType, IRefCodeGenerator>
        {
            [PublicationType.Book] = bookRefCodeCalculator,
            [PublicationType.BibleCommentary] = bookRefCodeCalculator,
            [PublicationType.Devotional] = bookRefCodeCalculator,
            // -
            [PublicationType.Bible] = bibleRefCodeCalculator,
            // -
            [PublicationType.PeriodicalPageBreak] = periodicalPageBreakCalculator,
            [PublicationType.PeriodicalNoPageBreak] = periodicalNoPageBreakCalculator,
            // -
            [PublicationType.Dictionary] = dictionaryRefCodeCalculator,
            [PublicationType.TopicalIndex] = dictionaryRefCodeCalculator,
            // -
            [PublicationType.Manuscript] = manuscriptCalculator,
            [PublicationType.ScriptureIndex] = bibleRefCodeCalculator,
        };
    }

    public string? Short(PublicationType type, string code, ParagraphMetadata? metadata)
    {
        return _calculators.GetValueOrDefault(type)?.Short(type, code, metadata);
    }

    public string? Long(PublicationType type, string title, ParagraphMetadata? metadata)
    {
        return _calculators.GetValueOrDefault(type)?.Long(type, title, metadata);
    }
}