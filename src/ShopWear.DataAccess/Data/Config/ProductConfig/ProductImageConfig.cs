using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopWear.DataAccess.Models.Products;

namespace ShopWear.DataAccess.Data.Config.ProductConfig;

public class ProductImageConfig : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        // Id
        builder.HasKey(pi => pi.Id);
        builder.Property(pi => pi.Id)
               .ValueGeneratedOnAdd()
               .HasDefaultValueSql("NEWSEQUENTIALID()");

        // ImageUrl
        builder.Property(pi => pi.ImageUrl).HasMaxLength(500);

        // ProductColor Relationship
        builder.HasOne(pi => pi.ProductColor)
               .WithMany(pc => pc.ProductImages)
               .HasForeignKey(pi => pi.ProductColorId);
    }
}