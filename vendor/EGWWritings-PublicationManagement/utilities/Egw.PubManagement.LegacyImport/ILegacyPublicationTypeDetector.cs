using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.LegacyImport;

/// <summary>
/// Legacy publication type detector
/// </summary>
public interface ILegacyPublicationTypeDetector
{
    /// <summary>
    /// Retrieves publication type for given type and subtype
    /// </summary>
    /// <param name="type">Publication type</param>
    /// <param name="subType">Publication subtype</param>
    /// <returns></returns>
    PublicationType? GetPublicationType(string type, string subType);
}