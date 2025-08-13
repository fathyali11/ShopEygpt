namespace Web.DataAccess.Data.EntitiesConfigurations;
internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.ImageName)
            .HasMaxLength(100);

        builder.HasIndex(c => c.Name).IsUnique();

    }
}