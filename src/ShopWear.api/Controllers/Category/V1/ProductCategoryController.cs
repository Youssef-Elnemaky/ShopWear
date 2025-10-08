using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShopWear.Application.Common.Errors;
using ShopWear.Application.Common.Pagination;
using ShopWear.Application.Dtos.Requests.Category;
using ShopWear.Application.Dtos.Responses.Category;
using ShopWear.Application.Services.Category;

namespace ShopWear.api.Controllers.Category.V1;

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
    [Consumes("application/json")]
    [ProducesResponseType<CategoryResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<Error>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CategoryResponse>> CreateCategory(CreateCategoryRequest request)
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
                var error = errors[0];
                switch (error.Kind)
                {
                    case ErrorKind.Conflict:
                        logger.LogWarning("CreateCategory conflict error {@Error}", error);
                        return Conflict(error);
                    default:
                        logger.LogError("CreateCategory unhandled error for {request}: {@Error}", request, error);
                        throw new InvalidOperationException($"Unhandled error kind {error.Kind}");
                }
            }
        );
    }

    [HttpGet("{id}")]
    [ProducesResponseType<CategoryResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CategoryResponse>> GetCategoryById(int id)
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
                var error = errors[0]; // take first for simplicity

                switch (error.Kind)
                {
                    case ErrorKind.NotFound:
                        logger.LogWarning("GetCategoryById not found error for {Id}: {@Error}", id, error);
                        return NotFound(error);
                    default:
                        logger.LogError("GetCategoryById unhandled error for {Id}: {@Error}", id, error);
                        throw new InvalidOperationException($"Unhandled error kind {error.Kind}");
                }
            });
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<CategoryResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<CategoryResponse>>> GetCategories()
    {
        logger.LogInformation("GetCategories called");
        var result = await categoryService.GetCategoriesAsync();
        if (result.IsError)
        {
            logger.LogError("GetCategories unexpected error {@Error}", result.FirstError);
            return StatusCode(500, result.FirstError);
        }
        logger.LogInformation("GetCategories succeeded");
        return Ok(result.Value);
    }

    [HttpPut("{id}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Error>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryRequest request)
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
                var error = errors[0];
                switch (error.Kind)
                {
                    case ErrorKind.NotFound:
                        logger.LogWarning("UpdateCategory not found error for {Id}: {@Error}", id, error);
                        return NotFound(error);
                    case ErrorKind.Conflict:
                        logger.LogWarning("UpdateCategory conflict error {@Error}", error);
                        return Conflict(error);
                    default:
                        logger.LogError("UpdateCategory unhandled error for {Id}: {@Error}", id, error);
                        throw new InvalidOperationException($"Unhandled error kind {error.Kind}");
                }
            }
        );
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCategory(int id)
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
                var error = errors[0];
                switch (error.Kind)
                {
                    case ErrorKind.NotFound:
                        logger.LogWarning("DeleteCategory not found error for {Id}: {@Error}", id, error);
                        return NotFound(error);
                    default:
                        logger.LogError("DeleteCategory unhandled error for {Id}: {@Error}", id, error);
                        throw new InvalidOperationException($"Unhandled error kind {error.Kind}");
                }
            }
        );
    }
}