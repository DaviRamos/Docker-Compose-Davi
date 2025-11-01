using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PostgresAuth.Data.Mappings;

public class UserMapping : IEntityTypeConfiguration<IdentityUser>
{
    public void Configure(EntityTypeBuilder<IdentityUser> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .UseIdentityAlwaysColumn();

        builder.Property(x => x.AccessFailedCount)
            .HasColumnName("access_failed_count");

        builder.Property(x => x.ConcurrencyStamp)
            .HasColumnName("concurrency_stamp")
            .HasColumnType("VARCHAR")
            .HasMaxLength(36)
            .IsRequired(true);

        builder.Property(x => x.Email)
            .HasColumnName("email")
            .HasColumnType("VARCHAR")
            .HasMaxLength(160)
            .IsRequired(true);

        builder.Property(x => x.EmailConfirmed)
            .HasColumnName("email_confirmed")
            .HasColumnType("BOOLEAN")
            .IsRequired(true);

        builder.Property(x => x.LockoutEnabled)
            .HasColumnName("lockout_enabled")
            .HasColumnType("BOOLEAN")
            .IsRequired(true);

        builder.Property(x => x.LockoutEnd)
            .HasColumnName("lockout_end")
            .HasColumnType("TIMESTAMPTZ").IsRequired(false);

        builder.Property(x => x.NormalizedEmail)
            .HasColumnName("normalized_email")
            .HasColumnType("VARCHAR")
            .HasMaxLength(160)
            .IsRequired(true);

        builder.Property(x => x.NormalizedUserName)
            .HasColumnName("normalized_username")
            .HasColumnType("VARCHAR")
            .HasMaxLength(80)
            .IsRequired(true);

        builder.Property(x => x.PasswordHash)
            .HasColumnName("password_hash")
            .HasColumnType("VARCHAR")
            .HasMaxLength(256)
            .IsRequired(true);

        builder.Property(x => x.PhoneNumber)
            .HasColumnName("phone_number")
            .HasColumnType("VARCHAR")
            .HasMaxLength(30)
            .IsRequired(false);

        builder.Property(x => x.PhoneNumberConfirmed)
            .HasColumnName("phone_number_confirmed")
            .HasColumnType("BOOLEAN")
            .IsRequired(true);

        builder.Property(x => x.SecurityStamp)
            .HasColumnName("security_stamp")
            .HasColumnType("VARCHAR")
            .HasMaxLength(36)
            .IsRequired(true);

        builder.Property(x => x.TwoFactorEnabled)
            .HasColumnName("two_factor_enabled")
            .HasColumnType("BOOLEAN")
            .IsRequired(true);

        builder.Property(x => x.UserName)
            .HasColumnName("username")
            .HasColumnType("VARCHAR")
            .HasMaxLength(80)
            .IsRequired(true);

        builder.HasMany<IdentityUserClaim<int>>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();
        builder.HasMany<IdentityUserLogin<int>>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();
        builder.HasMany<IdentityUserToken<int>>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();
        builder.HasMany<IdentityUserRole<int>>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
    }
}