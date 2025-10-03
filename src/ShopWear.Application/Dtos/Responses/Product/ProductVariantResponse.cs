using System.Text.Json.Serialization;
using ShopWear.DataAccess.Enums.ProductEnums;
using ShopWear.DataAccess.Models.Products;

namespace ShopWear.Application.Dtos.Responses.Products;

public record class ProductVariantResponse
{
    public int Id { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ProductSize Size { get; init; }
    public int Stock { get; init; }
    public decimal Price { get; init; }

    public static ProductVariantResponse FromEntity(ProductVariant productVariant)
        => new ProductVariantResponse()
        {
            Id = productVariant.Id,
            Size = productVariant.Size,
            Stock = productVariant.Stock,
            Price = productVariant.Price
        };
}