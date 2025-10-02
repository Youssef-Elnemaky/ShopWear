using System.ComponentModel.DataAnnotations;
using ShopWear.DataAccess.Enums.ProductEnums;

namespace ShopWear.Application.Dtos.Requests.Products;

public record class UpdateProductVariantRequest
{
    public ProductSize Size { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "SKU must be greater than 0.")]
    public int Sku { get; init; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be positive.")]
    public decimal Price { get; init; }
}