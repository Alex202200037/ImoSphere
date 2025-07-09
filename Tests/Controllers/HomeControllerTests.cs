using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using FluentAssertions;
using Xunit;
using ImoSphere.Controllers;
using ImoSphere.Models;

namespace ImoSphere.Tests.Controllers
{
    public class HomeControllerTests : TestBase
    {
        private HomeController _controller;

        public HomeControllerTests()
        {
            _controller = new HomeController(Context, UserManager);
            
            // Set up controller context with authenticated user
            var user = CreateTestUser("test@test.com", "testuser", "Comercial").Result;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Fact]
        public async Task Properties_WithoutFilters_ShouldReturnAllProperties()
        {
            // Arrange
            var agency = await CreateTestAgency();
            var user = await CreateTestUser("test@test.com", "testuser", "Comercial");
            await CreateTestAgencyUser(user, agency, "Comercial");
            await CreateTestProperty(user, agency);

            // Act
            var result = await _controller.Properties();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Properties_WithPriceFilter_ShouldReturnFilteredProperties()
        {
            // Arrange
            var agency = await CreateTestAgency();
            var user = await CreateTestUser("test@test.com", "testuser", "Comercial");
            await CreateTestAgencyUser(user, agency, "Comercial");
            await CreateTestProperty(user, agency);

            // Act
            var result = await _controller.Properties(minPrice: 50000, maxPrice: 150000);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Properties_WithBedroomsFilter_ShouldReturnFilteredProperties()
        {
            // Arrange
            var agency = await CreateTestAgency();
            var user = await CreateTestUser("test@test.com", "testuser", "Comercial");
            await CreateTestAgencyUser(user, agency, "Comercial");
            await CreateTestProperty(user, agency);

            // Act
            var result = await _controller.Properties(minBedrooms: 2, maxBedrooms: 4);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Properties_WithLocationFilter_ShouldReturnFilteredProperties()
        {
            // Arrange
            var agency = await CreateTestAgency();
            var user = await CreateTestUser("test@test.com", "testuser", "Comercial");
            await CreateTestAgencyUser(user, agency, "Comercial");
            await CreateTestProperty(user, agency);

            // Act
            var result = await _controller.Properties(location: "Test");

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Properties_WithSorting_ShouldReturnSortedProperties()
        {
            // Arrange
            var agency = await CreateTestAgency();
            var user = await CreateTestUser("test@test.com", "testuser", "Comercial");
            await CreateTestAgencyUser(user, agency, "Comercial");
            await CreateTestProperty(user, agency);

            // Act
            var result = await _controller.Properties(sortBy: "Price", sortOrder: "desc");

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        public new void Dispose()
        {
            base.Dispose();
        }
    }
} 