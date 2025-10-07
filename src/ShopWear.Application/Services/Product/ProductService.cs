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

        //get the min price
        newProduct.MinPrice = newProduct.ProductColors
        .SelectMany(c => c.ProductVariants)
        .Select(v => v.Price)
        .DefaultIfEmpty(0m)
        .Min();

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

    public async Task<Result<PagedResult<ProductSummaryResponse>>> GetProductsAsync(ProductListParams queryParams)
    {
        var pageSize = Math.Clamp(queryParams.PageSize, 1, 100);
        var page = queryParams.Page < 1 ? 1 : queryParams.Page;

        var fixedParams = queryParams with { PageSize = pageSize, Page = page };

        var (products, total) = await _uow.Products.GetPagedAsync(fixedParams);

        var data = products.Select(p =>
        {
            var mainColor = p.ProductColors.FirstOrDefault();
            var imgUrl = mainColor?.ProductImages.OrderByDescending(i => i.IsMainImage)
                                                 .ThenBy(i => i.Id)
                                                 .Select(i => i.ImageUrl)
                                                 .FirstOrDefault();
            return new ProductSummaryResponse(p.Id, p.Name, p.Category.Name, p.Category.Id, p.MinPrice, imgUrl);
        }).ToList();

        return new PagedResult<ProductSummaryResponse>(data, page, pageSize, total);
    }


    public async Task<Result<List<ProductImageResponse>>> AddImageAsync(int productId, int colorId, List<IFormFile> files)
    {
        var product = await _uow.Products.GetByProductIdAndColorIdAsync(productId, colorId, false);
        if (product is null) return ProductError.ProductNotFound(productId);

        var color = product.ProductColors.FirstOrDefault();
        if (color is null) return ProductError.ProductColorNotFound(colorId);

        //save the image
        foreach (var file in files)
        {
            var saved = await _fileService.SaveAsync(file, "uploads/products", FileKind.Image, 5 * 1024 * 1024);
            if (saved.IsError) return saved.FirstError;
            var imgEntity = new ProductImage()
            {
                ImageUrl = saved.Value,
                ProductColorId = colorId
            };
            color.ProductImages.Add(imgEntity);
        }

        await _uow.SaveAsync();

        var imagesResponse = color.ProductImages.Select(i => ProductImageResponse.FromEntity(i)).ToList();
        return imagesResponse;
    }

    public async Task<Result<Success>> RemoveImageAsync(int productId, int colorId, Guid imageId)
    {
        var product = await _uow.Products.GetByProductIdAndColorIdAsync(productId, colorId, false);
        if (product is null) return ProductError.ProductNotFound(productId);

        var color = product.ProductColors.FirstOrDefault();
        if (color is null) return ProductError.ProductColorNotFound(colorId);

        var image = color.ProductImages.FirstOrDefault(i => i.Id == imageId);
        if (image is null) return ProductError.ProductImageNotFound(imageId);

        await _fileService.DeleteAsync(image.ImageUrl);
        color.ProductImages.Remove(image);
        await _uow.SaveAsync();
        return ResultTypes.Success;
    }

    public async Task<Result<Success>> SetMainImageAsync(int productId, int colorId, Guid imageId)
    {
        var product = await _uow.Products.GetByProductIdAndColorIdAsync(productId, colorId, false);
        if (product is null) return ProductError.ProductNotFound(productId);

        var color = product.ProductColors.FirstOrDefault();
        if (color is null) return ProductError.ProductColorNotFound(colorId);

        foreach (var img in color.ProductImages)
        {
            img.IsMainImage = false;
            if (img.Id == imageId) img.IsMainImage = true;
        }
        await _uow.SaveAsync();
        return ResultTypes.Success;
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

        product.Name = request.Name;
        product.Description = request.Description;
        product.CategoryId = request.CategoryId;

        var existingColorById = product.ProductColors.ToDictionary(c => c.Id);

        var incomingColorIds = request.ProductColors.Where(c => c.Id.HasValue).Select(c => c.Id!.Value).ToHashSet();
        var colorsToRemove = product.ProductColors.Where(c => !incomingColorIds.Contains(c.Id)).ToList();

        //save the urls of the images before we remove them from the product so we can remove them from the file system later on
        var urlsToDelete = colorsToRemove.SelectMany(c => c.ProductImages).Select(i => i.ImageUrl).ToList();

        foreach (var color in colorsToRemove) product.ProductColors.Remove(color);

        //adding or updating
        foreach (var colorRequest in request.ProductColors)
        {
            if (colorRequest.Id is null || !existingColorById.TryGetValue(colorRequest.Id.Value, out var color))
            {
                //new color case as there was no id provided and also the id that was passed was not in the original color list
                var newColor = new ProductColor()
                {
                    Color = colorRequest.Color,
                    IsMainColor = colorRequest.IsMainColor,
                    ProductVariants = colorRequest.ProductVariants.Select(v => new ProductVariant()
                    {
                        Size = v.Size,
                        Stock = v.Stock,
                        Price = v.Price
                    }).ToList()
                };
                product.ProductColors.Add(newColor);
            }
            else
            {
                //not a new color so update it
                color.Color = colorRequest.Color;
                color.IsMainColor = colorRequest.IsMainColor;

                var variantsById = color.ProductVariants.ToDictionary(v => v.Id);

                var incomingVariantIds = colorRequest.ProductVariants.Where(v => v.Id.HasValue).Select(v => v.Id!.Value).ToHashSet();
                var variantsToRemove = color.ProductVariants.Where(v => !incomingVariantIds.Contains(v.Id)).ToList();

                foreach (var v in variantsToRemove) color.ProductVariants.Remove(v);

                foreach (var variantRequest in colorRequest.ProductVariants)
                {
                    if (variantRequest.Id is null || !variantsById.TryGetValue(variantRequest.Id.Value, out var variant))
                    {
                        //new variant
                        var newVariant = new ProductVariant()
                        {
                            Size = variantRequest.Size,
                            Stock = variantRequest.Stock,
                            Price = variantRequest.Price
                        };
                        color.ProductVariants.Add(newVariant);
                    }
                    else
                    {
                        // already here so update it
                        variant.Size = variantRequest.Size;
                        variant.Stock = variantRequest.Stock;
                        variant.Price = variantRequest.Price;
                    }
                }
            }
        }

        var requestedMain = product.ProductColors.FirstOrDefault(c => c.IsMainColor) ?? product.ProductColors.FirstOrDefault();
        if (requestedMain is not null)
            foreach (var c in product.ProductColors) c.IsMainColor = ReferenceEquals(c, requestedMain);

        // recompute min price denormalized
        product.MinPrice = product.ProductColors
            .SelectMany(c => c.ProductVariants)
            .Select(v => v.Price)
            .DefaultIfEmpty(0m)
            .Min();

        await _uow.SaveAsync();

        foreach (var url in urlsToDelete) await _fileService.DeleteAsync(url);

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