using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EfPostgresStore.Models;

namespace EfPostgresStore.Data.Mappings
{
    public class ProductMapping : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products   ");

            builder.HasKey(c => c.Id).HasName("PK_Products");

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

            builder.HasIndex(c => c.Slug).IsUnique().HasDatabaseName("IX_Products_Slug");

            builder.Property(c => c.CreatedAtUTc)
                .HasColumnName("CreatedAt")
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamptz")   
                .IsRequired();

            builder.Property(c => c.UpdatedAtUTc)
                .HasColumnName("UpdatedAt")
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamptz")
                .IsRequired();

            builder.Property(c => c.IsActive)
                .HasColumnName("IsActive")
                .IsRequired();

            builder.Property(c => c.DefaultLanguage)
                .HasColumnName("DefaultLanguage")                
                .HasMaxLength(8)
                .HasDefaultValue("en-us")
                .IsRequired();
        }
    }
}