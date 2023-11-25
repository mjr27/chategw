namespace Egw.PubManagement.EpubExport.Types;

/// <summary>
/// Page info
/// </summary>
public class EpubPageInfo
{
    /// <summary>
    /// Page number
    /// </summary>
    public required string PageNumber { get; init; }

    /// <summary>
    /// Content SRC
    /// </summary>
    public required string ContentSrc { get; init; }

    /// <summary>
    /// Play order 
    /// </summary>
    public int PlayOrder { get; set; }
}