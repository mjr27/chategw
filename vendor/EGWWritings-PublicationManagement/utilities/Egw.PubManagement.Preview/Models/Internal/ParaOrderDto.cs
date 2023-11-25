using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Preview.Models.Internal;

/// <summary> Paragraph order DTO </summary>
public readonly struct ParaOrderDto
{
    /// <summary> Paragraph id </summary>
    public readonly ParaId ParaId;
    /// <summary> Order </summary>
    public readonly int Order;

    /// <summary> Default constructor </summary>
    public ParaOrderDto(ParaId paraId, int order)
    {
        ParaId = paraId;
        Order = order;
    }
}