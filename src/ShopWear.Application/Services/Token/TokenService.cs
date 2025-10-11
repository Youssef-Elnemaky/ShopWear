using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ShopWear.Application.Common.Results;
using ShopWear.Application.Dtos.Requests.Auth;
using ShopWear.Application.Dtos.Responses.Auth;
using ShopWear.Application.Options;
using ShopWear.DataAccess.Interfaces.Repositories;
using ShopWear.DataAccess.Models.Auth;

namespace ShopWear.Application.Services.Token;

public sealed class TokenService : ITokenService
{
    private readonly IUnitOfWork _uow;
    private readonly JwtOptions _jwt;
    private readonly TimeProvider _clock;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly byte[] _signingKey;
    private readonly byte[] _pepper;

    public TokenService(IUnitOfWork uow, IOptions<JwtOptions> jwt, TimeProvider clock, UserManager<ApplicationUser> userManager)
    {
        _uow = uow;
        _jwt = jwt.Value;
        _clock = clock;
        _userManager = userManager;
        _signingKey = Convert.FromBase64String(_jwt.SigningKeyB64);
        _pepper = Convert.FromBase64String(_jwt.RefreshTokenPepperB64);
    }

    public async Task<Result<AuthResponse>> GenerateTokenAsync(ApplicationUser user, IList<string> roles)
    {
        var now = _clock.GetUtcNow().UtcDateTime;
        var accessExp = now.AddMinutes(_jwt.AccessTokenMinutes);
        var rtExp = now.AddDays(_jwt.RefreshTokenDays);
        var jwt = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: BuildClaims(user, roles),
            notBefore: now,
            expires: accessExp,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_signingKey), SecurityAlgorithms.HmacSha256));
        var access = new JwtSecurityTokenHandler().WriteToken(jwt);

        var rawRefreshToken = Base64Url(RandomBytes(64));
        var refreshTokenHash = Hash(rawRefreshToken);

        await _uow.RefreshTokens.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            CreatedAt = now,
            ExpiresAt = rtExp
        });
        await _uow.SaveAsync();

        return Result<AuthResponse>.Success(new AuthResponse(access, rawRefreshToken, accessExp, rtExp));
    }

    public async Task<Result<AuthResponse>> RefreshAsync(AuthRefreshRequest request)
    {
        var now = _clock.GetUtcNow().UtcDateTime;

        var stored = await _uow.RefreshTokens.GetByTokenHashAsync(Hash(request.RefreshToken));
        if (stored is null || stored.RevokedAt is not null || stored.ExpiresAt <= now)
            return TokenError.RefreshTokenInvalidOrExpired();

        var user = stored.User;

        // revoke old
        stored.RevokedAt = now;
        await _uow.RefreshTokens.UpdateAsync(stored);

        // issue new refresh
        var newRTRaw = Base64Url(RandomBytes(64));
        var newHash = Hash(newRTRaw);
        await _uow.RefreshTokens.AddAsync(new RefreshToken
        {
            UserId = stored.UserId,
            TokenHash = newHash,
            CreatedAt = now,
            ExpiresAt = now.AddDays(_jwt.RefreshTokenDays)
        });
        await _uow.SaveAsync();

        // new access
        var roles = await _userManager.GetRolesAsync(user);
        var accessExp = now.AddMinutes(_jwt.AccessTokenMinutes);
        var jwt = new JwtSecurityToken(
            _jwt.Issuer,
            _jwt.Audience,
            BuildClaims(user, roles),
            now,
            accessExp,
            new SigningCredentials(new SymmetricSecurityKey(_signingKey), SecurityAlgorithms.HmacSha256));

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);
        return new AuthResponse(accessToken, newRTRaw, accessExp, now.AddDays(_jwt.RefreshTokenDays));
    }

    private string Hash(string raw)
        => Convert.ToBase64String(HMACSHA256.HashData(_pepper, Encoding.UTF8.GetBytes(raw)));

    private static IEnumerable<Claim> BuildClaims(ApplicationUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim()),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        foreach (var r in roles) claims.Add(new Claim(ClaimTypes.Role, r));
        return claims;
    }

    /*
    To make the token URL-safe and cookie/header-safe.
    Standard Base64 uses +, /, and = padding. These break URLs and some parsers.
    Base64URL replaces + with -, / with _, and removes trailing = per RFC 7515/7519 (JWT/JWS).
    Result can be sent in query strings, headers, and bodies without extra encoding.
    */
    private static string Base64Url(byte[] b) => Convert.ToBase64String(b).Replace('+', '-').Replace('/', '_').TrimEnd('=');
    private static byte[] RandomBytes(int n)
    {
        var randomBytes = new byte[n];
        RandomNumberGenerator.Fill(randomBytes);
        return randomBytes;
    }
}