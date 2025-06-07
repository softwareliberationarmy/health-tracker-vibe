using System.Threading.Tasks;
using FluentAssertions;
using HealthTracker.Api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace HealthTracker.Tests.Integration.Api
{
    public class DatabaseServiceIntegrationTests
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DatabaseService> _logger;

        public DatabaseServiceIntegrationTests()
        {
            // Create a test configuration with in-memory database
            var inMemorySettings = new Dictionary<string, string>
            {
                {"ConnectionStrings:DefaultConnection", "Data Source=:memory:"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<DatabaseService>();
        }

        [Fact]
        public async Task InitializeDatabaseAsync_CreatesTablesSuccessfully()
        {
            // Arrange
            var databaseService = new DatabaseService(_configuration, _logger);

            // Act
            await databaseService.InitializeDatabaseAsync();

            // Assert
            var isHealthy = await databaseService.IsHealthy();
            isHealthy.Should().BeTrue();
        }

        [Fact]
        public async Task GetCountsFromEmptyDatabase_ShouldReturnZero()
        {
            // Arrange
            var databaseService = new DatabaseService(_configuration, _logger);
            await databaseService.InitializeDatabaseAsync();

            // Act
            var weighInsCount = await databaseService.GetWeighInsCountAsync();
            var runsCount = await databaseService.GetRunsCountAsync();

            // Assert
            weighInsCount.Should().Be(0);
            runsCount.Should().Be(0);
        }

        [Fact]
        public async Task GetLastDatesFromEmptyDatabase_ShouldReturnNull()
        {
            // Arrange
            var databaseService = new DatabaseService(_configuration, _logger);
            await databaseService.InitializeDatabaseAsync();

            // Act
            var lastWeighInDate = await databaseService.GetLastWeighInDateAsync();
            var lastRunDate = await databaseService.GetLastRunDateAsync();

            // Assert
            lastWeighInDate.Should().BeNull();
            lastRunDate.Should().BeNull();
        }
    }
}
