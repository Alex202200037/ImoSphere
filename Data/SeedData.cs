using Microsoft.EntityFrameworkCore;
using ImoSphere.Models;
using Microsoft.AspNetCore.Identity;

namespace ImoSphere.Data
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            // VERIFICAÇÃO INICIAL: Se já existem propriedades, não fazer seed
            if (context.Properties.Any())
            {
                Console.WriteLine("[SEED] Base de dados já contém dados. Skipping seed.");
                return;
            }

            Console.WriteLine("[SEED] Iniciando seed de dados...");

            // 1. Criar imobiliárias
            if (!context.Agencies.Any())
            {
                Console.WriteLine("[SEED] Criando agências...");
                context.Agencies.AddRange(
                    new Agency { Name = "ERA" },
                    new Agency { Name = "REMAX" },
                    new Agency { Name = "Century21" },
                    new Agency { Name = "KW" },
                    new Agency { Name = "Fine and Country" }
                );
                await context.SaveChangesAsync();
                Console.WriteLine("[SEED] Agências criadas com sucesso.");
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
                    Console.WriteLine($"[SEED] Role '{role}' criada.");
                }
            }

            // Seed SuperAdmin
            var superAdminEmail = "imosphere.admin@imosphere.com";
            var superAdminUser = await userManager.FindByEmailAsync(superAdminEmail);
            if (superAdminUser == null)
            {
                Console.WriteLine("[SEED] Criando SuperAdmin...");
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
                    Console.WriteLine("[SEED] SuperAdmin criado com sucesso.");
                }
                else
                {
                    Console.WriteLine($"[SEED] Falha a criar SuperAdmin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            // Adicionar agência especial ImoSphere
            var imoSphereAgency = context.Agencies.FirstOrDefault(a => a.Name == "ImoSphere");
            if (imoSphereAgency == null)
            {
                Console.WriteLine("[SEED] Criando agência ImoSphere...");
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
                Console.WriteLine("[SEED] SuperAdmin associado à agência ImoSphere.");
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

            // Lista de imagens disponíveis (novas imagens da pasta Houses)
            var imagens = new[] {
                "1Fotos-Projeto.jpg", "2Fotos-Projeto.jpg", "3Fotos-Projeto.jpg", "4Fotos-Projeto.jpeg",
                "5Fotos-Projeto.jpg", "6Fotos-Projeto.jpg", "7Fotos-Projeto.jpg", "8Fotos-Projeto.jpg",
                "9Fotos-Projeto.jpg", "10Fotos-Projeto.jpg", "11Fotos-Projeto.jpg", "12Fotos-Projeto.jpeg",
                "13Fotos-Projeto.jpg", "14Fotos-Projeto.jpg", "15Fotos-Projeto.jpeg", "16Fotos-Projeto.jpg",
                "17Fotos-Projeto.jpg", "18Fotos-Projeto.jpg", "19Fotos-Projeto.jpg", "20Fotos-Projeto.jpeg",
                "21Fotos-Projeto.jpg", "22Fotos-Projeto.jpeg", "23Fotos-Projeto.jpg", "24Fotos-Projeto.jpeg",
                "25Fotos-Projeto.jpg", "26Fotos-Projeto.jpg", "27Fotos-Projeto.jpg", "28Fotos-Projeto.jpg",
                "29Fotos-Projeto.jpg", "30Fotos-Projeto.jpg", "31Fotos-Projeto.jpeg", "32Fotos-Projeto.jpg",
                "33Fotos-Projeto.jpg", "34Fotos-Projeto.jpg", "35Fotos-Projeto.jpg", "36Fotos-Projeto.jpg",
                "37Fotos-Projeto.jpg", "38Fotos-Projeto.jpg", "39Fotos-Projeto.png", "40Fotos-Projeto.png",
                "41Fotos-Projeto.jpeg", "moradia1.jpg", "moradia2.jpg", "moradia3.jpg"
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
                AgencyUser adminAgencyUser = null;
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
                        adminAgencyUser = new AgencyUser
                        {
                            UserId = adminUser.Id,
                            AgencyId = agency.Id,
                            Role = "Admin"
                        };
                        context.AgencyUsers.Add(adminAgencyUser);
                        await context.SaveChangesAsync();
                        Console.WriteLine($"[SEED] Admin criado: {adminUser.UserName} ({adminUser.Email})");
                    }
                    else
                    {
                        Console.WriteLine($"[SEED] Falha a criar admin {agencySeed.Admin.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        continue;
                    }
                }
                else
                {
                    // Se o admin já existe, buscar o seu AgencyUser
                    adminAgencyUser = context.AgencyUsers.FirstOrDefault(au => au.UserId == adminUser.Id && au.AgencyId == agency.Id);
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
                                Role = "Comercial",
                                AdminId = adminAgencyUser?.UserId // Associar ao admin da agência
                            });
                            await context.SaveChangesAsync();
                            Console.WriteLine($"[SEED] Comercial criado: {comercialUser.UserName} ({comercialUser.Email}) - Admin: {adminAgencyUser?.UserId}");
                        }
                        else
                        {
                            Console.WriteLine($"[SEED] Falha a criar comercial {comercialSeed.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                            continue;
                        }
                    }
                    else
                    {
                        // Se o comercial já existe, verificar se tem AdminId associado
                        var existingAgencyUser = context.AgencyUsers.FirstOrDefault(au => au.UserId == comercialUser.Id && au.AgencyId == agency.Id);
                        if (existingAgencyUser != null && existingAgencyUser.AdminId == null && adminAgencyUser != null)
                        {
                            // Atualizar o AdminId se não estiver definido
                            existingAgencyUser.AdminId = adminAgencyUser.UserId;
                            await context.SaveChangesAsync();
                            Console.WriteLine($"[SEED] AdminId atualizado para comercial existente: {comercialUser.UserName} - Admin: {adminAgencyUser.UserId}");
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

                            // Adicionar variação nas coordenadas para evitar sobreposição
                            var latVariation = (rnd.NextDouble() - 0.5) * 0.01; // ±0.005 graus (~500m)
                            var lngVariation = (rnd.NextDouble() - 0.5) * 0.01; // ±0.005 graus (~500m)

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
                                Latitude = cidade.Lat + latVariation,
                                Longitude = cidade.Lng + lngVariation,
                                YearBuilt = ano,
                                AgencyId = agency.Id,
                                CreatedByUserId = comercialUser.Id,
                                Images = new List<PropertyImage>()
                            };

                            // 6-7 imagens por casa, sem repetir na mesma casa
                            int numImagens = rnd.Next(6, 8); // 6 ou 7 imagens
                            for (int img = 0; img < numImagens; img++)
                            {
                                casa.Images.Add(new PropertyImage { ImageUrl = "/images/Houses/" + imagens[imgIdx % imagens.Length] });
                                imgIdx++;
                            }

                            context.Properties.Add(casa);
                        }
                    }
                }
            }

            await context.SaveChangesAsync();
            Console.WriteLine($"[SEED] {context.Properties.Count()} propriedades criadas com sucesso.");

            // 4. Criar utilizador comum sem agência
            var userEmail = "user@imosphere.com";
            var regularUser = await userManager.FindByEmailAsync(userEmail);
            if (regularUser == null)
            {
                Console.WriteLine("[SEED] Criando utilizador comum...");
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
                    Console.WriteLine("[SEED] Utilizador comum criado com sucesso.");
                }
                else
                {
                    Console.WriteLine($"[SEED] Falha a criar utilizador comum: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            Console.WriteLine("[SEED] Seed de dados concluído com sucesso!");
        }
    }
}
