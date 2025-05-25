using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using HealthTracker.Core.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HealthTracker.Tests;

/// <summary>
/// Integration tests for the Health Tracker API endpoints
/// </summary>
public class ApiEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ApiEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task PostWeight_WithValidPayload_ReturnsCreated()
    {
        // Arrange
        var weighIn = new WeighIn
        {
            Date = DateTime.UtcNow.Date,
            Weight = 180.5,
            Bmi = 24.5,
            Fat = 15.2,
            Muscle = 42.8,
            RestingMetab = 1850,
            VisceralFat = 12
        };

        var json = JsonSerializer.Serialize(weighIn);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/weight", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetWeightLast5_ReturnsOkWithJsonArray()
    {
        // Act
        var response = await _client.GetAsync("/weight/last/5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

        var responseContent = await response.Content.ReadAsStringAsync();
        var weighIns = JsonSerializer.Deserialize<WeighIn[]>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        weighIns.Should().NotBeNull();
        weighIns.Should().BeOfType<WeighIn[]>();
    }

    [Fact]
    public async Task PostRun_WithValidPayload_ReturnsCreated()
    {
        // Arrange
        var run = new Run
        {
            Date = DateTime.UtcNow.Date,
            Distance = 3.1,
            DistanceUnit = "mi",
            TimeInSeconds = 1800 // 30 minutes
        };

        var json = JsonSerializer.Serialize(run);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/run", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetRunLast5_ReturnsOkWithJsonArray()
    {
        // Act
        var response = await _client.GetAsync("/run/last/5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

        var responseContent = await response.Content.ReadAsStringAsync();
        var runs = JsonSerializer.Deserialize<Run[]>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        runs.Should().NotBeNull();
        runs.Should().BeOfType<Run[]>();
    }
}
