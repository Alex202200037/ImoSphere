using Microsoft.EntityFrameworkCore;
using ImoSphere.Models;
using Microsoft.AspNetCore.Identity;

namespace ImoSphere.Data
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            // 1. Criar imobiliárias
            if (!context.Agencies.Any())
            {
                context.Agencies.AddRange(
                    new Agency { Name = "ERA" },
                    new Agency { Name = "REMAX" },
                    new Agency { Name = "Century21" }
                );
                await context.SaveChangesAsync();
            }

            var agencies = context.Agencies.ToList();

            // 2. Criar utilizadores e roles
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[] { "Admin", "Comercial", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 3. Criar admins e comerciais para cada agência
            foreach (var agency in agencies)
            {
                var agencyEmail = agency.Name.ToLower();
                // Admin
                var adminEmail = $"admin1.{agencyEmail}@imosphere.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = $"admin1.{agencyEmail}",
                        Email = adminEmail,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(adminUser, "Admin@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                        context.AgencyUsers.Add(new AgencyUser
                        {
                            UserId = adminUser.Id,
                            AgencyId = agency.Id,
                            Role = "Admin"
                        });
                    }
                }

                // Comercial
                var comercialEmail = $"comercial1.{agencyEmail}@imosphere.com";
                var comercialUser = await userManager.FindByEmailAsync(comercialEmail);
                if (comercialUser == null)
                {
                    comercialUser = new ApplicationUser
                    {
                        UserName = $"comercial1.{agencyEmail}",
                        Email = comercialEmail,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(comercialUser, "Comercial@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(comercialUser, "Comercial");
                        context.AgencyUsers.Add(new AgencyUser
                        {
                            UserId = comercialUser.Id,
                            AgencyId = agency.Id,
                            Role = "Comercial"
                        });
                    }
                }
            }
            await context.SaveChangesAsync();

            // 4. Criar utilizador comum sem agência
            var userEmail = "user@imosphere.com";
            var regularUser = await userManager.FindByEmailAsync(userEmail);
            if (regularUser == null)
            {
                regularUser = new ApplicationUser
                {
                    UserName = "RegularUser",
                    Email = userEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(regularUser, "User@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(regularUser, "User");
                }
            }

            // 5. Criar casas para cada agência
            if (!context.Properties.Any())
            {
                var properties = new List<Property>
                {
                    new Property
                    {
                        Name = "Luxury Apartment",
                        Description = "Located in the heart of the city, this apartment offers stunning views and modern amenities.",
                        Price = 500000,
                        Bedrooms = 2,
                        Bathrooms = 2,
                        Area = 120,
                        Location = "Downtown",
                        YearBuilt = 2015,
                        AgencyId = agencies.First(a => a.Name == "ERA").Id,
                        Images = new List<PropertyImage>
                        {
                            new PropertyImage { ImageUrl = "/images/moradia1.jpg" },
                            new PropertyImage { ImageUrl = "/images/moradia2.jpg" },
                            new PropertyImage { ImageUrl = "/images/moradia3.jpg" }
                        }
                    },
                    new Property
                    {
                        Name = "Cozy Cottage",
                        Description = "A charming cottage in the countryside, perfect for a peaceful retreat.",
                        Price = 250000,
                        Bedrooms = 3,
                        Bathrooms = 2,
                        Area = 150,
                        Location = "Countryside",
                        YearBuilt = 2005,
                        AgencyId = agencies.First(a => a.Name == "REMAX").Id,
                        Images = new List<PropertyImage>
                        {
                            new PropertyImage { ImageUrl = "/images/moradia2.jpg" },
                            new PropertyImage { ImageUrl = "/images/moradia1.jpg" },
                            new PropertyImage { ImageUrl = "/images/moradia3.jpg" }
                        }
                    },
                    new Property
                    {
                        Name = "Beachfront Villa",
                        Description = "Enjoy the ocean breeze in this luxurious villa with private beach access.",
                        Price = 1200000,
                        Bedrooms = 4,
                        Bathrooms = 3,
                        Area = 350,
                        Location = "Beachfront",
                        YearBuilt = 2020,
                        AgencyId = agencies.First(a => a.Name == "Century21").Id,
                        Images = new List<PropertyImage>
                        {
                            new PropertyImage { ImageUrl = "/images/moradia3.jpg" },
                            new PropertyImage { ImageUrl = "/images/moradia1.jpg" },
                            new PropertyImage { ImageUrl = "/images/moradia2.jpg" }
                        }
                    }
                };
                context.Properties.AddRange(properties);
                await context.SaveChangesAsync();
            }
        }
    }
}
