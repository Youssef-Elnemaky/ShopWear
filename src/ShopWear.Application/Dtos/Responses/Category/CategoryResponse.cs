namespace ShopWear.Application.Dtos.Responses.Category;

public record class CategoryResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string? Description { get; init; }
}