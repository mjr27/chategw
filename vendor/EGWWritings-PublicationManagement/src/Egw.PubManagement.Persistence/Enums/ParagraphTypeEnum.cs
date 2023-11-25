using System.ComponentModel;
namespace Egw.PubManagement.Persistence.Enums;

/// <summary>
/// Paragraph type
/// </summary>
public enum ParagraphTypeEnum
{
    /// <summary>
    /// Header
    /// </summary>
    [Description("heading")] Heading = 1,

    /// <summary>
    /// Paragraph
    /// </summary>
    [Description("paragraph")] Paragraph = 0,

    /// <summary>
    /// Page
    /// </summary>
    [Description("page")] Page = 2
}