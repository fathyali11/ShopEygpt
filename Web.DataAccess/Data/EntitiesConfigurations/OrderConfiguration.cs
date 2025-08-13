namespace Web.DataAccess.Data.EntitiesConfigurations;
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
