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

    public async Task<Product?> GetByIdWithDetailsAsync(int id, bool asNoTracking = true)
    {
        //another way is using projection by using new ProductDetailResponse but in that case:
        //we will need to have a shared project for contracts between application and data access since 
        //data access cannot access application layer directly
        //***************maybe consider using split queries and check if boosts performance later on.***************
        IQueryable<Product> query = asNoTracking ? _db.Products.AsNoTracking() : _db.Products;

        query = query.Include(p => p.Category)
                    .Include(p => p.ProductColors).ThenInclude(c => c.ProductImages)
                    .Include(p => p.ProductColors).ThenInclude(c => c.ProductVariants);

        return await query.FirstOrDefaultAsync(p => p.Id == id);
    }
}