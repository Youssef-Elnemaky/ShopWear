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
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/categories")]
public class ProductCategoryController : ControllerBase
{
    private readonly ICategoryService categoryService;
    private readonly ILogger<ProductCategoryController> logger;

    public ProductCategoryController(ICategoryService categoryService, ILogger<ProductCategoryController> logger)
    {
        this.categoryService = categoryService;
        this.logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> CreateCategory(CreateCategoryRequest request)
    {
        logger.LogInformation("CreateCategory called with {@request}", request);
        var result = await categoryService.CreateCategoryAsync(request);

        return result.Match<ActionResult>(
            category =>
            {
                logger.LogInformation("CreateCategory succeeded for {Id}", category.Id);
                return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
            },
            errors =>
            {
                var type = errors.First().Kind;
                switch (type)
                {
                    case ErrorKind.Validation:
                        logger.LogWarning("CreateCategory validation error {@Errors}", errors);
                        return BadRequest(errors);
                    case ErrorKind.Conflict:
                        logger.LogWarning("CreateCategory conflict error {@Errors}", errors);
                        return Conflict(errors);
                    default:
                        logger.LogError("CreateCategory unexpected error {@Errors}", errors);
                        return StatusCode(500, errors);
                }
            }
        );
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetCategoryById(int id)
    {
        logger.LogInformation("GetCategoryById called with id: {Id}", id);

        var result = await categoryService.GetCategoryAsync(id);

        return result.Match<ActionResult>(
            category =>
            {
                logger.LogInformation("GetCategoryById succeeded for {Id}", id);
                return Ok(category);
            },
            errors =>
            {
                var type = errors.First().Kind; // take first for simplicity

                switch (type)
                {
                    case ErrorKind.NotFound:
                        logger.LogWarning("GetCategoryById not found error for {Id}: {@Errors}", id, errors);
                        return NotFound(errors);
                    default:
                        logger.LogError("GetCategoryById unexpected error {@Errors}", errors);
                        return StatusCode(500, errors);
                }
            });
    }

    [HttpGet]
    public async Task<ActionResult> GetCategories()
    {
        logger.LogInformation("GetCategories called");
        var result = await categoryService.GetCategoriesAsync();
        if (result.IsError)
        {
            logger.LogError("GetCategories unexpected error {@Errors}", result.Errors);
            return StatusCode(500, result.Errors);
        }
        logger.LogInformation("GetCategories succeeded");
        return Ok(result.Value);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCategory(int id, UpdateCategoryRequest request)
    {
        logger.LogInformation("UpdateCategory called with {Id}", id);

        var result = await categoryService.UpdateCategoryAsync(id, request);

        return result.Match<ActionResult>(
            category =>
            {
                logger.LogInformation("Category with id: {Id} updated successfully", id);
                return NoContent();
            },
            errors =>
            {
                var type = errors.First().Kind;
                switch (type)
                {
                    case ErrorKind.NotFound:
                        logger.LogWarning("UpdateCategory not found error for {Id}: {@Errors}", id, errors);
                        return NotFound(errors);
                    default:
                        logger.LogError("UpdateCategory unexpected error for {Id}: {@Errors}", id, errors);
                        return StatusCode(500, errors);
                }
            }
        );
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategory(int id)
    {
        logger.LogInformation("DeleteCategory called with id: {Id}", id);

        var result = await categoryService.DeleteCategoryAsync(id);

        return result.Match<ActionResult>(
            category =>
            {
                logger.LogInformation("DeleteCategory succeeded for {Id}", id);
                return NoContent();
            },
            errors =>
            {
                var type = errors.First().Kind;
                switch (type)
                {
                    case ErrorKind.NotFound:
                        logger.LogWarning("DeleteCategory not found error for {Id}: {@Errors}", id, errors);
                        return NotFound(errors);
                    default:
                        logger.LogError("DeleteCategory unexpected error for {Id}: {@Errors}", id, errors);
                        return StatusCode(500, errors);
                }
            }
        );
    }
}