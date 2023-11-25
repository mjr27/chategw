using System.ComponentModel;
namespace Egw.PubManagement.Persistence.Enums;

/// <summary>
/// Reference type
/// </summary>
public enum ReferenceTypeEnum
{
    /// <summary>
    /// Paragraph is system
    /// </summary>
    [Description("system")] System = 0,

    /// <summary>
    /// Paragraph is not referenced
    /// </summary>
    [Description("skipped")] Skipped,

    /// <summary>
    /// Paragraph is referenced
    /// </summary>
    [Description("referenced")] Referenced
}