using System.Net;
using System.Net.Http.Json;
using HealthTracker.Shared.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;

namespace HealthTracker.Tests.Integration;

public class AboutEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AboutEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AboutEndpoint_Returns200OkStatus()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/about");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AboutEndpoint_ReturnsValidAboutResponse()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var aboutResponse = await client.GetFromJsonAsync<AboutResponse>("/about");        // Assert
        aboutResponse.Should().NotBeNull();
        aboutResponse!.ApiVersion.Should().NotBeNullOrEmpty();
        aboutResponse.WeighInsCount.Should().BeGreaterThanOrEqualTo(0);
        aboutResponse.RunsCount.Should().BeGreaterThanOrEqualTo(0);
        // LastWeighInDate and LastRunDate can be null for now
    }
}
