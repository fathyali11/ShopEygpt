namespace Web.DataAccess.Data.EntitiesConfigurations;
internal class WishlistItemConfiguration : IEntityTypeConfiguration<WishlistItem>
{
    public void Configure(EntityTypeBuilder<WishlistItem> builder)
    {
        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.ProductName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ci => ci.ImageName)
            .HasMaxLength(100);

        builder.Property(ci => ci.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
    }
}

internal class ProductRatingConfiguration : IEntityTypeConfiguration<ProductRating>
{
    public void Configure(EntityTypeBuilder<ProductRating> builder)
    {
        builder.HasKey(p => new {p.ProductId,p.UserId});

        builder.Property(p => p.Rating)
            .IsRequired();
    }
}