public sealed record ProductListParams
{
    public string? Search { get; init; }
    public int? CategoryId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string SortBy { get; init; } = "price";
    public bool Desc { get; init; } = false;
}