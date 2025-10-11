using Microsoft.EntityFrameworkCore;
using ShopWear.DataAccess.Data;
using ShopWear.DataAccess.Interfaces.Repositories.Auth;
using ShopWear.DataAccess.Models.Auth;

namespace ShopWear.DataAccess.Repositories.Auth;

public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    private readonly AppDbContext _db;

    public RefreshTokenRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
    {
        var refreshToken = await _db.RefreshTokens
            .AsNoTracking()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

        return refreshToken;
    }
}