using System.ComponentModel;
namespace WhiteEstate.DocFormat.Json.Serializers;

/// <summary>
/// Json node type.
/// </summary>
public enum JsonNodeType
{
    /// <summary>
    /// Table
    /// </summary>
    [Description("table")]
    Table,

    /// <summary>
    /// Table row
    /// </summary>
    [Description("tr")]
    TableRow,

    /// <summary>
    /// Table cell
    /// </summary>
    [Description("td")]
    TableCell,

    /// <summary>
    /// Text
    /// </summary>
    [Description("text")]
    TextNode,

    /// <summary>
    /// Line break
    /// </summary>
    [Description("br")]
    LineBreak,

    /// <summary>
    /// Thought break / HR
    /// </summary>
    [Description("hr")]
    ThoughtBreak,


    /// <summary>
    /// Image
    /// </summary>
    [Description("image")]
    Image,
    /// <summary>
    /// Text format
    /// </summary>
    [Description("text-format")]
    TextFormat,

    /// <summary>
    /// Sentence
    /// </summary>
    [Description("sentence")]
    Sentence,
    /// <summary>
    /// Page break
    /// </summary>
    [Description("page-break")]
    PageBreak,

    /// <summary>
    /// List
    /// </summary>
    [Description("list")]
    List,
    /// <summary>
    /// List item
    /// </summary>
    [Description("list-item")]
    ListItem,


    /// <summary>
    /// Link
    /// </summary>
    [Description("link")]
    Link,

    /// <summary>
    /// Anchor
    /// </summary>
    [Description("anchor")]
    Anchor,

    /// <summary>
    /// Language
    /// </summary>
    [Description("lang")]
    Language,

    /// <summary>
    /// Non-Egw text
    /// </summary>
    [Description("non-egw")]
    NonEgw,

    /// <summary>
    /// Entity
    /// </summary>
    [Description("entity")]
    Entity,


    /// <summary>
    /// Paragraph
    /// </summary>
    [Description("paragraph")]
    Paragraph,

    /// <summary>
    /// Heading container
    /// </summary>
    [Description("heading-container")]
    HeadingContainer,

    /// <summary>
    /// Paragraph container
    /// </summary>
    [Description("paragraph-group-container")]
    ParagraphGroupContainer,
    
    /// <summary>
    /// Paragraph container
    /// </summary>
    [Description("paragraph-container")]
    ParagraphContainer,

    /// <summary>
    /// Note
    /// </summary>
    [Description("note")]
    Note,
}