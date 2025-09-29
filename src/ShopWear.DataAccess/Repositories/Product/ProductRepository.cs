using ShopWear.DataAccess.Data;
using ShopWear.DataAccess.Interfaces.Repositories;
using ShopWear.DataAccess.Interfaces.Repositories.Products;
using ShopWear.DataAccess.Models.Products;

namespace ShopWear.DataAccess.Repositories.Products;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly AppDbContext _db;

    public ProductRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }
}