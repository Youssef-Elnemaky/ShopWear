using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShopWear.Application.Common.Errors;
using ShopWear.Application.Common.Pagination;
using ShopWear.Application.Dtos.Requests.Products;
using ShopWear.Application.Dtos.Responses.Products;
using ShopWear.Application.Services.Products;
using ShopWear.DataAccess.Models.Products;

namespace ShopWear.api.Controllers.Products.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductController> logger;

    public ProductController(IProductService productService, ILogger<ProductController> logger)
    {
        _productService = productService;
        this.logger = logger;
    }

    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<Error>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductDetailResponse>> CreateProduct(CreateProductRequest request)
    {
        logger.LogInformation("CreateProduct called with {@request}", request);
        var result = await _productService.CreateProductAsync(request);

        return result.Match<ActionResult>(
            product =>
            {
                logger.LogInformation("CreateProduct succeeded for {Id}", product.Id);
                return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
            },
            errors =>
            {
                var error = errors[0];
                switch (error.Kind)
                {
                    case ErrorKind.Conflict:
                        logger.LogWarning("CreateProduct conflict error {@Error}", error);
                        return Conflict(error);
                    case ErrorKind.Validation:
                        logger.LogWarning("CreateProduct validation error {@Error}", error);
                        return BadRequest(error);
                    case ErrorKind.NotFound:
                        logger.LogWarning("CreateProduct notfound error {@Error}", error);
                        return NotFound(error);
                    default:
                        logger.LogError("CreateCategory unhandled error for {request}: {@Error}", request, error);
                        throw new InvalidOperationException($"Unhandled error kind {error.Kind}");
                }
            }
        );
    }

    [HttpGet("{id}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductDetailResponse>> GetProductById(int id)
    {
        logger.LogInformation("GetProductById called with id: {Id}", id);

        var result = await _productService.GetProductByIdAsync(id);

        return result.Match<ActionResult>(
            product =>
            {
                logger.LogInformation("GetProductById succeeded for {Id}", id);
                return Ok(product);
            },
            errors =>
            {
                var error = errors[0];

                switch (error.Kind)
                {
                    case ErrorKind.NotFound:
                        logger.LogWarning("GetProductById not found error for {Id}: {@Error}", id, error);
                        return NotFound(error);
                    default:
                        logger.LogError("GetProductById unhandled error for {Id}: {@Error}", id, error);
                        throw new InvalidOperationException($"Unhandled error kind {error.Kind}");
                }
            });
    }

    [HttpGet]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<ProductSummaryResponse>>> GetProducts([FromQuery] ProductListParams queryParams)
    {
        var result = await _productService.GetProductsAsync(queryParams);
        return result.Match<ActionResult>(
            products =>
            {
                logger.LogInformation("GetProductsAsync succeeded");
                return Ok(products);
            },
            errors =>
            {
                var error = errors[0];

                switch (error.Kind)
                {
                    default:
                        logger.LogError("GetProductsAsync unhandled error: {@Error}", error);
                        throw new InvalidOperationException($"Unhandled error kind {error.Kind}");
                }
            });
    }



    [HttpPut("{id}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Error>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductRequest request)
    {
        logger.LogInformation("UpdateProduct called with {Id}", id);

        var result = await _productService.UpdateProductAsync(id, request);

        return result.Match<ActionResult>(
            product =>
            {
                logger.LogInformation("Product with id: {Id} updated successfully", id);
                return NoContent();
            },
            errors =>
            {
                var error = errors[0];
                switch (error.Kind)
                {
                    case ErrorKind.NotFound:
                        logger.LogWarning("UpdateProduct not found error for {Id}: {@Error}", id, error);
                        return NotFound(error);
                    case ErrorKind.Conflict:
                        logger.LogWarning("UpdateProduct conflict error {@Error}", error);
                        return Conflict(error);
                    case ErrorKind.Validation:
                        logger.LogWarning("UpdateProduct validation error {@Error}", error);
                        return BadRequest(error);
                    default:
                        logger.LogError("UpdateProduct unhandled error for {Id}: {@Error}", id, error);
                        throw new InvalidOperationException($"Unhandled error kind {error.Kind}");
                }
            }
        );
    }

    [HttpDelete("{id}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        logger.LogInformation("DeleteProduct called with id: {Id}", id);

        var result = await _productService.DeleteProductAsync(id);

        return result.Match<ActionResult>(
            category =>
            {
                logger.LogInformation("DeleteProduct succeeded for {Id}", id);
                return NoContent();
            },
            errors =>
            {
                var error = errors[0];
                switch (error.Kind)
                {
                    case ErrorKind.NotFound:
                        logger.LogWarning("DeleteProduct not found error for {Id}: {@Error}", id, error);
                        return NotFound(error);
                    default:
                        logger.LogError("DeleteProduct unhandled error for {Id}: {@Error}", id, error);
                        throw new InvalidOperationException($"Unhandled error kind {error.Kind}");
                }
            }
        );
    }


    [HttpPost("{productId:int}/colors/{colorId:int}/images")]
    [RequestSizeLimit(6 * 1024 * 1024)]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Error>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<ProductImageResponse>>> UploadImages(int productId, int colorId, List<IFormFile> files)
    {
        if (files is null) return BadRequest("You must upload at least 1 image.");
        var res = await _productService.AddImageAsync(productId, colorId, files);
        return res.Match<ActionResult>(
            productImage =>
            {
                logger.LogInformation("UploadImage succeeded for");
                return Ok(productImage);
            },
            errors =>
            {
                var error = errors[0];
                switch (error.Kind)
                {
                    case ErrorKind.NotFound:
                        logger.LogWarning("UploadImage not found error {@Error}", error);
                        return NotFound(error);
                    case ErrorKind.Conflict:
                        logger.LogWarning("UploadImage conflict error {@Error}", error);
                        return Conflict(error);
                    case ErrorKind.Validation:
                        logger.LogWarning("UploadImage validation error {@Error}", error);
                        return BadRequest(error);
                    default:
                        logger.LogError("UpdateProduct unhandled error for {Id}: {@Error}", productId, error);
                        throw new InvalidOperationException($"Unhandled error kind {error.Kind}");
                }
            }
        );
    }

    [HttpPut("{productId:int}/colors/{colorId:int}/images/{imageId:guid}/set-main")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SetMainImage(int productId, int colorId, Guid imageId)
    {

        var res = await _productService.SetMainImageAsync(productId, colorId, imageId);
        return res.Match<ActionResult>(
            productImage =>
            {
                logger.LogInformation("SetMainImage succeeded");
                return NoContent();
            },
            errors =>
            {
                var error = errors[0];
                switch (error.Kind)
                {
                    case ErrorKind.NotFound:
                        logger.LogWarning("UploadImage not found error {@Error}", error);
                        return NotFound(error);
                    default:
                        logger.LogError("UpdateProduct unhandled error for {Id}: {@Error}", productId, error);
                        throw new InvalidOperationException($"Unhandled error kind {error.Kind}");
                }
            }
        );
    }

    [HttpDelete("{productId:int}/colors/{colorId:int}/images/{imageId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteImage(int productId, int colorId, Guid imageId)
    {
        var result = await _productService.RemoveImageAsync(productId, colorId, imageId);
        return result.Match<ActionResult>(
            productImage =>
            {
                logger.LogInformation("DeleteImage succeeded for");
                return NoContent();
            },
            errors =>
            {
                var error = errors[0];
                switch (error.Kind)
                {
                    case ErrorKind.NotFound:
                        logger.LogWarning("UploadImage not found error {@Error}", error);
                        return NotFound(error);
                    default:
                        logger.LogError("UpdateProduct unhandled error for {Id}: {@Error}", productId, error);
                        throw new InvalidOperationException($"Unhandled error kind {error.Kind}");
                }
            }
        );
    }
}