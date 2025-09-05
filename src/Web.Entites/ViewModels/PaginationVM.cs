namespace Web.Entites.ViewModels;
public class PaginationVM
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
    public string? SearchTerm { get; set; }
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string Action { get; set; } = "Index";
    public string Controller { get; set; } = "";
}