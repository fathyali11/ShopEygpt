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
