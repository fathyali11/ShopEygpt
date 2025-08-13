namespace Web.Entites.Consts;
public class PaginatedList<T>(List<T> items, int count, int pageIndex, int pageSize)
{
    public List<T> Items { get; private set; } = items;
    public int PageIndex { get; private set; } = pageIndex;
    public int TotalPages { get; private set; } = (int)Math.Ceiling(count / (double)pageSize);
    public int PageSize { get; private set; } = pageSize;
    public int TotalCount { get; private set; } = count;

    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public static PaginatedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
    {
        var count = source.Count();
        if (pageIndex < 1)
            pageIndex = PaginationConstants.DefaultPageIndex;
        if (pageSize < 1)
            pageSize = PaginationConstants.DefaultPageSize;
        var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }
}