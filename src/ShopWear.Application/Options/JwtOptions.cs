namespace ShopWear.Application.Options;

public sealed class JwtOptions
{
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public string SigningKeyB64 { get; init; } = default!;
    public int AccessTokenMinutes { get; init; } = 10;
    public int RefreshTokenDays { get; init; } = 14;
    public string RefreshTokenPepperB64 { get; init; } = default!;
}