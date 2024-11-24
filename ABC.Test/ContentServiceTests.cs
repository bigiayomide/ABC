using ABC.Web.Services.Implementations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABC.Test
{
    public class ContentServiceTests
    {
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly Mock<ILogger<ContentService>> _mockLogger;

        public ContentServiceTests()
        {
            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockLogger = new Mock<ILogger<ContentService>>();
        }

        [Fact]
        public void Constructor_LoadsContentSuccessfully_WhenJsonExists()
        {
            // Arrange
            string mockContentPath = Path.Combine("wwwroot", "content");
            string mockJsonFile = Path.Combine(mockContentPath, "content.json");
            string mockJson = @"{
            ""preface"": { ""Title"": ""Welcome"", ""Navigation"": [] }
        }";

            _mockEnv.Setup(env => env.WebRootPath).Returns("wwwroot");
            Directory.CreateDirectory(mockContentPath);
            File.WriteAllText(mockJsonFile, mockJson);

            // Act
            var contentService = new ContentService(_mockEnv.Object, _mockLogger.Object);

            // Assert
            Assert.NotNull(contentService.GetPreface());
            Assert.Equal("Welcome", contentService.GetPreface().Title);

            // Cleanup
            File.Delete(mockJsonFile);
            Directory.Delete(mockContentPath);
        }

        [Fact]
        public void Constructor_ThrowsException_WhenJsonFileNotFound()
        {
            // Arrange
            _mockEnv.Setup(env => env.WebRootPath).Returns("wwwroot");

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() =>
                new ContentService(_mockEnv.Object, _mockLogger.Object)
            );
        }

        [Fact]
        public void GetPreface_ReturnsNull_WhenPrefaceSectionNotFound()
        {
            // Arrange
            string mockJson = @"{}";
            SetupMockContentFile(mockJson);

            var contentService = new ContentService(_mockEnv.Object, _mockLogger.Object);

            // Act
            var preface = contentService.GetPreface();

            // Assert
            Assert.Null(preface);
        }

        [Fact]
        public void GetSection_ReturnsSection_WhenSectionExists()
        {
            // Arrange
            string mockJson = @"{
            ""section1"": { ""Title"": ""Section 1"", ""Navigation"": [] }
        }";
            SetupMockContentFile(mockJson);

            var contentService = new ContentService(_mockEnv.Object, _mockLogger.Object);

            // Act
            var section = contentService.GetSection("section1");

            // Assert
            Assert.NotNull(section);
            Assert.Equal("Section 1", section.Title);
        }

        [Fact]
        public void IsLinkingComplete_ReturnsFalse_WhenNavigationIsIncomplete()
        {
            // Arrange
            string mockJson = @"{
            ""preface"": { ""Title"": ""Preface"", ""Navigation"": [{ ""Section"": ""missingSection"" }] }
        }";
            SetupMockContentFile(mockJson);

            var contentService = new ContentService(_mockEnv.Object, _mockLogger.Object);

            // Act
            var isComplete = contentService.IsLinkingComplete();

            // Assert
            Assert.False(isComplete);
        }

        private void SetupMockContentFile(string contentJson)
        {
            string mockContentPath = Path.Combine("wwwroot", "content");
            string mockJsonFile = Path.Combine(mockContentPath, "content.json");

            _mockEnv.Setup(env => env.WebRootPath).Returns("wwwroot");
            Directory.CreateDirectory(mockContentPath);
            File.WriteAllText(mockJsonFile, contentJson);
        }


    }
}
