using System.ComponentModel;

namespace ChatEgw.UI.Application.Models;

public enum SearchTypeEnum
{
    [Description("Auto")] Auto,
    [Description("Q&A Search")] AiSearch,
    [Description("Keyword Search")] KeywordSearch
}