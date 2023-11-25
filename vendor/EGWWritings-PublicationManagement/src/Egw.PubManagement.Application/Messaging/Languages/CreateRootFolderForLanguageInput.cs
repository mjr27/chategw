namespace Egw.PubManagement.Application.Messaging.Languages;

/// <summary>
/// Create folder for language
/// </summary>
public class CreateRootFolderForLanguageInput : IApplicationCommand
{
    /// <summary>
    /// Language code
    /// </summary>
    public required string Code { get; init; }
}