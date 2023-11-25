using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WhiteEstate.DocFormat;

namespace ChatEgw.UI.Persistence.Internal;

/// <summary> 
/// Database converter for <see cref="ParaId"/>
/// </summary>
public class ParaIdConverter : ValueConverter<ParaId, string>
{
    /// <inheritdoc />
    public ParaIdConverter() : base(paraId => paraId.ToString(), paraIdStr => ParaId.Parse(paraIdStr))
    {
        
    }
}