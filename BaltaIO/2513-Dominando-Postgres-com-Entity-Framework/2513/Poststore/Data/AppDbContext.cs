// docker run --name postgres -e POSTGRES_PASSWORD=1q2w3e4r@#$ -p 5432:5432 -d postgres

using Microsoft.EntityFrameworkCore;
using Poststore.Models;

namespace Poststore.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Program).Assembly);
    }
}