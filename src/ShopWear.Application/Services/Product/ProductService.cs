using Microsoft.AspNetCore.Http;
using ShopWear.Application.Common.Errors;
using ShopWear.Application.Common.Pagination;
using ShopWear.Application.Common.Results;
using ShopWear.Application.Dtos.Requests.Products;
using ShopWear.Application.Dtos.Responses.Products;
using ShopWear.Application.Services.Files;
using ShopWear.DataAccess.Enums.ProductEnums;
using ShopWear.DataAccess.Interfaces.Repositories;
using ShopWear.DataAccess.Interfaces.Repositories.Products;
using ShopWear.DataAccess.Models.Products;

namespace ShopWear.Application.Services.Products;

public sealed class ProductService : IProductService
{
    private readonly IUnitOfWork _uow;
    private readonly IFileService _fileService;

    public ProductService(IUnitOfWork uow, IFileService fileService)
    {
        _uow = uow;
        _fileService = fileService;
    }

    public async Task<Result<ProductDetailResponse>> CreateProductAsync(CreateProductRequest request)
    {
        //validate that the category exists
        var category = await _uow.ProductCategories.GetAsync(c => c.Id == request.CategoryId);
        if (category is null) return ProductError.CategoryNotFound(request.CategoryId);

        //validate on the rest of product parameters
        var validationResult = ValidateCreateRequest(request);
        if (validationResult.IsError) return validationResult.FirstError;

        // create a new product object from the request
        var newProduct = request.ToEntity();

        // save it to the db
        newProduct = await _uow.Products.AddAsync(newProduct);
        await _uow.SaveAsync();

        newProduct.Category = category;
        return ProductDetailResponse.FromEntity(newProduct);
    }

    public async Task<Result<Deleted>> DeleteProductAsync(int id)
    {
        //check if the product exists
        var category = await _uow.Products.GetAsync(p => p.Id == id);
        if (category is null) return ProductError.ProductNotFound(id);

        await _uow.Products.DeleteAsync(category);
        await _uow.SaveAsync();

        return ResultTypes.Deleted;
    }

    public async Task<Result<ProductDetailResponse>> GetProductByIdAsync(int id)
    {
        //get the productResponse using projection 
        var product = await _uow.Products.GetByIdWithDetailsAsync(id);
        if (product is null) return ProductError.ProductNotFound(id);

        var productResponse = ProductDetailResponse.FromEntity(product);
        //check if the product exists for the passed id
        if (productResponse is null) return ProductError.ProductNotFound(id);

        return productResponse;
    }

    public Task<Result<PagedResult<ProductSummaryResponse>>> GetProductsAsync()
    {
        throw new NotImplementedException();
    }


    public async Task<Result<ProductImageResponse>> AddImageAsync(int productId, int colorId, IFormFile file, bool isMain)
    {
        var product = await _uow.Products.GetByProductIdAndColorIdAsync(productId, colorId, false);
        if (product is null) return ProductError.ProductNotFound(productId);

        var color = product.ProductColors.FirstOrDefault();
        if (color is null) return ProductError.ProductColorNotFound(colorId);

        //save the image
        var saved = await _fileService.SaveAsync(file, "uploads/products", FileKind.Image, 5 * 1024 * 1024);
        if (saved.IsError) return saved.FirstError;

        if (isMain)
        {
            foreach (var img in color.ProductImages)
            {
                img.IsMainImage = false;
            }
        }

        var imgEntity = new ProductImage()
        {
            ImageUrl = saved.Value,
            IsMainImage = isMain,
            ProductColorId = colorId
        };
        color.ProductImages.Add(imgEntity);
        await _uow.SaveAsync();
        return ProductImageResponse.FromEntity(imgEntity);
    }

