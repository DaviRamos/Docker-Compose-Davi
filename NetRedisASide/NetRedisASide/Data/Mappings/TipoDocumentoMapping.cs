using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetRedisASide.Models;

namespace NetRedisASide.Data.Mappings
{
    public class TipoDocumentoMapping : IEntityTypeConfiguration<TipoDocumento>
    {
        public void Configure(EntityTypeBuilder<TipoDocumento> builder)
        {
            builder.ToTable("TipoDocumentos");

            builder.HasKey(c => c.Id).HasName("PK_TipoDocumentos");

            builder.Property(c => c.Id)
                .HasColumnName("Id")
                .IsRequired().UseIdentityAlwaysColumn().UseSerialColumn();

            builder.Property(c => c.Descricao)
                .HasColumnName("Descricao")
                .IsRequired()
                .HasColumnType("varchar")
                .HasMaxLength(160);

            builder.Property(c => c.Nome)
                .HasColumnName("Nome")
                .IsRequired()
                .HasColumnType("varchar")
                .HasMaxLength(250);        

            builder.HasIndex(c => c.Nome).IsUnique().HasDatabaseName("IX_Assuntos_Nome");
            builder.Property(c => c.DataCriacao)
                .HasColumnName("DataCriacao")
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamptz")   
                .IsRequired();
        }
    }
}