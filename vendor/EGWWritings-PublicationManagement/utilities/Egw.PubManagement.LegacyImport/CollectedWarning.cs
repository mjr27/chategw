using AngleSharp.Dom;
namespace Egw.PubManagement.LegacyImport;

/// <summary>
/// Collected warning
/// </summary>
/// <param name="Level"></param>
/// <param name="Node"></param>
/// <param name="Root"></param>
/// <param name="Message"></param>
public record CollectedWarning(WarningLevel Level, INode Node, IElement Root, string Message);