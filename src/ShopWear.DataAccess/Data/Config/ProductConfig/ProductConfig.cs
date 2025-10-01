using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using ShopWear.DataAccess.Models.Products;

namespace ShopWear.DataAccess.Data.Config.ProductConfig;

public class ProductConfig : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Id
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).UseIdentityColumn();

        // Name
        builder.Property(p => p.Name).HasMaxLength(200);

        // Description
        builder.Property(p => p.Description).HasMaxLength(4000);

        // Category Relationship
        builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

    }
}