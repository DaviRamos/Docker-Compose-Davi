using NetRedisASide.Models;
using Microsoft.EntityFrameworkCore;

namespace NetRedisASide.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options): DbContext(options)
    {               
        public DbSet<Assunto> Assuntos { get; set; } =  null!;
        public DbSet<Movimentacao> Movimentacoes { get; set; } = null!;
        public DbSet<TipoDocumento> TiposDocumento { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          // Configurações adicionais do modelo podem ser feitas aqui
        }
    }


}