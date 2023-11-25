namespace Egw.PubManagement.Preview.Models;

/// <summary> Result wrapper </summary>
/// <typeparam name="T"></typeparam>
public class ResultWrapper<T>
{
    /// <summary> Default constructor </summary>
    /// <param name="results"></param>
    public ResultWrapper(IReadOnlyCollection<T> results)
    {
        Results = results;
    }

    /// <summary>
    /// Total count of results
    /// </summary>
    public int Count => Results.Count();
    /// <summary>
    /// Items per page
    /// </summary>
    public int Ipp => 1000;
    /// <summary>
    /// Results
    /// </summary>
    public IReadOnlyCollection<T> Results { get; }
    /// <summary> Previous page URI </summary>
    public string? Previous => null;
    /// <summary> Next page URI </summary>
    public string? Next => null;
}