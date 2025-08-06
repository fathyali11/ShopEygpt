using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Web.DataAccess.Data.EntitiesConfigurations;

internal class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.ProductName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.ImageName)
            .HasMaxLength(100);

        builder.Property(c => c.UnitPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.Quantity)
            .IsRequired();

    }
}
