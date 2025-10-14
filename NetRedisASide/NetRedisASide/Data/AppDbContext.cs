using NetRedisASide.Models;
using Microsoft.EntityFrameworkCore;

namespace NetRedisASide.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options): DbContext(options)
    {               
        public DbSet<Assunto> Assuntos { get; set; } =  null!;
        public DbSet<Movimentacao> Movimentacoes { get; set; } = null!;
        public DbSet<TipoDocumento> TiposDocumentos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          modelBuilder.ApplyConfigurationsFromAssembly(typeof(Assunto).Assembly);
          modelBuilder.ApplyConfigurationsFromAssembly(typeof(Movimentacao).Assembly);
          modelBuilder.ApplyConfigurationsFromAssembly(typeof(TipoDocumento).Assembly);
        }
    }


}