using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PostgresAuth.Data.Mappings;

public class UserRoleMapping
    : IEntityTypeConfiguration<IdentityUserRole<int>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<int>> builder)
    {
        builder.ToTable("user_roles");
        builder.HasKey(r => new { r.UserId, r.RoleId });

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasColumnType("INT")
            .IsRequired(true);

        builder.Property(x => x.RoleId)
            .HasColumnName("role_id")
            .HasColumnType("INT")
            .IsRequired(true);
    }
}