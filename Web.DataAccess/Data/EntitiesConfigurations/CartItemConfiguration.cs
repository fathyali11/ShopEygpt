using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Web.DataAccess.Data.EntitiesConfigurations;

internal class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.ProductName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ci => ci.ImageName)
            .HasMaxLength(100);

        builder.Property(ci => ci.Count)
            .IsRequired();

        builder.Property(ci => ci.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Ignore(ci => ci.TotalPrice); 


    }
}