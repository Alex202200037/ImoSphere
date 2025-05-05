using Microsoft.EntityFrameworkCore;
using ImoSphere.Models;
using Microsoft.AspNetCore.Identity;

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
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Seed roles
            var roles = new[] { "Admin", "Seller", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed admin user
            var adminEmail = "admin@imosphere.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = "AdminUser", // Set a username for the admin
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Seed seller user
            var sellerEmail = "seller@imosphere.com";
            var sellerUser = await userManager.FindByEmailAsync(sellerEmail);
            if (sellerUser == null)
            {
                sellerUser = new IdentityUser
                {
                    UserName = "SellerUser", // Set a username for the seller
                    Email = sellerEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(sellerUser, "Seller@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(sellerUser, "Seller");
                }
            }

            // Seed regular user
            var userEmail = "user@imosphere.com";
            var regularUser = await userManager.FindByEmailAsync(userEmail);
            if (regularUser == null)
            {
                regularUser = new IdentityUser
                {
                    UserName = "RegularUser", // Set a username for the regular user
                    Email = userEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(regularUser, "User@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(regularUser, "User");
                }
            }
        }
    }
}
