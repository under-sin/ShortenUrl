using System.Net;
using System.Net.Http.Json;
using Shouldly;
using ShortenUrl.API.Requests;

namespace ShortenUrl.Tests.Integration;

public class ShortenUrlControllerIntegrationTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestWebAppFactory _factory;

    public ShortenUrlControllerIntegrationTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ShortenUrl_ShouldReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        var request = new ShortenRequest
        {
            OriginalUrl = "https://www.example.com/very/long/url/that/needs/to/be/shortened"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/ShortenUrl", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        
        var result = await response.Content.ReadFromJsonAsync<ShortenUrlResponse>();
        result.ShouldNotBeNull();
        result.ShortUrl.ShouldNotBeNullOrEmpty();
        result.ShortUrl.Length.ShouldBeLessThanOrEqualTo(7);
    }

    [Fact]
    public async Task ShortenUrl_ShouldReturnBadRequest_WhenUrlIsEmpty()
    {
        // Arrange
        var request = new ShortenRequest
        {
            OriginalUrl = string.Empty
        };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/ShortenUrl", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShortenUrl_ShouldReturnBadRequest_WhenUrlIsInvalid()
    {
        // Arrange
        var request = new ShortenRequest
        {
            OriginalUrl = "not-a-valid-url"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/ShortenUrl", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RedirectToOriginalUrl_ShouldReturnRedirect_WhenShortCodeExists()
    {
        // Arrange
        var originalUrl = "https://www.github.com/test";
        var shortenRequest = new ShortenRequest { OriginalUrl = originalUrl };
        
        var shortenResponse = await _client.PostAsJsonAsync("/v1/ShortenUrl", shortenRequest);
        var shortenResult = await shortenResponse.Content.ReadFromJsonAsync<ShortenUrlResponse>();
        
        // Desabilitar auto redirect para testar o status code 302
        var clientWithoutRedirect = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // Act
        var response = await clientWithoutRedirect.GetAsync($"/v1/ShortenUrl/{shortenResult!.ShortUrl}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.MovedPermanently);
        response.Headers.Location.ShouldNotBeNull();
        response.Headers.Location.ToString().ShouldBe(originalUrl);
    }

    [Fact]
    public async Task RedirectToOriginalUrl_ShouldReturnNotFound_WhenShortCodeDoesNotExist()
    {
        // Arrange
        const string nonExistentShortCode = "XXXXXX";
        
        var clientWithoutRedirect = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // Act
        var response = await clientWithoutRedirect.GetAsync($"/v1/ShortenUrl/{nonExistentShortCode}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShortenUrl_ShouldGenerateDifferentCodes_ForDifferentUrls()
    {
        // Arrange
        var request1 = new ShortenRequest { OriginalUrl = "https://www.example.com/url1" };
        var request2 = new ShortenRequest { OriginalUrl = "https://www.example.com/url2" };

        // Act
        var response1 = await _client.PostAsJsonAsync("/v1/ShortenUrl", request1);
        var response2 = await _client.PostAsJsonAsync("/v1/ShortenUrl", request2);

        var result1 = await response1.Content.ReadFromJsonAsync<ShortenUrlResponse>();
        var result2 = await response2.Content.ReadFromJsonAsync<ShortenUrlResponse>();

        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();
        result1.ShortUrl.ShouldNotBe(result2.ShortUrl);
    }

    [Fact]
    public async Task FullWorkflow_ShouldWork_EndToEnd()
    {
        // Arrange
        const string originalUrl = "https://www.microsoft.com/dotnet";
        var shortenRequest = new ShortenRequest { OriginalUrl = originalUrl };

        // Act 1: Shorten URL
        var shortenResponse = await _client.PostAsJsonAsync("/v1/ShortenUrl", shortenRequest);
        shortenResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        
        var shortenResult = await shortenResponse.Content.ReadFromJsonAsync<ShortenUrlResponse>();
        shortenResult.ShouldNotBeNull();
        var shortCode = shortenResult.ShortUrl;

        // Act 2: Redirect usando o código encurtado
        var clientWithoutRedirect = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        
        var redirectResponse = await clientWithoutRedirect.GetAsync($"/v1/ShortenUrl/{shortCode}");

        // Assert
        redirectResponse.StatusCode.ShouldBe(HttpStatusCode.MovedPermanently);
        redirectResponse.Headers.Location.ShouldNotBeNull();
        redirectResponse.Headers.Location.ToString().ShouldBe(originalUrl);
    }

    private record ShortenUrlResponse(string ShortUrl);
}

