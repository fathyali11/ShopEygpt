namespace Web.DataAccess.Data.EntitiesConfigurations;

internal class ProductRatingConfiguration : IEntityTypeConfiguration<ProductRating>
{
    public void Configure(EntityTypeBuilder<ProductRating> builder)
    {
        builder.HasKey(p => new {p.ProductId,p.UserId});

        builder.Property(p => p.Rating)
            .IsRequired();
    }
}
