using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using ImoSphere.Data;
using ImoSphere.Models;
using Xunit;

namespace ImoSphere.Tests.Data
{
    public class ApplicationDbContextTests : TestBase, IDisposable
    {
        [Fact]
        public async Task SaveChanges_WithValidProperty_ShouldSaveToDatabase()
        {
            // Arrange
            var agency = await CreateTestAgency();
            var user = await CreateTestUser("test@test.com", "testuser", "Comercial");
            await CreateTestAgencyUser(user, agency, "Comercial");

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
                CreatedByUserId = user.Id,
                Latitude = 38.7223,
                Longitude = -9.1393
            };

            // Act
            Context.Properties.Add(property);
            var result = await Context.SaveChangesAsync();

            // Assert
            result.Should().BeGreaterThan(0);
            Context.Properties.Should().Contain(p => p.Name == "Test Property");
        }

        [Fact]
        public async Task Properties_WithAgency_ShouldIncludeAgency()
        {
            // Arrange
            var agency = await CreateTestAgency();
            var user = await CreateTestUser("test@test.com", "testuser", "Comercial");
            await CreateTestAgencyUser(user, agency, "Comercial");
            var property = await CreateTestProperty(user, agency);

            // Act
            var result = await Context.Properties
                .Include(p => p.Agency)
                .FirstOrDefaultAsync(p => p.Id == property.Id);

            // Assert
            result.Should().NotBeNull();
            result.Agency.Should().NotBeNull();
            result.Agency.Name.Should().Be(agency.Name);
        }

        [Fact]
        public async Task Properties_WithImages_ShouldIncludeImages()
        {
            // Arrange
            var agency = await CreateTestAgency();
            var user = await CreateTestUser("test@test.com", "testuser", "Comercial");
            await CreateTestAgencyUser(user, agency, "Comercial");
            var property = await CreateTestProperty(user, agency);

            var image = new PropertyImage
            {
                PropertyId = property.Id,
                ImageUrl = "test-image.jpg"
            };

            Context.PropertyImages.Add(image);
            await Context.SaveChangesAsync();

            // Act
            var result = await Context.Properties
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == property.Id);

            // Assert
            result.Should().NotBeNull();
            result.Images.Should().HaveCount(1);
            result.Images.First().ImageUrl.Should().Be("test-image.jpg");
        }

        [Fact]
        public async Task DeleteProperty_ShouldDeleteRelatedData()
        {
            // Arrange
            var agency = await CreateTestAgency();
            var user = await CreateTestUser("test@test.com", "testuser", "Comercial");
            await CreateTestAgencyUser(user, agency, "Comercial");
            var property = await CreateTestProperty(user, agency);

            var image = new PropertyImage
            {
                PropertyId = property.Id,
                ImageUrl = "test-image.jpg"
            };

            var favorite = new Favorite
            {
                UserId = user.Id,
                PropertyId = property.Id
            };

            Context.PropertyImages.Add(image);
            Context.Favorites.Add(favorite);
            await Context.SaveChangesAsync();

            // Act
            Context.Properties.Remove(property);
            await Context.SaveChangesAsync();

            // Assert
            Context.Properties.Should().NotContain(p => p.Id == property.Id);
            Context.PropertyImages.Should().NotContain(i => i.PropertyId == property.Id);
            Context.Favorites.Should().NotContain(f => f.PropertyId == property.Id);
        }

        public new void Dispose()
        {
            base.Dispose();
        }
    }
} 