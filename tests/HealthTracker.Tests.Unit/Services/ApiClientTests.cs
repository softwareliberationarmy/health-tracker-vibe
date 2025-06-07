using System.Net;
using FluentAssertions;
using HealthTracker.Cli.Services;
using HealthTracker.Shared.Models;
using Moq;
using Moq.Protected;

namespace HealthTracker.Tests.Unit.Services;

public class ApiClientTests
{
    [Fact]
    public async Task GetAboutAsync_WhenApiIsAvailable_ShouldReturnAboutResponse()
    {
        // Given
        var expectedResponse = new AboutResponse
        {
            ApiVersion = "1.0.0",
            WeighInsCount = 5,
            RunsCount = 10,
            LastWeighInDate = new DateTime(2025, 6, 5),
            LastRunDate = new DateTime(2025, 6, 6)
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(expectedResponse))
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost:50000")
        };

        var apiClient = new ApiClient(httpClient);

        // When
        var result = await apiClient.GetAboutAsync();

        // Then
        result.Should().NotBeNull();
        result!.ApiVersion.Should().Be(expectedResponse.ApiVersion);
        result.WeighInsCount.Should().Be(expectedResponse.WeighInsCount);
        result.RunsCount.Should().Be(expectedResponse.RunsCount);
        result.LastWeighInDate.Should().Be(expectedResponse.LastWeighInDate);
        result.LastRunDate.Should().Be(expectedResponse.LastRunDate);

        handlerMock.Protected()
            .Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri!.ToString() == "http://localhost:50000/about"),
                ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetAboutAsync_WhenApiIsUnavailable_ShouldReturnNull()
    {
        // Given
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Connection refused"));

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost:50000")
        };

        var apiClient = new ApiClient(httpClient);

        // When
        var result = await apiClient.GetAboutAsync();

        // Then
        result.Should().BeNull();
    }
}
