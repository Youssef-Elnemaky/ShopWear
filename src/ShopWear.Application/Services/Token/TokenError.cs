using ShopWear.Application.Common.Errors;

namespace ShopWear.Application.Services.Token;


public static class TokenError
{
    public static Error RefreshTokenInvalidOrExpired()
        => Error.Validation("Token.RefreshToken.Invalid", "Invalid or expired refresh token.");
}