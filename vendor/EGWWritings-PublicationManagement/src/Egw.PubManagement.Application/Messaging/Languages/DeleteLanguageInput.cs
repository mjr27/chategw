namespace Egw.PubManagement.Application.Messaging.Languages;

/// <summary>
/// Deletes a language
/// </summary>
public class DeleteLanguageInput : IApplicationCommand
{
    /// <summary>
    /// Language code
    /// </summary>
    public required string Code { get; init; }
}