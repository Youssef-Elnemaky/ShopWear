using ShopWear.DataAccess.Models.Products;

namespace ShopWear.Application.Dtos.Responses.Products;

public record class ProductColorResponse
{
    public int Id { get; init; }
    public string Color { get; init; } = "";
    public bool IsMainColor { get; init; }
    public IReadOnlyList<ProductImageResponse> ProductImages { get; init; } = [];
    public IReadOnlyList<ProductVariantResponse> ProductVariants { get; init; } = [];

    public static ProductColorResponse FromEntity(ProductColor productColor)
        => new ProductColorResponse()
        {
            Id = productColor.Id,
            Color = productColor.Color,
            IsMainColor = productColor.IsMainColor,
            ProductImages = productColor.ProductImages.Select(ProductImageResponse.FromEntity).ToList(),
            ProductVariants = productColor.ProductVariants.Select(ProductVariantResponse.FromEntity).ToList()
        };
}