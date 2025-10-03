using System.ComponentModel.DataAnnotations;

namespace ShopWear.Application.Dtos.Requests.Products;

public record class UpdateProductRequest
{
    [Required]
    public int CategoryId { get; init; }

    [Required(ErrorMessage = "Product name is required.")]
    [MaxLength(200, ErrorMessage = "Product name cannot exceed 200 characters.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "Product name cannot be whitespace.")]
    public string Name { get; init; } = "";

    [MaxLength(4000, ErrorMessage = "Product description cannot exceed 4000 characters.")]
    public string? Description { get; init; }

    [MinLength(1, ErrorMessage = "Product must have at least one color")]
    [MaxLength(5, ErrorMessage = "Product cannot have more than 5 colors")]
    public List<UpdateProductColorRequest> ProductColors { get; init; } = [];
}