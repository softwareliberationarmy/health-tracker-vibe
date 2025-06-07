using System;
using System.Data;
using System.Threading.Tasks;
using FluentAssertions;
using HealthTracker.Api.Services;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HealthTracker.Tests.Unit.Services
{
    public class DatabaseServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<DatabaseService>> _mockLogger;
        private readonly string _dbPath;

        public DatabaseServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<DatabaseService>>();
            _dbPath = "Data Source=:memory:";

            var mockConfigSection = new Mock<IConfigurationSection>();
            mockConfigSection.Setup(x => x.Value).Returns(_dbPath);
            _mockConfiguration.Setup(x => x.GetSection("ConnectionStrings:DefaultConnection"))
                .Returns(mockConfigSection.Object);
        }

        [Fact]
        public async Task InitializeDatabaseAsync_ShouldCreateTables()
        {
            // Arrange
            using var connection = new SqliteConnection(_dbPath);
            connection.Open();

            var sut = new DatabaseService(_mockConfiguration.Object, _mockLogger.Object);

            // Act
            await sut.InitializeDatabaseAsync();

            // Assert
            // Check if tables exist by executing a query
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND (name='weighins' OR name='runs')";

            using var reader = await cmd.ExecuteReaderAsync();
            var tableCount = 0;
            while (await reader.ReadAsync())
            {
                tableCount++;
            }

            tableCount.Should().Be(2);
        }

        [Fact]
        public async Task GetConnectionAsync_ShouldReturnOpenConnection()
        {
            // Arrange
            var sut = new DatabaseService(_mockConfiguration.Object, _mockLogger.Object);

            // Act
            using var connection = await sut.GetConnectionAsync();

            // Assert
            connection.Should().NotBeNull();
            connection.State.Should().Be(ConnectionState.Open);
        }

        [Fact]
        public async Task IsHealthy_ShouldReturnTrue_WhenDatabaseIsAvailable()
        {
            // Arrange
            var sut = new DatabaseService(_mockConfiguration.Object, _mockLogger.Object);

            // Act
            var result = await sut.IsHealthy();

            // Assert
            result.Should().BeTrue();
        }
    }
}
