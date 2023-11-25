using System.ComponentModel;
namespace Egw.PubManagement.Preview.Models;

/// <summary>
/// Book Type
/// </summary>
public enum BookType
{
    /// <summary>Book</summary>
    [Description("book")]
    Book = 1,
    /// <summary>Bible version</summary>
    [Description("bible")]
    Bible = 2,
    /// <summary>Periodical</summary>
    [Description("periodical")]
    Periodical = 3,
    /// <summary>Letters/manuscripts bound volume</summary>
    [Description("manuscript")]
    LetterOrManuscript = 4,
    /// <summary>Scripture index</summary>
    [Description("scriptindex")]
    ScriptureIndex = 5,
    /// <summary>Topical index</summary>
    [Description("topicalindex")]
    TopicalIndex = 6,
    /// <summary>Dictionary</summary>
    [Description("dictionary")]
    Dictionary = 7,
}