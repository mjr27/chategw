using System.ComponentModel;
namespace Egw.PubManagement.Persistence.Enums;

/// <summary>
/// Series type
/// </summary>
public enum SeriesTypeEnum
{
    /// <summary>
    /// Topic
    /// </summary>
    [Description("topic")] Topic,

    /// <summary>
    /// Series
    /// </summary>
    [Description("series")] Series
}