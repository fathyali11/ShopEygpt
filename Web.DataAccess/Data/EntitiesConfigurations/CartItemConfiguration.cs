using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Web.DataAccess.Data.EntitiesConfigurations;

internal class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.ProductId)
            .IsRequired();

        builder.Property(c => c.CartId)
           .IsRequired();

    }
}