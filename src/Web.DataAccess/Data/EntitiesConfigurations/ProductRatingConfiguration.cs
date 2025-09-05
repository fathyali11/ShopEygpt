namespace Web.DataAccess.Data.EntitiesConfigurations;

internal class ProductRatingConfiguration : IEntityTypeConfiguration<ProductRating>
{
    public void Configure(EntityTypeBuilder<ProductRating> builder)
    {
        builder.HasKey(p => new {p.ProductId,p.UserId});

        builder.Property(p => p.Rating)
            .IsRequired();

        builder.HasOne(x=>x.Product)
            .WithMany(x=>x.ProductRatings)
            .HasForeignKey(x=>x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x=>x.User)
            .WithMany(x=>x.ProductRatings)
            .HasForeignKey(x=>x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
