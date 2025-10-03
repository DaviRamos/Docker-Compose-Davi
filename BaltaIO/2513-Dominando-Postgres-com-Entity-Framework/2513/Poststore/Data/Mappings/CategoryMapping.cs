using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poststore.Models;

namespace Poststore.Data.Mappings;

public class CategoryMapping : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        builder
            .HasKey(x => x.Id)
            .HasName("pk_category");
        
        builder
            .Property(x => x.Id)
            .HasColumnName("id")
            .UseIdentityAlwaysColumn()
            .UseSerialColumn();

        builder.OwnsOne(x => x.Heading, heading =>
        {
            heading.Property(x => x.Title)
                .HasColumnName("title")
                .HasColumnType("varchar")
                .HasMaxLength(160)
                .IsRequired(true);

            heading.Property(x => x.Slug)
                .HasColumnName("slug")
                .HasColumnType("varchar")
                .HasMaxLength(160)
                .IsRequired(true);
            
            heading.HasIndex(x => x.Slug)
                .IsUnique()
                .HasDatabaseName("idx_categories_slug");
        });

        builder.HasMany(x => x.Products);
    }
}