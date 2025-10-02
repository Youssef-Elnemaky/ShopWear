using Microsoft.EntityFrameworkCore;
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

    public async Task<Product?> GetByIdWithDetailsAsync(int id)
    {
        //another way is using projection by using new ProductDetailResponse but in that case:
        //we will need to have a shared project for contracts between application and data access since 
        //data access cannot access application layer directly
        //***************maybe consider using split queries and check if boosts performance later on.***************
        var product = await _db.Products.Include(p => p.Category)
                    .Include(p => p.ProductColors).ThenInclude(c => c.ProductImages)
                    .Include(p => p.ProductColors).ThenInclude(c => c.ProductVariants)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == id);
        return product;
    }
}