    public Task<Result<Success>> RemoveImageAsync(int productId, int colorId, int imageId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Success>> SetMainImageAsync(int productId, int colorId, int imageId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Updated>> UpdateProductAsync(int id, UpdateProductRequest request)
    {
        //check if the category exists
        var category = await _uow.ProductCategories.GetAsync(c => c.Id == request.CategoryId);
        if (category is null) return ProductError.CategoryNotFound(request.CategoryId);

        //check if the product exists
        var product = await _uow.Products.GetByIdWithDetailsAsync(id, asNoTracking: false);
        if (product is null) return ProductError.ProductNotFound(id);

        // validate input just like create (same business rules)
        var validationResult = ValidateUpdateRequest(request);
        if (validationResult.IsError) return validationResult.FirstError;


        // remove old
        product.ProductColors.Clear();
        product.ProductColors = request.ProductColors.Select(c => new ProductColor()
        {
            Color = c.Color,
            ProductVariants = c.ProductVariants.Select(v => new ProductVariant()
            {
                Size = v.Size,
                Stock = v.Stock,
                Price = v.Price
            }).ToList()
        }).ToList();

        await _uow.SaveAsync();
        return ResultTypes.Updated;
    }


    private Result<Success> ValidateCreateRequest(CreateProductRequest request)
    {
        //validate on the name
        if (string.IsNullOrWhiteSpace(request.Name)) return ProductError.ProductNameRequired();
        if (request.Name.Length > 200) return ProductError.ProductNameMaxLength();

        //validate colors: at least one color and cannot exceed 5 colors
        if (request.ProductColors.Count is < 1 or > 5) return ProductError.ProductVariantsCount();

        var uniqueColors = new HashSet<string>();
        bool hasMainColorFlag = false;
        foreach (var color in request.ProductColors)
        {
            //empty color strings
            if (string.IsNullOrWhiteSpace(color.Color)) return ProductError.ProductColorRequired();
            //color string length
            if (color.Color.Length > 200) return ProductError.ProductColorLength();
            //color uniqueness
            if (!uniqueColors.Add(color.Color)) return ProductError.ProductColorConflict(color.Color);
            // check against the flag to see disallow multiple main colors on the product.
            if (hasMainColorFlag && color.IsMainColor) return ProductError.ProductMultipleMainColor();
            // first main color? set the flag
            if (!hasMainColorFlag && color.IsMainColor) hasMainColorFlag = true;
            // color must have a variant
            if (color.ProductVariants.Count is < 1 or > 6) return ProductError.ProductVariantsCount();
            //validate variants:
            var uniqueVariants = new HashSet<string>();

            foreach (var variant in color.ProductVariants)
            {
                //check if he entered the right variant size
                if (!Enum.IsDefined(typeof(ProductSize), variant.Size)) return ProductError.ProductVariantSizeInvalid((int)variant.Size);
                if (!uniqueVariants.Add(variant.Size.ToString())) return ProductError.ProductVariantConflict(variant.Size.ToString());
                if (variant.Stock < 1) return ProductError.ProductVariantStockInvalid();
                if (variant.Price <= 0) return ProductError.ProductVariantPriceInvalid();
            }
        }
        if (!hasMainColorFlag) return ProductError.ProductNoMainColor();
        return ResultTypes.Success;
    }


    //probably move this and create to ValidateCommon
    private Result<Success> ValidateUpdateRequest(UpdateProductRequest request)
    {
        //validate on the name
        if (string.IsNullOrWhiteSpace(request.Name)) return ProductError.ProductNameRequired();
        if (request.Name.Length > 200) return ProductError.ProductNameMaxLength();

        //validate colors: at least one color and cannot exceed 5 colors
        if (request.ProductColors.Count is < 1 or > 5) return ProductError.ProductVariantsCount();

        var uniqueColors = new HashSet<string>();
        bool hasMainColorFlag = false;
        foreach (var color in request.ProductColors)
        {
            //empty color strings
            if (string.IsNullOrWhiteSpace(color.Color)) return ProductError.ProductColorRequired();
            //color string length
            if (color.Color.Length > 200) return ProductError.ProductColorLength();
            //color uniqueness
            if (!uniqueColors.Add(color.Color)) return ProductError.ProductColorConflict(color.Color);
            // check against the flag to see disallow multiple main colors on the product.
            if (hasMainColorFlag && color.IsMainColor) return ProductError.ProductMultipleMainColor();
            // first main color? set the flag
            if (!hasMainColorFlag && color.IsMainColor) hasMainColorFlag = true;
            // color must have a variant
            // color must have a variant
            if (color.ProductVariants.Count is < 1 or > 6) return ProductError.ProductVariantsCount();
            //validate variants:
            var uniqueVariants = new HashSet<string>();

            foreach (var variant in color.ProductVariants)
            {
                //check if he entered the right variant size
                if (!Enum.IsDefined(typeof(ProductSize), variant.Size)) return ProductError.ProductVariantSizeInvalid((int)variant.Size);
                if (!uniqueVariants.Add(variant.Size.ToString())) return ProductError.ProductVariantConflict(variant.Size.ToString());
                if (variant.Stock < 1) return ProductError.ProductVariantStockInvalid();
                if (variant.Price <= 0) return ProductError.ProductVariantPriceInvalid();
            }
        }
        if (!hasMainColorFlag) return ProductError.ProductNoMainColor();
        return ResultTypes.Success;
    }

}