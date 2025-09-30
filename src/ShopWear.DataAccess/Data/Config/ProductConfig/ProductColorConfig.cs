using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopWear.DataAccess.Models.Products;

namespace ShopWear.DataAccess.Data.Config.ProductConfig;

public class ProductColorConfig : IEntityTypeConfiguration<ProductColor>
{
    public void Configure(EntityTypeBuilder<ProductColor> builder)
    {
        // Id
        builder.HasKey(pc => pc.Id);
        builder.Property(pc => pc.Id).UseIdentityColumn();

        // Color
        builder.Property(pc => pc.Color).HasMaxLength(50);

        // Product Relationship
        builder.HasOne(pc => pc.Product)
               .WithMany(p => p.ProductColors)
               .HasForeignKey(pc => pc.ProductId);
    }
}