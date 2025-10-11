namespace ShopWear.DataAccess.Models.Auth;

public class RefreshToken
{
    public int Id { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => RevokedAt is null && !IsExpired;

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
}