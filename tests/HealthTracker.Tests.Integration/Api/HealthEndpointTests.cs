using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using HealthTracker.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace HealthTracker.Tests.Integration.Api
{
    public class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public HealthEndpointTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task HealthEndpoint_ReturnsOk_WhenDatabaseIsHealthy()
        {
            // Arrange
            var mockDatabaseService = new Mock<IDatabaseService>();
            mockDatabaseService.Setup(ds => ds.IsHealthy()).ReturnsAsync(true);

            var client = _factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddSingleton(mockDatabaseService.Object);
                    });
                })
                .CreateClient();

            // Act
            var response = await client.GetAsync("/health");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().Contain("Healthy");

            mockDatabaseService.Verify(ds => ds.IsHealthy(), Times.Once());
        }

        [Fact]
        public async Task HealthEndpoint_ReturnsServiceUnavailable_WhenDatabaseIsUnhealthy()
        {
            // Arrange
            var mockDatabaseService = new Mock<IDatabaseService>();
            mockDatabaseService.Setup(ds => ds.IsHealthy()).ReturnsAsync(false);

            var client = _factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddSingleton(mockDatabaseService.Object);
                    });
                })
                .CreateClient();

            // Act
            var response = await client.GetAsync("/health");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
            mockDatabaseService.Verify(ds => ds.IsHealthy(), Times.Once());
        }
    }
}
