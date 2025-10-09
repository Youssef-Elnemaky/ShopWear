using Microsoft.AspNetCore.Identity;

namespace ShopWear.DataAccess.Models.Auth;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}