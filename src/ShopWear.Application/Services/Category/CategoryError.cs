using ShopWear.Application.Common.Errors;

namespace ShopWear.Application.Services.Category;

public static class CategoryError
{
    public static Error NotFound(int id)
        => Error.NotFound("Category.Id.NotFound", $"Category with {id} not found.");
}