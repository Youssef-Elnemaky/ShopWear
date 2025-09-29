using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopWear.DataAccess.Models.Product;

namespace ShopWear.DataAccess.Data.Config.ProductConfig;

public class ProductVariantConfig : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        // Key
        builder.HasKey(pv => pv.Id);
        builder.Property(pv => pv.Id).UseIdentityColumn();

        // Size
        builder.Property(pv => pv.Size).HasConversion<string>();

        // Price
        builder.Property(pv => pv.Price).HasPrecision(18, 2);

        // ProductColor Relationship
        builder.HasOne(pv => pv.ProductColor)
               .WithMany(pc => pc.ProductVariants)
               .HasForeignKey(pv => pv.ProductColorId);
    }
}