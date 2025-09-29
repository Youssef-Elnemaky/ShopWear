namespace ShopWear.DataAccess.Models.Products;


public class ProductImage
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = null!;

    // Navigation
    public int ProductColorId { get; set; }
    public ProductColor ProductColor { get; set; } = null!;
}