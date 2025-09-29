using ShopWear.DataAccess.Data;
using ShopWear.DataAccess.Interfaces.Repositories;
using ShopWear.DataAccess.Interfaces.Repositories.Products;
using ShopWear.DataAccess.Models.Products;

namespace ShopWear.DataAccess.Repositories.Products;

public class ProductVariantRepository : Repository<ProductVariant>, IProductVariantRepository
{
    private readonly AppDbContext _db;

    public ProductVariantRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }
}