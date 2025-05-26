using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using HealthTracker.Api.Data;
using HealthTracker.Core.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace HealthTracker.Tests;

/// <summary>
/// Custom WebApplicationFactory for testing with shared in-memory SQLite database
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private static readonly string SharedInMemoryConnectionString = $"Data Source=TestDb_{Guid.NewGuid():N};Mode=Memory;Cache=Shared";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing database services
            var dbConnectionFactoryDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IDbConnectionFactory));
            if (dbConnectionFactoryDescriptor != null)
            {
                services.Remove(dbConnectionFactoryDescriptor);
            }

            var dbInitializerDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbInitializer));
            if (dbInitializerDescriptor != null)
            {
                services.Remove(dbInitializerDescriptor);
            }

            // Add test database services with shared in-memory SQLite database
            services.AddSingleton<IDbConnectionFactory>(new SqliteConnectionFactory(SharedInMemoryConnectionString));
            services.AddSingleton<DbInitializer>();
        });
    }
}

/// <summary>
/// Integration tests for the Health Tracker API endpoints
/// </summary>
public class ApiEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ApiEndpointsTests(TestWebApplicationFactory factory)
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
