using System.ComponentModel.DataAnnotations;
using ShopWear.DataAccess.Models.Products;

namespace ShopWear.Application.Dtos.Requests.Products;

public record class CreateProductColorRequest
{
    [Required(ErrorMessage = "Color is required.")]
    [MaxLength(50, ErrorMessage = "Color cannot exceed 50 characters.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "Color cannot be whitespace.")]
    public string Color { get; init; } = "";

    [MinLength(1, ErrorMessage = "Color must have at least one variant")]
    [MaxLength(5, ErrorMessage = "Color cannot have more than 5 variants")]
    public List<CreateProductVariantRequest> ProductVariants { get; init; } = [];

    public ProductColor ToEntity()
        => new ProductColor()
        {
            Color = Color,
            ProductVariants = ProductVariants.Select(v => v.ToEntity()).ToList()
        };
}