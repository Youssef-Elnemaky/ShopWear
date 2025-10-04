using ShopWear.Application.Common.Errors;
using ShopWear.DataAccess.Enums.ProductEnums;

namespace ShopWear.Application.Services.Products;

public static class ProductError
{
    public static Error CategoryNotFound(int categoryId)
        => Error.NotFound("Product.CategoryId.NotFound", $"Category with id: {categoryId} not found.");

    public static Error ProductNotFound(int productId)
        => Error.NotFound("Product.Product.NotFound", $"Product with id: {productId} not found.");
    public static Error ProductNameRequired()
        => Error.Validation("Product.Name.Empty", "Product name cannot be empty or white spaces.");

    public static Error ProductNameMaxLength()
        => Error.Validation("Product.Name.Length", "Product cannot be more than 200 characters.");

    public static Error ProductColorsCount()
        => Error.Validation("Product.Colors.Count", "Product must have (min:1)-(max:5) color/s");

    // color errors
    public static Error ProductColorRequired()
        => Error.Validation("Product.Colors.Color.Required", "Color cannot be empty or white spaces.");
    public static Error ProductColorLength()
        => Error.Validation("Product.Colors.Color.Length", "Color cannot exceed 50 characters.");

    public static Error ProductVariantsCount()
        => Error.Validation("Product.Colors.Variant.Count", "Color must have (min:1)-(max:6) variant/s");

    public static Error ProductMultipleMainColor()
        => Error.Validation("Product.Colors.IsMainColor.Multiple", "Only 1 main color is allowed");

    public static Error ProductNoMainColor()
        => Error.Validation("Product.Colors.IsMainColor.NoMainColor", "A single main color must be selected.");

    public static Error ProductColorConflict(string color)
        => Error.Conflict(
            "Product.Colors.Color.Unique",
            $"Color with value {color} was already supplied. All color values must be unique");
    // variant errors
    public static Error ProductVariantSizeInvalid(int rawSize)
    {
        var allowed = string.Join(", ", Enum.GetNames(typeof(ProductSize)));
        return Error.Validation(
            "Product.Colors.Color.Variants.Size.Invalid",
            $"Invalid variant size value: {rawSize}. Allowed values: {allowed}"
        );
    }

    public static Error ProductVariantStockInvalid()
        => Error.Validation("Product.Colors.Color.Variants.Stock", "Variant Stock must be at least 1.");
    public static Error ProductVariantPriceInvalid()
        => Error.Validation("Product.Color.Color.Variants.Price", "Variant price cannot be 0");
    public static Error ProductVariantConflict(string variantSize)
=> Error.Conflict(
    "Product.Colors.Color.Variant.Size.Unique",
    $"Variant size with value {variantSize} was already supplied. All variant sizes values must be unique");
}