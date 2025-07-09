using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web.Entites.Models;

namespace Web.Entites.EntityConfigurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(30);

            builder.Property(c => c.ImageName)
                   .IsRequired();

            builder.Property(c => c.CreatedAt)
                   .IsRequired();
        }
    }
}