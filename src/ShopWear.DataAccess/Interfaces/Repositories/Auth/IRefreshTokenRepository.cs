using ShopWear.DataAccess.Models.Auth;

namespace ShopWear.DataAccess.Interfaces.Repositories.Auth;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token);
}