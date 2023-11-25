using System.ComponentModel;
namespace Egw.PubManagement.Preview.Models;

/// <summary>
/// Content direction
/// </summary>
public enum ContentDirection
{
    /// <summary> Up </summary>
    [Description("up")]
    Up,
    /// <summary> Down </summary>
    [Description("down")]
    Down,
    /// <summary> Both </summary>
    [Description("both")]
    Both
}