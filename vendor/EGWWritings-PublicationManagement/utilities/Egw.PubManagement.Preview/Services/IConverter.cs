using AngleSharp.Dom;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;
namespace Egw.PubManagement.Preview.Services;

internal interface IConverter
{
    INode Serialize(PublicationType publicationType, IWemlNode node);
}