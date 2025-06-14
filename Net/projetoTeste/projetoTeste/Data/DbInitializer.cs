using System;
using System.Linq;
using projetoTeste.Models;

namespace projetoTeste.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.ModelInputs.Any())
            {
                return;   // DB has been seeded
            }

            var modelInputs = new ModelInput[]
            {
            new ModelInput{
                        Longitude = 2f,
                        Latitude = 3f,
                        Housing_median_age = 4f,
                        Total_rooms = 4f,
                        Total_bedrooms =3f ,
                        Population = 10000f,
                        Households = 5f,
                        Median_income = 1000f,
                        Median_house_value = 1500000f,
                        Ocean_proximity = "Near"},
            new ModelInput{
                        Longitude = 3f,
                        Latitude = 4f,
                        Housing_median_age = 5f,
                        Total_rooms = 6f,
                        Total_bedrooms =8f ,
                        Population = 20000f,
                        Households = 6f,
                        Median_income = 2000f,
                        Median_house_value = 2500000f,
                        Ocean_proximity = "Near"}
            };
            foreach (ModelInput s in modelInputs)
            {
                context.ModelInputs.Add(s);
            }
            context.SaveChanges();


        }
    }
}