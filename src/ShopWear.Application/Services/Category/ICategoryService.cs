using ShopWear.Application.Common.Pagination;
using ShopWear.Application.Common.Results;
using ShopWear.Application.Dtos.Requests.Category;
using ShopWear.Application.Dtos.Responses.Category;

namespace ShopWear.Application.Services.Category;

public interface ICategoryService
{
    Task<Result<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request);
    Task<Result<IReadOnlyList<CategoryResponse>>> GetCategoriesAsync();
    Task<Result<CategoryResponse>> GetCategoryAsync(int id);
    Task<Result<Updated>> UpdateCategoryAsync(int id, UpdateCategoryRequest request);
    Task<Result<Deleted>> DeleteCategoryAsync(int id);
}