namespace Egw.PubManagement.Application.Messaging.Paragraphs
{
    /// <summary>
    /// Add a paragraph
    /// </summary>
    public class AddParagraphInput : IApplicationCommand
    {
        /// <summary>
        /// ID of publication for paragraph
        /// </summary>
        public required int PublicationId { get; init; }
        /// <summary>
        /// Content of new paragraph
        /// </summary>
        public required String Content { get; init; }
        /// <summary>
        /// Element before that added new paragraph
        /// </summary>
        public int? Before { get; init; }
    }
}
