using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using ImoSphere.Controllers;
using ImoSphere.Data;
using ImoSphere.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ImoSphere.Tests.Controllers
{
    public class PropertyControllerTests : TestBase
    {
        [Fact]
        public async Task Index_ShouldReturnViewResult()
        {
            // Arrange
            var controller = new PropertiesController(Context, UserManager);

            // Act
            var result = await controller.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        public new void Dispose()
        {
            base.Dispose();
        }
    }
} 