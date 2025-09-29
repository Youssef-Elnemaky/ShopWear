namespace ShopWear.DataAccess.Models.Products;


public class ProductCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    // Navigation
    public ICollection<Product> Products { get; set; } = [];
}