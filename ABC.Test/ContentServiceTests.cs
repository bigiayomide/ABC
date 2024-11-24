using ABC.Web.Services.Implementations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Moq;

namespace ABC.Test;

public class ContentServiceTests
{
    private readonly Mock<IWebHostEnvironment> _mockEnv = new();
    private readonly Mock<ILogger<ContentService>> _mockLogger = new();

    [Fact]
    public void Constructor_LoadsContentSuccessfully_WhenJsonExists()
    {
        // Arrange
        var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);

        var mockWebRootPath = Path.Combine(tempDirectory, "wwwroot");
        var mockContentPath = Path.Combine(mockWebRootPath, "content");
        Directory.CreateDirectory(mockContentPath);

        var mockJsonFile = Path.Combine(mockContentPath, "content.json");
        const string mockJson = @"{
        ""preface"": { ""Title"": ""Welcome"", ""Navigation"": [] }
    }";

        _mockEnv.Setup(env => env.WebRootPath).Returns(mockWebRootPath);
        File.WriteAllText(mockJsonFile, mockJson);

        // Act
        var contentService = new ContentService(_mockEnv.Object, _mockLogger.Object);

        // Assert
        var preface = contentService.GetPreface();
        Assert.NotNull(preface);
        Assert.Equal("Welcome", preface.Title);

        // Cleanup
        File.Delete(mockJsonFile);
        Directory.Delete(tempDirectory, true);
    }


    [Fact]
    public void Constructor_ThrowsFileNotFoundException_WhenNoJsonFilesExist()
    {
        // Arrange
        var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);
        var mockWebRootPath = Path.Combine(tempDirectory, "wwwroot");
        Directory.CreateDirectory(mockWebRootPath);
        _mockEnv.Setup(env => env.WebRootPath).Returns(mockWebRootPath);

        // Act & Assert
        Assert.Throws<DirectoryNotFoundException>(() => new ContentService(_mockEnv.Object, _mockLogger.Object));

        // Cleanup
        Directory.Delete(tempDirectory, true);
    }


    [Fact]
    public void GetPreface_ReturnsNull_WhenPrefaceSectionNotFound()
    {
        // Arrange
        const string mockJson = @"{}";
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
        var mockJson = @"{
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
        const string mockJson = @"{
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
        var mockContentPath = Path.Combine("wwwroot", "content");
        var mockJsonFile = Path.Combine(mockContentPath, "content.json");

        _mockEnv.Setup(env => env.WebRootPath).Returns("wwwroot");
        Directory.CreateDirectory(mockContentPath);
        File.WriteAllText(mockJsonFile, contentJson);
    }
}