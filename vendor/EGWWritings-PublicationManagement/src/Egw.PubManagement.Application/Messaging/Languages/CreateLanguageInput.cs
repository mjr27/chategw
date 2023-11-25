using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.Application.Messaging.Languages;

/// <summary>
/// Creates a new language
/// </summary>
public class CreateLanguageInput : IApplicationCommand
{
    /// <summary>
    /// Language code
    /// </summary>
    public required string Code { get; init; }
    /// <summary>
    /// EGW language code
    /// </summary>
    public required string EgwCode { get; init; }
    /// <summary>
    /// BCP47 language code
    /// </summary>
    public required string Bcp47Code { get; init; }
    /// <summary>
    /// Language title
    /// </summary>
    public required string Title { get; init; }
    /// <summary>
    /// Language direction
    /// </summary>
    public required WemlTextDirection Direction { get; init; }

}