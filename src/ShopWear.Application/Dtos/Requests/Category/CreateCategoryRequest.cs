using System.ComponentModel.DataAnnotations;

namespace ShopWear.Application.Dtos.Requests.Category;

public record class CreateCategoryRequest
{
    [Required(ErrorMessage = "Category name is required.")]
    [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "Category name cannot be whitespace.")]
    public string Name { get; init; } = "";

    [MaxLength(1000, ErrorMessage = "Category description cannot exceed 1000 characters.")]
    public string? Description { get; init; }
}