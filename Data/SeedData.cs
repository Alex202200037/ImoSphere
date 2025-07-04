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
                    new Agency { Name = "Century21" },
                    new Agency { Name = "KW" },
                    new Agency { Name = "Fine and Country" }
                );
                await context.SaveChangesAsync();
            }

            var agencies = context.Agencies.ToList();

            // 2. Criar utilizadores e roles
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[] { "SuperAdmin", "Admin", "Comercial", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed SuperAdmin
            var superAdminEmail = "imosphere.admin@imosphere.com";
            var superAdminUser = await userManager.FindByEmailAsync(superAdminEmail);
            if (superAdminUser == null)
            {
                superAdminUser = new ApplicationUser
                {
                    UserName = "imosphere.admin",
                    Email = superAdminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(superAdminUser, "Imosphere@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(superAdminUser, "SuperAdmin");
                }
            }

            // Adicionar agência especial ImoSphere
            var imoSphereAgency = context.Agencies.FirstOrDefault(a => a.Name == "ImoSphere");
            if (imoSphereAgency == null)
            {
                imoSphereAgency = new Agency { Name = "ImoSphere" };
                context.Agencies.Add(imoSphereAgency);
                await context.SaveChangesAsync();
            }
            
            // Garantir que o SuperAdmin está associado à agência ImoSphere
            var superAdmin = await userManager.FindByEmailAsync("imosphere.admin@imosphere.com");
            if (superAdmin != null && !context.AgencyUsers.Any(au => au.UserId == superAdmin.Id && au.AgencyId == imoSphereAgency.Id))
            {
                context.AgencyUsers.Add(new AgencyUser
                {
                    UserId = superAdmin.Id,
                    AgencyId = imoSphereAgency.Id,
                    Role = "SuperAdmin"
                });
                await context.SaveChangesAsync();
            }

            // 3. Criar admins e comerciais para cada agência
            var agencySeedData = new[]
            {
                new {
                    Name = "ERA",
                    Admin = new { Name = "João Guilherme", Email = "jguilherme.era@imosphere.com" },
                    Comerciais = new[] {
                        new { Name = "Tiago Silva", Email = "tsilva.era@imosphere.com" },
                        new { Name = "Marta Lopes", Email = "mlopes.era@imosphere.com" },
                        new { Name = "Rui Costa", Email = "rcosta.era@imosphere.com" }
                    }
                },
                new {
                    Name = "REMAX",
                    Admin = new { Name = "Carlota Andrade", Email = "candrade.remax@imosphere.com" },
                    Comerciais = new[] {
                        new { Name = "Ana Pereira", Email = "apereira.remax@imosphere.com" },
                        new { Name = "Pedro Sousa", Email = "psousa.remax@imosphere.com" }
                    }
                },
                new {
                    Name = "Century21",
                    Admin = new { Name = "Miguel Ramos", Email = "mramos.century21@imosphere.com" },
                    Comerciais = new[] {
                        new { Name = "Sofia Martins", Email = "smartins.century21@imosphere.com" },
                        new { Name = "Bruno Alves", Email = "balves.century21@imosphere.com" }
                    }
                },
                new {
                    Name = "KW",
                    Admin = new { Name = "Vera Nunes", Email = "vnunes.kw@imosphere.com" },
                    Comerciais = new[] {
                        new { Name = "Ricardo Pinto", Email = "rpinto.kw@imosphere.com" },
                        new { Name = "Helena Cruz", Email = "hcruz.kw@imosphere.com" }
                    }
                },
                new {
                    Name = "Fine and Country",
                    Admin = new { Name = "André Faria", Email = "afaria.fac@imosphere.com" },
                    Comerciais = new[] {
                        new { Name = "Patrícia Dias", Email = "pdias.fac@imosphere.com" },
                        new { Name = "Luís Amaral", Email = "lamaral.fac@imosphere.com" }
                    }
                }
            };

            // Lista de cidades portuguesas com lat/lng
            var cidadesPortugal = new[] {
                new { Nome = "Lisboa", Lat = 38.7223, Lng = -9.1393 },
                new { Nome = "Porto", Lat = 41.1579, Lng = -8.6291 },
                new { Nome = "Braga", Lat = 41.5454, Lng = -8.4265 },
                new { Nome = "Faro", Lat = 37.0194, Lng = -7.9304 },
                new { Nome = "Coimbra", Lat = 40.2033, Lng = -8.4103 },
                new { Nome = "Évora", Lat = 38.5711, Lng = -7.9135 },
                new { Nome = "Cascais", Lat = 38.6979, Lng = -9.4215 },
                new { Nome = "Aveiro", Lat = 40.6405, Lng = -8.6538 },
                new { Nome = "Setúbal", Lat = 38.5244, Lng = -8.8882 },
                new { Nome = "Viseu", Lat = 40.6610, Lng = -7.9097 },
                new { Nome = "Guimarães", Lat = 41.4445, Lng = -8.2962 },
                new { Nome = "Sintra", Lat = 38.8029, Lng = -9.3817 },
                new { Nome = "Leiria", Lat = 39.7436, Lng = -8.8071 },
                new { Nome = "Portimão", Lat = 37.1366, Lng = -8.5378 },
                new { Nome = "Funchal", Lat = 32.6669, Lng = -16.9241 }
            };
            
            // Lista de imagens disponíveis
            var imagens = new[] {
                "house-casas-1.png", "casa_de_luxo.jpg", "feature.jpg", "JLF_6309.jpg.webp", "AAFComporta05.jpg", "Imativa_Casa_Carrasco_0013.jpg.webp", "180403355.jpg", "Imagem-de-Destaque-Casas-de-Arroz.jpg", "0151b87c-49b2-4117-9c79-264f2633a6ec.jpg", "ar6.jpg", "images.jpeg", "n7_copiar.jpg", "1bba87_000fdde40f714a8f99b412fd80ecf8a8~mv2.jpeg.avif", "images-2.jpeg", "casa-de-campo-paisagismo-inspirado-jardins-italianos-renata-guastelli-credito-miro-martins-8.jpg", "19TV.jpg.webp", "original.webp", "17ecf3bc3e5cd42c747108943682cf09_12.jpg", "01HMRYZCWGNCVNJ3E3V6B3R9B6.jpg", "a95b817470363d6ec74e72531f039257fe83b8af_600x435.jpg", "FACHADA_1.jpg.webp", "casa-pre-fabricada-150-1.jpg", "images-3.jpeg", "3197765d4430d0a57076bbcabc28904d_10335872.jpg", "b8cfa3_bd5e9a28633e4265b3e4ebcfa35a98a6~mv2.png.avif", "images-4.jpeg", "P466-FOTOS_11-Foto-1024x576.jpg", "images-5.jpeg", "preco-casa-modular-38-01.jpg", "af728a9c-b5a6-4510-9618-29d6fb4cd94e.jpg", "353cd957-94e4-4216-a234-282da200b005-1-1024x696.jpg", "o-charme-e-requinte-das-casas-da-madeira3.jpg", "images-6.jpeg", "01_HCG_blog_INTRO_850x450-1-1.jpg", "casa-pre-fabricada-158-1.jpg", "images-7.jpeg", "Passion-House-M1-by-Architect-11-qdy3pf27lthjzd3r90ytfkna07hbawykiemiut4imk.jpg", "1-6-1.jpg", "BIG-20-EXT-1.jpg", "11-e1725896785207-768x384.jpg", "preco-casa-modular-25-01.jpg", "casas-modernas-2024.webp", "avantecture-0vdrg5pr7ny-unsplash.jpg", "spain-4789793_960_720.jpg", "images-8.jpeg", "galivon-casas-modulares-t3-imocasapronta-2.jpg", "casa_de_madeira_pre-fabricada_de_design_moderno.jpg", "01HMRVW9KQZY4HNH91G9SC97EM.jpg"
            };
            
            var imgIdx = 0;
            var rnd = new Random();
            
            foreach (var agencySeed in agencySeedData)
            {
                var agency = agencies.FirstOrDefault(a => a.Name == agencySeed.Name);
                if (agency == null)
                {
                    // Se a agência não existe, continuar para a próxima
                    continue;
                }
                
                // Admin
                var adminUser = await userManager.FindByEmailAsync(agencySeed.Admin.Email);
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = agencySeed.Admin.Name, // Nome completo
                        Email = agencySeed.Admin.Email,
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
                        await context.SaveChangesAsync();
                        Console.WriteLine($"[SEED] Admin criado: {adminUser.UserName} ({adminUser.Email})");
                    }
                    else
                    {
                        Console.WriteLine($"[SEED] Falha a criar admin {agencySeed.Admin.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        continue;
                    }
                }
                
                // Comerciais
                foreach (var comercialSeed in agencySeed.Comerciais)
                {
                    var comercialUser = await userManager.FindByEmailAsync(comercialSeed.Email);
                    if (comercialUser == null)
                    {
                        comercialUser = new ApplicationUser
                        {
                            UserName = comercialSeed.Name, // Nome completo
                            Email = comercialSeed.Email,
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
                            await context.SaveChangesAsync();
                            Console.WriteLine($"[SEED] Comercial criado: {comercialUser.UserName} ({comercialUser.Email})");
                        }
                        else
                        {
                            Console.WriteLine($"[SEED] Falha a criar comercial {comercialSeed.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                            continue;
                        }
                    }
                    
                    // Verificar novamente se o utilizador existe e tem ID válido
                    if (comercialUser != null && !string.IsNullOrEmpty(comercialUser.Id))
                    {
                        // Criar 2-3 casas para cada comercial
                        int casasCount = rnd.Next(2, 4);
                        for (int i = 0; i < casasCount; i++)
                        {
                            var cidade = cidadesPortugal[rnd.Next(cidadesPortugal.Length)];
                            var nomeCasa = $"{(i == 0 ? "Penthouse" : i == 1 ? "Moradia de Luxo" : "Vivenda Exclusiva")} em {cidade.Nome}";
                            var descricao = $"{nomeCasa} com piscina privativa, jardim paisagístico, acabamentos premium, vista panorâmica e localização privilegiada em {cidade.Nome}. Inclui garagem para vários carros, cozinha equipada com eletrodomésticos topo de gama, suite master com closet e spa, e sistema de domótica de última geração. Ideal para quem procura exclusividade, conforto e requinte.";
                            // Preço múltiplo de 100.000 entre 500.000 e 3.000.000
                            var preco = rnd.Next(5, 31) * 100000;
                            var area = rnd.Next(180, 600);
                            var quartos = rnd.Next(3, 8);
                            var wc = rnd.Next(2, 6);
                            var ano = rnd.Next(2015, 2024);
                            
                            var casa = new Property
                            {
                                Name = nomeCasa,
                                Description = descricao,
                                Price = preco,
                                Bedrooms = quartos,
                                Bathrooms = wc,
                                Area = area,
                                Location = cidade.Nome,
                                Latitude = cidade.Lat,
                                Longitude = cidade.Lng,
                                YearBuilt = ano,
                                AgencyId = agency.Id,
                                CreatedByUserId = comercialUser.Id,
                                Images = new List<PropertyImage>()
                            };
                            
                            // 2 imagens por casa, sem repetir na mesma casa
                            for (int img = 0; img < 2; img++)
                            {
                                casa.Images.Add(new PropertyImage { ImageUrl = "/images/" + imagens[imgIdx % imagens.Length] });
                                imgIdx++;
                            }
                            
                            context.Properties.Add(casa);
                        }
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
        }
    }
}
