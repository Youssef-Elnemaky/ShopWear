using ShopWear.DataAccess.Data;
using ShopWear.DataAccess.Interfaces.Repositories;
using ShopWear.DataAccess.Interfaces.Repositories.Products;
using ShopWear.DataAccess.Models.Products;
using ShopWear.DataAccess.Repositories.Products;

namespace ShopWear.DataAccess.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    public IProductCategoryRepository ProductCategories { get; }

    public IProductRepository Products { get; }

    public IProductColorRepository ProductColors { get; }

    public IProductImageRepository ProductImages { get; }

    public IProductVariantRepository ProductVariants { get; }
    public UnitOfWork(AppDbContext db)
    {
        _db = db;
        ProductCategories = new ProductCategoryRepository(_db);
        Products = new ProductRepository(_db);
        ProductColors = new ProductColorRepository(_db);
        ProductImages = new ProductImageRepository(_db);
        ProductVariants = new ProductVariantRepository(_db);
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
    {
        return await _db.SaveChangesAsync(cancellationToken);
    }
}