namespace ShopWear.Application.Dtos.Responses.Products;

public record class ProductSummaryResponse(
    int Id,
    string Name,
    decimal MinPrice,
    string? TopImageUrl
);