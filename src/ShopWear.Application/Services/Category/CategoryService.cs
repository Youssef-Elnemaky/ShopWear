using ShopWear.Application.Common.Results;
using ShopWear.Application.Dtos.Requests.Category;
using ShopWear.Application.Dtos.Responses.Category;
using ShopWear.DataAccess.Interfaces.Repositories;
using ShopWear.DataAccess.Models.Products;

namespace ShopWear.Application.Services.Category;

public sealed class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _uow;

    public CategoryService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request)
    {
        //check if the name is unique
        var exists = await _uow.ProductCategories.GetAsync(c => c.Name == request.Name);
        if (exists is not null) return CategoryError.CategoryNameExists(request.Name);

        var category = new ProductCategory()
        {
            Name = request.Name,
            Description = request.Description
        };

        await _uow.ProductCategories.AddAsync(category);
        await _uow.SaveAsync();

        var catRes = new CategoryResponse()
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };

        return catRes;
    }

    public async Task<Result<Deleted>> DeleteCategoryAsync(int id)
    {
        var category = await _uow.ProductCategories.GetAsync(c => c.Id == id);
        if (category is null) return CategoryError.NotFound(id);

        await _uow.ProductCategories.DeleteAsync(category);
        await _uow.SaveAsync();

        return ResultTypes.Deleted;
    }

    public async Task<Result<IReadOnlyList<CategoryResponse>>> GetCategoriesAsync()
    {
        var categories = await _uow.ProductCategories.GetAllAsync(orderBy: q => q.OrderBy(c => c.Name));

        var categoryResponses = categories.Select(c => new CategoryResponse()
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        }).ToList().AsReadOnly();

        return categoryResponses;
    }

    public async Task<Result<CategoryResponse>> GetCategoryAsync(int id)
    {
        var category = await _uow.ProductCategories.GetAsync(c => c.Id == id);
        if (category is null) return CategoryError.NotFound(id);

        var result = new CategoryResponse()
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };

        return result;
    }

    public async Task<Result<Updated>> UpdateCategoryAsync(int id, UpdateCategoryRequest request)
    {
        var category = await _uow.ProductCategories.GetAsync(c => c.Id == id);
        if (category is null) return CategoryError.NotFound(id);

        //don't allow duplicate category names
        var exists = await _uow.ProductCategories.GetAsync(c => c.Name == request.Name);
        if (exists is not null) return CategoryError.CategoryNameExists(request.Name);


        category.Name = request.Name;
        category.Description = request.Description;

        await _uow.ProductCategories.UpdateAsync(category);
        await _uow.SaveAsync();

        return ResultTypes.Updated;
    }
}