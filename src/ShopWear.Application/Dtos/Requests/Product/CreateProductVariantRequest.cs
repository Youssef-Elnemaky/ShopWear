using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ShopWear.DataAccess.Enums.ProductEnums;
using ShopWear.DataAccess.Models.Products;

namespace ShopWear.Application.Dtos.Requests.Products;

public record class CreateProductVariantRequest
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ProductSize Size { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "SKU must be greater than 0.")]
    public int Sku { get; init; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be positive.")]
    public decimal Price { get; init; }

    public ProductVariant ToEntity()
        => new ProductVariant()
        {
            Size = Size,
            Sku = Sku,
            Price = Price
        };
}