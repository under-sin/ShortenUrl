using Microsoft.Extensions.Configuration;
using Moq;
using ShortenUrl.Application.Services;
using ShortenUrl.Application.Utils;
using ShortenUrl.Domain.Entities;
using ShortenUrl.Domain.Repositories;
using ShortenUrl.Domain.Services;
using Shouldly;

namespace ShortenUrl.Tests.Unit.Application;

public class ShortenUrlServiceTests
{
    private readonly Mock<ISequenceGenerator> _sequenceGeneratorMock;
    private readonly Mock<IUrlsRepository> _urlsRepositoryMock;
    private readonly ShortenUrlService _sut;

    public ShortenUrlServiceTests()
    {
        var inMemorySettings = new Dictionary<string, string?> {
            {"Base62:CharacterSet", "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        
        EncodeBase62.Initialize(configuration);
        
        _sequenceGeneratorMock = new Mock<ISequenceGenerator>();
        _urlsRepositoryMock = new Mock<IUrlsRepository>();
        _sut = new ShortenUrlService(_sequenceGeneratorMock.Object, _urlsRepositoryMock.Object);
    }

    [Fact]
    public async Task ShortenAsync_ShouldReturnShortCode_WhenUrlIsValid()
    {
        // Arrange
        const string originalUrl = "https://www.example.com";
        const long expectedId = 123;
        const string expectedShortCode = "1Z"; // 123 in base62

        _sequenceGeneratorMock
            .Setup(x => x.GetNextIdAsync())
            .ReturnsAsync(expectedId);

        _urlsRepositoryMock
            .Setup(x => x.AddUrlAsync(It.IsAny<Url>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.ShortenAsync(originalUrl, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedShortCode);
        _urlsRepositoryMock.Verify(x => 
            x.AddUrlAsync(It.Is<Url>(u => u.OriginalUrl == originalUrl && u.ShortCode == expectedShortCode), 
                It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task ShortenAsync_ShouldThrowException_WhenRepositoryFails()
    {
        // Arrange
        const string originalUrl = "https://www.example.com";
        const long expectedId = 123;

        _sequenceGeneratorMock
            .Setup(x => x.GetNextIdAsync())
            .ReturnsAsync(expectedId);

        _urlsRepositoryMock
            .Setup(x => x.AddUrlAsync(It.IsAny<Url>(), CancellationToken.None))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Should.ThrowAsync<Exception>(() => _sut.ShortenAsync(originalUrl, CancellationToken.None));
        exception.Message.ShouldBe("Database error");
    }

    [Fact]
    public async Task GetOriginalUrlAsync_ShouldReturnOriginalUrl_WhenShortCodeExists()
    {
        // Arrange
        const string shortCode = "1Z";
        const string expectedOriginalUrl = "https://www.example.com";
        var urlEntity = new Url(shortCode, expectedOriginalUrl);

        _urlsRepositoryMock
            .Setup(x => x.GetUrlByShortCodeAsync(shortCode, CancellationToken.None))
            .ReturnsAsync(urlEntity);

        // Act
        var result = await _sut.GetOriginalUrlAsync(shortCode, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedOriginalUrl);
    }

    [Fact]
    public async Task GetOriginalUrlAsync_ShouldReturnNull_WhenShortCodeDoesNotExist()
    {
        // Arrange
        const string shortCode = "nonexistent";

        _urlsRepositoryMock
            .Setup(x => x.GetUrlByShortCodeAsync(shortCode, CancellationToken.None))
            .ReturnsAsync((Url?)null);

        // Act
        var result = await _sut.GetOriginalUrlAsync(shortCode, CancellationToken.None);

        // Assert
        result.ShouldBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetOriginalUrlAsync_ShouldReturnNull_WhenShortCodeIsNullOrEmpty(string shortCode)
    {
        // Act
        var result = await _sut.GetOriginalUrlAsync(shortCode, CancellationToken.None);

        // Assert
        result.ShouldBeNull();
        _urlsRepositoryMock.Verify(
            x => x.GetUrlByShortCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }
}
