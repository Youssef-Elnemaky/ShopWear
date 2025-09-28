using ShopWear.DataAccess.Enums;

namespace ShopWear.DataAccess.Models.Product;


public class ProductVariant
{
    public int Id { get; set; }
    public ProductSize Size { get; set; }
    public int Sku { get; set; }
    public decimal Price { get; set; }

    // Navigation
    public int ProductColorId { get; set; }
    public ProductColor ProductColor { get; set; } = null!;
}