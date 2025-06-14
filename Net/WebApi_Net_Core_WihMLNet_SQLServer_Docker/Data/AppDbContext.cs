using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using projetoTeste.Models;

namespace projetoTeste.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ModelInput>()
              .HasData(
                  new ModelInput
                  {
                      Id = 1,
                      Longitude = 2f,
                      Latitude = 3f,
                      Housing_median_age = 4f,
                      Total_rooms = 4f,
                      Total_bedrooms = 3f,
                      Population = 10000f,
                      Households = 5f,
                      Median_income = 1000f,
                      Median_house_value = 1500000f,
                      Ocean_proximity = "Near"
                  },
                  new ModelInput
                  {
                      Id = 2,
                      Longitude = 3f,
                      Latitude = 4f,
                      Housing_median_age = 5f,
                      Total_rooms = 6f,
                      Total_bedrooms = 8f,
                      Population = 20000f,
                      Households = 6f,
                      Median_income = 2000f,
                      Median_house_value = 2500000f,
                      Ocean_proximity = "Near"
                  }
              );
            base.OnModelCreating(modelBuilder);

        }

        public DbSet<ModelInput> ModelInputs { get; set; }
        public DbSet<ModelOutput> ModelOutputs { get; set; }
    }
}