namespace Web.Entites.Consts;
public record FilterRequest(
    string? SearchTerm,
    string? SortField,
    string? SortOrder,
    int PageNumber = 1
);