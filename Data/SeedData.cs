using Microsoft.EntityFrameworkCore;
using ImoSphere.Models;

namespace ImoSphere.Data
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            // Verifica se o banco de dados já foi criado
            context.Database.EnsureCreated();

            // Adiciona algumas propriedades de exemplo, caso o banco de dados esteja vazio
            if (!context.Properties.Any()) // Se a tabela 'Properties' estiver vazia
            {
                context.Properties.AddRange(
                    new Property
                    {
                        Name = "Luxury Apartment",
                        Description = "Located in the heart of the city, this apartment offers stunning views and modern amenities.",
                        Price = 500000,
                        ImageUrl = "/images/moradia1.jpg",
                        Bedrooms = 2,
                        Bathrooms = 2,
                        Area = 120,
                        Location = "Downtown",
                        YearBuilt = 2015
                    },
                    new Property
                    {
                        Name = "Cozy Cottage",
                        Description = "A charming cottage in the countryside, perfect for a peaceful retreat.",
                        Price = 250000,
                        ImageUrl = "/images/moradia2.jpg",
                        Bedrooms = 3,
                        Bathrooms = 2,
                        Area = 150,
                        Location = "Countryside",
                        YearBuilt = 2005
                    },
                    new Property
                    {
                        Name = "Beachfront Villa",
                        Description = "Enjoy the ocean breeze in this luxurious villa with private beach access.",
                        Price = 1200000,
                        ImageUrl = "/images/moradia3.jpg",
                        Bedrooms = 4,
                        Bathrooms = 3,
                        Area = 350,
                        Location = "Beachfront",
                        YearBuilt = 2020
                    }
                );

                await context.SaveChangesAsync();
            }
            else
            {
                // Se você quiser adicionar logs para garantir que o seeding não foi feito novamente:
                Console.WriteLine("Properties já foram adicionadas à base de dados.");
            }
        }
    }
}
