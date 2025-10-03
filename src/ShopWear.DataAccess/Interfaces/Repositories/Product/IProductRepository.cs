using ShopWear.DataAccess.Models.Products;

namespace ShopWear.DataAccess.Interfaces.Repositories.Products;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByIdWithDetailsAsync(int id, bool asNoTracking = true);
}