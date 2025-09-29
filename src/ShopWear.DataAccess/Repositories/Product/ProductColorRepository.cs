using ShopWear.DataAccess.Data;
using ShopWear.DataAccess.Interfaces.Repositories.Products;
using ShopWear.DataAccess.Models.Products;

namespace ShopWear.DataAccess.Repositories.Products;

public class ProductColorRepository : Repository<ProductColor>, IProductColorRepository
{
    private readonly AppDbContext _db;

    public ProductColorRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }
}