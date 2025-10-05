using Microsoft.AspNetCore.Http;
using ShopWear.Application.Common.Pagination;
using ShopWear.Application.Common.Results;
using ShopWear.Application.Dtos.Requests.Products;
using ShopWear.Application.Dtos.Responses.Products;

namespace ShopWear.Application.Services.Products;

public interface IProductService
{
    public Task<Result<ProductDetailResponse>> CreateProductAsync(CreateProductRequest request);
    public Task<Result<ProductDetailResponse>> GetProductByIdAsync(int id);
    public Task<Result<PagedResult<ProductSummaryResponse>>> GetProductsAsync(); // will be updated later to add filters
    public Task<Result<Updated>> UpdateProductAsync(int id, UpdateProductRequest request);
    public Task<Result<Deleted>> DeleteProductAsync(int id);
    Task<Result<ProductImageResponse>> AddImageAsync(int productId, int colorId, IFormFile file, bool isMain);
    Task<Result<Success>> RemoveImageAsync(int productId, int colorId, Guid imageId);
    Task<Result<Success>> SetMainImageAsync(int productId, int colorId, Guid imageId);
}