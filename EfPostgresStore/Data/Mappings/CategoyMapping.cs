using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EfPostgresStore.Models;

namespace EfPostgresStore.Data.Mappings
{
    public class CategoryMapping : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.Id).HasName("PK_Categories");

            builder.Property(c => c.Id)
                .HasColumnName("Id")
                .IsRequired().UseIdentityAlwaysColumn().UseSerialColumn();

            builder.Property(c => c.Title)
                .HasColumnName("Title")
                .IsRequired()
                .HasColumnType("varchar")
                .HasMaxLength(160);

            builder.Property(c => c.Slug)
                .HasColumnName("Slug")
                .IsRequired()
                .HasColumnType("varchar")
                .HasMaxLength(160);

            builder.HasMany(c => c.Products);

            builder.HasIndex(c => c.Slug).IsUnique().HasDatabaseName("IX_Categories_Slug");            
        
        }
    }
}