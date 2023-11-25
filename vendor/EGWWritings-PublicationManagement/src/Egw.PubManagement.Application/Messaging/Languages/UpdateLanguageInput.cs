namespace Egw.PubManagement.Application.Messaging.Languages;

/// <summary>
/// Changes language details
/// </summary>
public class UpdateLanguageInput : IApplicationCommand
{
    /// <summary>
    /// Language code
    /// </summary>
    public required string Code { get; init; }
    /// <summary>
    /// New EGW Code
    /// </summary>
    public string? EgwCode { get; init; }
    /// <summary>
    /// New BCP47 Code
    /// </summary>
    public string? Bcp47Code { get; init; }
    /// <summary>
    /// New Title
    /// </summary>
    public string? Title { get; init; }
}