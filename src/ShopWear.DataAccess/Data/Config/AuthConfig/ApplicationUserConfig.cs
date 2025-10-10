using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopWear.DataAccess.Models.Auth;

namespace ShopWear.DataAccess.Data.Config.AuthConfig;

public class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FirstName).HasColumnType("VARCHAR(30)");
        builder.Property(u => u.LastName).HasColumnType("VARCHAR(30)");
        builder.HasIndex(u => u.Email).IsUnique();
    }
}