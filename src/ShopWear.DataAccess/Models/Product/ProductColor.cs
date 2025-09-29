namespace ShopWear.DataAccess.Models.Products;

public class ProductColor
{
    public int Id { get; set; }
    public string Color { get; set; } = null!;

    // Navigation
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public ICollection<ProductVariant> ProductVariants { get; set; } = [];
    public ICollection<ProductImage> ProductImages { get; set; } = [];
}