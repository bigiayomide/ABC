using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using ABC.Web;

namespace ABC.Test
{
    public class ContentIntegrationTests(WebApplicationFactory<Program> factory)
        : IClassFixture<WebApplicationFactory<Program>>
    {
        [Fact]
        public async Task GetPreface_ReturnsPrefacePage()
        {
            // Arrange
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("ome Page");
        }

        [Fact]
        public async Task GetSection_WithValidSection_ReturnsSectionPage()
        {
            // Arrange
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/Content/Section?sectionName=chapter-1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Chapter 1 Title");
        }

        [Fact]
        public async Task GetSection_WithInvalidSection_ReturnsErrorPage()
        {
            // Arrange
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/Content/Section?sectionName=non-existent-section");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Section not found");
        }
    }
}
