using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PostgresAuth.Data.Mappings;

public class UserLoginMapping
    : IEntityTypeConfiguration<IdentityUserLogin<int>>
{
    public void Configure(EntityTypeBuilder<IdentityUserLogin<int>> builder)
    {
        builder.ToTable("user_logins");
        builder.HasKey(x => new { x.UserId, x.LoginProvider, x.ProviderKey });

        builder.Property(x => x.LoginProvider)
            .HasColumnName("login_provider")
            .HasColumnType("VARCHAR")
            .HasMaxLength(80)
            .IsRequired(true);

        builder.Property(x => x.ProviderKey)
            .HasColumnName("provider_key")
            .HasColumnType("VARCHAR")
            .HasMaxLength(80)
            .IsRequired(true);

        builder.Property(x => x.ProviderDisplayName)
            .HasColumnName("provider_display_name")
            .HasColumnType("VARCHAR")
            .HasMaxLength(80)
            .IsRequired(false);

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasColumnType("INT")
            .IsRequired(true);
    }
}