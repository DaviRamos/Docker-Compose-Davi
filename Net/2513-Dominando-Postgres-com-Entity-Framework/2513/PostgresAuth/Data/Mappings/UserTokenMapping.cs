using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PostgresAuth.Data.Mappings;

public class UserTokenMapping
    : IEntityTypeConfiguration<IdentityUserToken<int>>
{
    public void Configure(EntityTypeBuilder<IdentityUserToken<int>> builder)
    {
        builder.ToTable("user_tokens");
        builder.HasKey(x => new { x.UserId, x.LoginProvider, x.Name });
        
        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasColumnType("INT")
            .IsRequired(true);
        
        builder.Property(x => x.LoginProvider)
            .HasColumnName("login_provider")
            .HasColumnType("VARCHAR")
            .HasMaxLength(80)
            .IsRequired(true);
        
        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType("VARCHAR")
            .HasMaxLength(80)
            .IsRequired(true);
        
        builder.Property(x => x.Value)
            .HasColumnName("value")
            .HasColumnType("VARCHAR")
            .HasMaxLength(512)
            .IsRequired(false);
    }
}