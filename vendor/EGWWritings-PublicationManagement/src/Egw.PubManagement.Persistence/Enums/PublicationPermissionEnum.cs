using System.ComponentModel;
namespace Egw.PubManagement.Persistence.Enums;

/// <summary>
/// Publication permission
/// </summary>
public enum PublicationPermissionEnum
{
    /// <summary>
    /// Publication is hidden from export
    /// </summary>
    [Description("hidden")] Hidden = 0,

    /// <summary>
    /// Publication is public
    /// </summary>
    [Description("public")] Public = 1,

    /// <summary>
    /// Must me authorized
    /// </summary>
    [Description("authenticated")] AuthenticatedOnly = 2,

    /// <summary>
    /// Must be purchased
    /// </summary>
    [Description("purchased")] PurchasedOnly = 3
}