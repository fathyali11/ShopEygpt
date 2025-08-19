namespace Web.DataAccess.Data.EntitiesConfigurations;

internal class UserRecommendationConfiguration : IEntityTypeConfiguration<UserRecommendation>
{
    public void Configure(EntityTypeBuilder<UserRecommendation> builder)
    {
        builder.HasKey(p => new { p.ProductId, p.UserId });

        builder.Property(p => p.Score)
            .IsRequired();

        builder.HasOne(x=>x.Product)
            .WithMany(x=>x.UserRecommendations)
            .HasForeignKey(x=>x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserRecommendations)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}