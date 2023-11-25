using AngleSharp.Html.Dom;
namespace Egw.PubManagement.LegacyImport;

/// <summary>
/// Converter of legacy HTML Files
/// </summary>
public interface ILegacyHtmlConverter : IDisposable
{
    /// <summary>
    /// Converts file to a new format
    /// </summary>
    /// <param name="document">Source document</param>
    /// <param name="warnings">List of collected warnings</param>
    /// <returns></returns>
    IHtmlDocument Convert(IHtmlDocument document, out IReadOnlyCollection<CollectedWarning> warnings);
}