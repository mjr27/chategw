namespace ChatEgw.UI.Application.Models;

public class SearchFilterRequest
{
    /// <summary> Search folders </summary>
    public string[] Folders { get; init; } = Array.Empty<string>();

    public bool? IsEgw { get; set; } = null;

    public DateOnly? MinDate { get; set; } = null;
    public DateOnly? MaxDate { get; set; } = null;
}