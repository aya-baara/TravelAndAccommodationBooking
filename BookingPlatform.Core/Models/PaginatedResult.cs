public class PaginatedResult<T>
{
    public List<T> Items { get; set; }
    public PaginationMetadata Pagination { get; set; }

    public PaginatedResult(List<T> items, int totalItems, int page, int pageSize)
    {
        Items = items;
        Pagination = new PaginationMetadata
        {
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize
        };
    }
}
