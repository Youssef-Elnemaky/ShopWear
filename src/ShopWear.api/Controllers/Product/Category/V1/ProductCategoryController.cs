using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShopWear.Application.Common.Errors;
using ShopWear.Application.Common.Pagination;
using ShopWear.Application.Dtos.Requests.Category;
using ShopWear.Application.Services.Category;

namespace ShopWear.api.Controllers.Product.Category.V1;

[ApiController]
[Route("categories")]
public class ProductCategoryController : ControllerBase
{
    private readonly ICategoryService categoryService;

    public ProductCategoryController(ICategoryService categoryService)
    {
        this.categoryService = categoryService;
    }

    [HttpPost]
    public async Task<ActionResult> CreateCategory(CreateCategoryRequest request)
    {
        var result = await categoryService.CreateCategoryAsync(request);

        return result.Match<ActionResult>(
            category => CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category),
            errors =>
            {
                var type = errors.First().Kind;
                return type switch
                {
                    ErrorKind.Validation => BadRequest(errors),
                    _ => StatusCode(500, errors)
                };
            }
        );
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetCategoryById(int id)
    {
        var result = await categoryService.GetCategoryAsync(id);
        return result.Match<ActionResult>(
            category => Ok(category),
            errors =>
            {
                var type = errors.First().Kind; // take first for simplicity

                return type switch
                {
                    ErrorKind.NotFound => NotFound(errors),
                    _ => StatusCode(500, errors)
                };
            });
    }

    [HttpGet]
    public async Task<ActionResult> GetCategories()
    {
        var result = await categoryService.GetCategoriesAsync();

        return Ok(result.Value);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCategory(int id, UpdateCategoryRequest request)
    {
        var result = await categoryService.UpdateCategoryAsync(id, request);

        return result.Match<ActionResult>(
            category => NoContent(),
            errors =>
            {
                var type = errors.First().Kind;
                return type switch
                {
                    ErrorKind.NotFound => NotFound(errors),
                    _ => StatusCode(500, errors)
                };
            }
        );
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategory(int id)
    {
        var result = await categoryService.DeleteCategoryAsync(id);

        return result.Match<ActionResult>(
            category => NoContent(),
            errors =>
            {
                var type = errors.First().Kind;
                return type switch
                {
                    ErrorKind.NotFound => NotFound(errors),
                    _ => StatusCode(500, errors)
                };
            }
        );
    }
}