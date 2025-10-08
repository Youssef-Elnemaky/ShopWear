using ShopWear.DataAccess.Interfaces.Repositories.Auth;
using ShopWear.DataAccess.Interfaces.Repositories.Products;

namespace ShopWear.DataAccess.Interfaces.Repositories;

public interface IUnitOfWork
{
    IProductCategoryRepository ProductCategories { get; }
    IProductRepository Products { get; }
    IProductColorRepository ProductColors { get; }
    IProductImageRepository ProductImages { get; }
    IProductVariantRepository ProductVariants { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    Task<int> SaveAsync(CancellationToken cancellationToken = default);
}