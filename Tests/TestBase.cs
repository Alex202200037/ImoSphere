using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ImoSphere.Data;
using ImoSphere.Models;

namespace ImoSphere.Tests
{
    public abstract class TestBase
    {
        protected ApplicationDbContext Context { get; private set; }
        protected UserManager<ApplicationUser> UserManager { get; private set; }
        protected Mock<IUserStore<ApplicationUser>> MockUserStore { get; private set; }

        protected TestBase()
        {
            SetupTestDatabase();
            SetupUserManager();
        }

        private void SetupTestDatabase()
        {
            var databaseName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            Context = new ApplicationDbContext(options);
            Context.Database.EnsureCreated();
        }

        private void SetupUserManager()
        {
            MockUserStore = new Mock<IUserStore<ApplicationUser>>();
            
            // Use a consistent database name for in-memory database
            var databaseName = "TestDatabase_" + Guid.NewGuid().ToString("N")[..8];
            var services = new ServiceCollection();
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.AddLogging();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(databaseName));

            var serviceProvider = services.BuildServiceProvider();
            UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            
            // Create roles for testing
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            CreateRolesIfNotExist(roleManager);
        }
        
        private void CreateRolesIfNotExist(RoleManager<IdentityRole> roleManager)
        {
            var roles = new[] { "Admin", "Comercial", "User", "SuperAdmin" };
            foreach (var role in roles)
            {
                if (!roleManager.RoleExistsAsync(role).Result)
                {
                    roleManager.CreateAsync(new IdentityRole(role)).Wait();
                }
            }
        }

        protected async Task<ApplicationUser> CreateTestUser(string email, string userName, string role = "User")
        {
            // Generate unique username to avoid conflicts
            var uniqueUserName = $"{userName}_{Guid.NewGuid().ToString("N")[..8]}";
            
            var user = new ApplicationUser
            {
                UserName = uniqueUserName,
                Email = email,
                EmailConfirmed = true
            };

            var result = await UserManager.CreateAsync(user, "TestPassword123!");
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create test user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            if (!string.IsNullOrEmpty(role))
            {
                await UserManager.AddToRoleAsync(user, role);
            }

            return user;
        }

        protected async Task<Agency> CreateTestAgency(string name = "Test Agency")
        {
            var agency = new Agency { Name = name };
            Context.Agencies.Add(agency);
            await Context.SaveChangesAsync();
            return agency;
        }

        protected async Task<Property> CreateTestProperty(ApplicationUser createdByUser, Agency agency)
        {
            var property = new Property
            {
                Name = "Test Property",
                Description = "Test Description",
                Price = 100000,
                Bedrooms = 3,
                Bathrooms = 2,
                Area = 150,
                Location = "Test Location",
                YearBuilt = 2020,
                AgencyId = agency.Id,
                CreatedByUserId = createdByUser.Id,
                Latitude = 38.7223,
                Longitude = -9.1393
            };

            Context.Properties.Add(property);
            await Context.SaveChangesAsync();
            return property;
        }

        protected async Task<AgencyUser> CreateTestAgencyUser(ApplicationUser user, Agency agency, string role = "Comercial")
        {
            var agencyUser = new AgencyUser
            {
                UserId = user.Id,
                AgencyId = agency.Id,
                Role = role
            };

            Context.AgencyUsers.Add(agencyUser);
            await Context.SaveChangesAsync();
            return agencyUser;
        }

        public virtual void Dispose()
        {
            Context?.Dispose();
        }
    }
} 