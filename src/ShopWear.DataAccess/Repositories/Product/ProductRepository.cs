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

    public async Task<Product?> GetByProductIdAndColorIdAsync(int productId, int colorId, bool asNoTracking = true)
    {
        IQueryable<Product> query = asNoTracking ? _db.Products.AsNoTracking() : _db.Products;
        query = query.Include(p => p.ProductColors.Where(c => c.Id == colorId)).ThenInclude(c => c.ProductImages);

        return await query.FirstOrDefaultAsync(p => p.Id == productId);
    }

    public async Task<(IReadOnlyList<Product> Items, int Total)> GetPagedAsync(ProductListParams queryParams)
    {
        IQueryable<Product> query = _db.Products;
        if (!string.IsNullOrWhiteSpace(queryParams.Search)) query = query.Where(p => p.Name.Contains(queryParams.Search));
        if (queryParams.CategoryId.HasValue) query = query.Where(p => p.CategoryId == queryParams.CategoryId.Value);

        query = (queryParams.SortBy.ToLowerInvariant(), queryParams.Desc) switch
        {
            ("price", false) => query = query.OrderBy(p => p.MinPrice),
            ("price", true) => query = query.OrderByDescending(p => p.MinPrice),
            ("name", false) => query = query.OrderBy(p => p.Name),
            _ => query = query.OrderByDescending(p => p.Name)
            // ("name", true) => query = query.OrderByDescending(p => p.Name)
        };

        var total = await query.CountAsync();

        query = query.Include(p => p.Category)
                     .Include(p => p.ProductColors.Where(c => c.IsMainColor))
                        .ThenInclude(c => c.ProductImages);

        var skip = Math.Max(0, (queryParams.Page - 1) * queryParams.PageSize);

        var products = await query.Skip(skip).Take(queryParams.PageSize).ToListAsync();
        return (products, total);
    }
}