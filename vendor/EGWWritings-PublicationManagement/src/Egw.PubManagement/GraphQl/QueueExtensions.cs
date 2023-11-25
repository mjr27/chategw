using Egw.PubManagement.Core;
using Egw.PubManagement.GraphQl.Namespaces;
namespace Egw.PubManagement.GraphQl;

/// <summary>
/// Queue extensions
/// </summary>
[ExtendObjectType(typeof(GraphQlQueries))]
public class QueueExtensions
{
    /// <summary>
    /// Queue-related stats
    /// </summary>
    /// <returns></returns>
    public QueueNamespace Queue()
    {
        return new QueueNamespace();
    }
}