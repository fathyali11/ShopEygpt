namespace Web.DataAccess.Data.EntitiesConfigurations;

internal class UserRecommendationConfiguration : IEntityTypeConfiguration<UserRecommendation>
{
    public void Configure(EntityTypeBuilder<UserRecommendation> builder)
    {
        builder.HasKey(p => new { p.ProductId, p.UserId });

        builder.Property(p => p.Score)
            .IsRequired();
    }
}