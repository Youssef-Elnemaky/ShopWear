namespace ShopWear.DataAccess.Models.Product;


public class ProductCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    // Navigation
    public ICollection<Product> Products { get; set; } = [];
}