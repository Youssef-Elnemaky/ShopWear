using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopWear.DataAccess.Models.Auth;

namespace ShopWear.DataAccess.Data.Config.AuthConfig;

public class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        //setting the key
        builder.HasKey(rt => rt.Id);

        // setting the max length of the token
        builder.Property(rt => rt.TokenHash).HasMaxLength(128).HasColumnType("VARCHAR(128)");
        builder.HasIndex(rt => rt.TokenHash).IsUnique();

        // setting the relation between the refresh tokens and the user
        builder.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId);
    }
}