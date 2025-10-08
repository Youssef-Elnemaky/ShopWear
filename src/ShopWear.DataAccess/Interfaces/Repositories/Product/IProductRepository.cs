using ShopWear.DataAccess.Models.Products;

namespace ShopWear.DataAccess.Interfaces.Repositories.Products;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByIdWithDetailsAsync(int id, bool asNoTracking = true);
    Task<Product?> GetByProductIdAndColorIdAsync(int productId, int colorId, bool asNoTracking = true);
    Task<(IReadOnlyList<Product> Items, int Total)> GetPagedAsync(ProductListParams query);

}