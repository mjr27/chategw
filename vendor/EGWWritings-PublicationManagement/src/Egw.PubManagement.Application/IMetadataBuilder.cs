using Egw.PubManagement.Persistence.Entities;
namespace Egw.PubManagement.Application;

/// <summary>
/// Metadata builder
/// </summary>
public interface IMetadataBuilder
{
    /// <summary>
    /// Builds metadata for publication
    /// </summary>
    /// <param name="publication">Current publication</param>
    /// <param name="paragraphs">List of paragraphs</param>
    /// <param name="currentDate">Current moment</param>
    /// <returns></returns>
    IEnumerable<ParagraphMetadata> GetMetadata(
        Publication publication,
        IEnumerable<Paragraph> paragraphs,
        DateTimeOffset currentDate
    );
}