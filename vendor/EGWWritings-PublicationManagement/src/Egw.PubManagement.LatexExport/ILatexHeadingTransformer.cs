namespace Egw.PubManagement.LatexExport;

/// <summary>
/// Heading transformer
/// </summary>
public interface ILatexHeadingTransformer
{
    /// <summary>
    /// Trims heading
    /// </summary>
    /// <param name="language">Language code</param>
    /// <param name="text">Heading text</param>
    /// <returns>Transformed heading</returns>
    string TrimHeading(string language, string text);
}