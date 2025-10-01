using EfPostgresStore.Models;
using Microsoft.EntityFrameworkCore;

namespace EfPostgresStore.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options): DbContext(options)
    {               
        public DbSet<Product> Products { get; set; } =  null!;
        public DbSet<Category> Categories { get; set; } = null!; 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Product).Assembly);
        }
    }


}