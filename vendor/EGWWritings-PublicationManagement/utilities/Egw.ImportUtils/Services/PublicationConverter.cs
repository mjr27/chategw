using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

using Egw.PubManagement.LegacyImport;
using Egw.PubManagement.LegacyImport.LinkRepository;

using EnumsNET;

using Microsoft.Extensions.Logging;
namespace Egw.ImportUtils.Services;

public class PublicationConverter : IDisposable
{
    private readonly ILogger<PublicationConverter> _logger;
    private readonly HtmlParser _htmlParser;
    private readonly ILegacyHtmlConverter _converter;

    public PublicationConverter(
        ILoggerFactory loggerFactory,
        ILinkRepository linkRepository)
    {
        _logger = loggerFactory.CreateLogger<PublicationConverter>();
        _htmlParser = new HtmlParser(new HtmlParserOptions { IsAcceptingCustomElementsEverywhere = true });
        _converter = new LegacyHtmlConverter(linkRepository);
    }

    public void Convert(string inputFile, string outputFile, bool continueOnError, WarningLevel minLevel)
    {
        using FileStream f = File.OpenRead(inputFile);
        IHtmlDocument document = _htmlParser.ParseDocument(f);
        f.Dispose();

        IHtmlDocument convertedDocument = _converter.Convert(document, out IReadOnlyCollection<CollectedWarning> warnings);

        if (!continueOnError && warnings.Any(w => w.Level == WarningLevel.Error))
        {
            _logger.LogError("Stopping {Filename}", inputFile);
            throw new OperationCanceledException($"Convert cancelled because of error in {inputFile}");
        }

        using IDisposable? bookScope = _logger.BeginScope("Processing file {Filename}", inputFile);
        foreach (IGrouping<IElement, CollectedWarning> group in warnings
                     .Where(r => r.Level > WarningLevel.Information)
                     .GroupBy(r => r.Root))
        {
            using IDisposable? scope = _logger.BeginScope("Paragraph {Paragraph}", group.Key.GetAttribute("id") ?? "unknown");
        }

        if (warnings.Any(r => r.Level >= minLevel))
        {
            FileStream outputLogFile = File.Open($"{outputFile}.log.txt", FileMode.Create);
            using var f2 = new StreamWriter(outputLogFile);
            foreach (CollectedWarning warning in warnings.Where(r => r.Level >= minLevel))
            {
                string elementId = warning.Root.GetAttribute("id") ?? "0";
                f2.WriteLine(
                    $"[{warning.Level.AsString(EnumFormat.Description)}:{elementId,8}] {warning.Message}\n{Indent(warning.Node.ToHtml())}"
                );
            }

            return;
        }

        using FileStream fOutput = File.Open(outputFile, FileMode.Create);
        using var writer = new StreamWriter(fOutput);
        convertedDocument.ToHtml(writer, new HtmlMarkupFormatter());
    }

    private static string Indent(string text, string indent = "\t")
    {
        return indent + text.Replace("\n", "\n" + indent);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _converter.Dispose();
    }
}