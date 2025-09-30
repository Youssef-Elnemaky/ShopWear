namespace ShopWear.Application.Common.Pagination;

public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int Page { get; }          // 1-based
    public int PageSize { get; }
    public long TotalCount { get; }
    public int TotalPages { get; }
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;

    public PagedResult(IEnumerable<T> items, int page, int pageSize, long totalCount)
    {
        if (items is null) throw new ArgumentNullException(nameof(items));
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page));
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));
        if (totalCount < 0) throw new ArgumentOutOfRangeException(nameof(totalCount));

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        TotalPages = Math.Max(1, totalPages);

        if (totalCount == 0)
        {
            Items = Array.Empty<T>();
            Page = 1;
            PageSize = pageSize;
            TotalCount = 0;
            return;
        }

        if (page > TotalPages) throw new ArgumentOutOfRangeException(nameof(page));

        Items = items.ToList();
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
}