using ShopWear.DataAccess.Data;
using ShopWear.DataAccess.Interfaces.Repositories.Products;
using ShopWear.DataAccess.Models.Products;

namespace ShopWear.DataAccess.Repositories.Products;

public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
{
    private readonly AppDbContext _db;

    public ProductImageRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }
}