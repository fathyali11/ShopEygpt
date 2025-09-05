namespace Web.DataAccess.Data.EntitiesConfigurations;
internal class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.HasMany(c => c.WishlistItems)
            .WithOne(c => c.Wishlist)
            .HasForeignKey(p => p.WishlistId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
