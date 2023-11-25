using WhiteEstate.DocFormat;
namespace Egw.PubManagement.Preview.Models.Internal;

/// <summary> Paragraph order DTO </summary>
public readonly struct ChapterOrderDto
{
    /// <summary> Paragraph id </summary>
    public readonly ParaId ParaId;
    /// <summary> Chapter id </summary>
    public readonly ParaId ChapterId;
    /// <summary> Order </summary>
    public readonly int Order;
    /// <summary> End Order </summary>
    public readonly int EndOrder;

    /// <summary> Default constructor </summary>
    public ChapterOrderDto(ParaId paraId, ParaId chapterId, int order, int endOrder)
    {
        ParaId = paraId;
        ChapterId = chapterId;
        Order = order;
        EndOrder = endOrder;
    }
}