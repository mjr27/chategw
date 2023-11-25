using System.ComponentModel;

namespace ChatEgw.UI.Persistence;

public enum SearchEntityTypeEnum
{
    [Description("person")] Person,
    [Description("place")] Place,
    [Description("reference")] Reference
}