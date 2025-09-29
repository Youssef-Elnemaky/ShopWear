namespace ShopWear.DataAccess.Models.Products;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    // Navigation
    public int CategoryId { get; set; }
    public ProductCategory Category { get; set; } = null!;

    public ICollection<ProductColor> ProductColors { get; set; } = [];
}