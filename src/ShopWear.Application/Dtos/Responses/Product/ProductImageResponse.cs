using ShopWear.DataAccess.Models.Products;

namespace ShopWear.Application.Dtos.Responses.Products;

public record class ProductImageResponse
{
    public Guid Id { get; init; }
    public string ImageUrl { get; init; } = "";

    public static ProductImageResponse FromEntity(ProductImage productImage)
        => new ProductImageResponse()
        {
            Id = productImage.Id,
            ImageUrl = productImage.ImageUrl
        };
}