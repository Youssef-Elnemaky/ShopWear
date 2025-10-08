namespace ShopWear.DataAccess.Models.Auth;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Revoked { get; set; }

    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsActive => Revoked is null && !IsExpired;

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
}