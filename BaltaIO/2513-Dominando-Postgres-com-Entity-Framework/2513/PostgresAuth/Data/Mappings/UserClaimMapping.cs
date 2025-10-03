using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PostgresAuth.Data.Mappings;

public class UserClaimMapping
    : IEntityTypeConfiguration<IdentityUserClaim<int>>
{
    public void Configure(EntityTypeBuilder<IdentityUserClaim<int>> builder)
    {
        builder.ToTable("user_claims");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("INT")
            .IsRequired(true);

        builder.Property(x => x.ClaimType)
            .HasColumnName("claim_type")
            .HasColumnType("VARCHAR")
            .HasMaxLength(256)
            .IsRequired(true);

        builder.Property(x => x.ClaimValue)
            .HasColumnName("claim_value")
            .HasColumnType("VARCHAR")
            .HasMaxLength(1024)
            .IsRequired(false);

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasColumnType("INT")
            .IsRequired(true);
    }
}