using ShopWear.DataAccess.Models.Products;

namespace ShopWear.Application.Dtos.Responses.Products;

public record class ProductDetailResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public string Category { get; init; } = "";
    public int CategoryId { get; init; }
    public IReadOnlyList<ProductColorResponse> ProductColors { get; init; } = [];

    public static ProductDetailResponse FromEntity(Product product)
        => new ProductDetailResponse()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description ?? "",
            Category = product.Category != null ? product.Category.Name : "",
            CategoryId = product.CategoryId,
            ProductColors = product.ProductColors.Select(ProductColorResponse.FromEntity).ToList()
        };
}