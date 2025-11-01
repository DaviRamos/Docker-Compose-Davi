using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poststore.Models;

namespace Poststore.Data.Mappings;

public class ProductMapping : IEntityTypeConfiguration<Product>
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder
            .HasKey(x => x.Id)
            .HasName("pk_product");

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
                .HasDatabaseName("idx_products_slug");
        });

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasDefaultValueSql("now()")
            .IsRequired(true);

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasDefaultValueSql("now()")
            .IsRequired(true);

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired(true);

        builder.Property(e => e.DefaultLanguage)
            .HasColumnName("default_language")
            .HasMaxLength(8)
            .HasDefaultValue("en-us")
            .IsRequired();

        builder.Property(x => x.Translations)
            .HasColumnName("translations")
            .HasColumnType("jsonb")
            .HasConversion(
                x => JsonSerializer.Serialize(x, _jsonOptions),
                x => string.IsNullOrEmpty(x)
                    ? new List<Translation>()
                    : JsonSerializer.Deserialize<List<Translation>>(x, _jsonOptions)
                      ?? new List<Translation>()
            );

        builder
            .Property(x => x.CategoryId)
            .HasColumnName("category_id");

        builder.HasOne(x => x.Category);
    }
}