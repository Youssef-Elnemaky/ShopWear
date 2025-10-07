namespace ShopWear.Application.Dtos.Responses.Products;

public record class ProductSummaryResponse(
    int Id,
    string Name,
    string Category,
    int CategoryId,
    decimal MinPrice,
    string? TopImageUrl
);