using System.ComponentModel.DataAnnotations;

namespace ShopWear.Application.Dtos.Requests.Products;

public record class UpdateProductColorRequest
{
    [Required(ErrorMessage = "Product name is required.")]
    [MaxLength(200, ErrorMessage = "Product name cannot exceed 200 characters.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "Product name cannot be whitespace.")]
    public string Color { get; init; } = "";

    [MinLength(1, ErrorMessage = "Product must have at least one color")]
    public List<CreateProductVariantRequest> ProductVariants { get; init; } = [];
}