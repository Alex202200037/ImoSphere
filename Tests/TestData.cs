using ImoSphere.Models;

namespace ImoSphere.Tests
{
    public static class TestData
    {
        public static class Users
        {
            public static ApplicationUser CreateSuperAdmin() => new()
            {
                UserName = "superadmin",
                Email = "superadmin@test.com",
                EmailConfirmed = true
            };

            public static ApplicationUser CreateAdmin() => new()
            {
                UserName = "admin",
                Email = "admin@test.com",
                EmailConfirmed = true
            };

            public static ApplicationUser CreateComercial() => new()
            {
                UserName = "comercial",
                Email = "comercial@test.com",
                EmailConfirmed = true
            };

            public static ApplicationUser CreateUser() => new()
            {
                UserName = "user",
                Email = "user@test.com",
                EmailConfirmed = true
            };
        }

        public static class Agencies
        {
            public static Agency CreateAgency(string name = "Test Agency") => new()
            {
                Name = name
            };
        }

        public static class Properties
        {
            public static Property CreateProperty(string name = "Test Property") => new()
            {
                Name = name,
                Description = "Test Description",
                Price = 100000,
                Bedrooms = 3,
                Bathrooms = 2,
                Area = 150,
                Location = "Test Location",
                YearBuilt = 2020,
                Latitude = 38.7223,
                Longitude = -9.1393
            };

            public static Property CreateLuxuryProperty() => new()
            {
                Name = "Luxury Villa",
                Description = "Luxury villa with pool",
                Price = 500000,
                Bedrooms = 5,
                Bathrooms = 4,
                Area = 300,
                Location = "Cascais",
                YearBuilt = 2022,
                Latitude = 38.6979,
                Longitude = -9.4214
            };

            public static Property CreateApartment() => new()
            {
                Name = "Modern Apartment",
                Description = "Modern apartment in city center",
                Price = 200000,
                Bedrooms = 2,
                Bathrooms = 1,
                Area = 80,
                Location = "Lisboa",
                YearBuilt = 2018,
                Latitude = 38.7223,
                Longitude = -9.1393
            };
        }

        public static class PropertyImages
        {
            public static PropertyImage CreateMainImage() => new()
            {
                ImageUrl = "main-image.jpg"
            };

            public static PropertyImage CreateSecondaryImage() => new()
            {
                ImageUrl = "secondary-image.jpg"
            };
        }

        public static class AgencyUsers
        {
            public static AgencyUser CreateSuperAdminAgencyUser() => new()
            {
                Role = "SuperAdmin"
            };

            public static AgencyUser CreateAdminAgencyUser() => new()
            {
                Role = "Admin"
            };

            public static AgencyUser CreateComercialAgencyUser() => new()
            {
                Role = "Comercial"
            };
        }

        public static class Favorites
        {
            public static Favorite CreateFavorite() => new()
            {
                // UserId e PropertyId serão definidos nos testes
            };
        }
    }
}