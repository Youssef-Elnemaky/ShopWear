using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopWear.DataAccess.Models.Product;

namespace ShopWear.DataAccess.Data.Config.ProductConfig;

public class ProductCategoryConfig : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        // Id
        builder.HasKey(pc => pc.Id);
        builder.Property(pc => pc.Id).UseIdentityColumn();

        // Name
        builder.Property(pc => pc.Name).HasMaxLength(100);

        // Description
        builder.Property(pc => pc.Description).HasMaxLength(1000);
    }
}