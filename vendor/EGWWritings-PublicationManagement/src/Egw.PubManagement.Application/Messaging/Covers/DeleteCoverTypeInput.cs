namespace Egw.PubManagement.Application.Messaging.Covers;

/// <summary> Delete cover type input </summary>
public class DeleteCoverTypeInput : IApplicationCommand
{
    /// <summary> Cover type code </summary>
    public required string Code { get; init; }
}