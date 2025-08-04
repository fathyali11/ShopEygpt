using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Web.DataAccess.Data.EntitiesConfigurations;

internal class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(c=>c.TotalPrice)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0.0m);

        builder.HasMany(c =>c.CartItems )
            .WithOne(c => c.Cart)
            .HasForeignKey(p => p.CartId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
internal class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(c => c.StripeSessionId)
            .IsRequired();

        builder.Property(c => c.PaymentIntentId)
            .IsRequired();

        builder.Property(c => c.Status)
            .IsRequired();

        builder.Property(c => c.TotalPrice)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0.0m);

        builder.HasMany(c => c.OrderItems)
            .WithOne(c => c.Order)
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
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
