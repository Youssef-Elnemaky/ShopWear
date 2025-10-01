using ShopWear.Application.Common.Errors;

namespace ShopWear.Application.Services.Category;

public static class CategoryError
{
    public static Error NotFound(int id)
        => Error.NotFound("Category.Id.NotFound", $"Category with id: {id} not found.");

    public static Error CategoryNameExists(string categoryName)
        => Error.Conflict($"Category.Name.Exists", $"Category with name: {categoryName} already exists.");
}