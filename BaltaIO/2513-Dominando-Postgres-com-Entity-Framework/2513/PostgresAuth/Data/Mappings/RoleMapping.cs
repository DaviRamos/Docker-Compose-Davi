using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PostgresAuth.Data.Mappings;

public class RoleMapping
    : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .UseIdentityAlwaysColumn();

        builder.Property(x => x.ConcurrencyStamp)
            .HasColumnName("concurrency_stamp")
            .HasColumnType("VARCHAR")
            .HasMaxLength(36)
            .IsRequired(true);

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType("VARCHAR")
            .HasMaxLength(80)
            .IsRequired(true);

        builder.Property(x => x.NormalizedName)
            .HasColumnName("normalized_name")
            .HasColumnType("VARCHAR")
            .HasMaxLength(80)
            .IsRequired(true);
        
        builder.HasIndex(r => r.NormalizedName).IsUnique();
        builder.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();
        builder.Property(u => u.Name).HasMaxLength(256);
        builder.Property(u => u.NormalizedName).HasMaxLength(256);
    }
}