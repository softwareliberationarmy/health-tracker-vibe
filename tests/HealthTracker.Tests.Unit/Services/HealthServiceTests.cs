using System;
using System.Threading.Tasks;
using FluentAssertions;
using HealthTracker.Api.Services;
using HealthTracker.Shared.Models;
using Moq;
using Xunit;

namespace HealthTracker.Tests.Unit.Services
{
    public class HealthServiceTests
    {
        private readonly Mock<IDatabaseService> _mockDatabaseService;
        private readonly HealthService _sut;

        public HealthServiceTests()
        {
            _mockDatabaseService = new Mock<IDatabaseService>();
            _sut = new HealthService(_mockDatabaseService.Object);
        }

        [Fact]
        public async Task GetAboutInfoAsync_ShouldReturnDataFromDatabase()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var yesterday = now.AddDays(-1);

            _mockDatabaseService.Setup(x => x.GetWeighInsCountAsync()).ReturnsAsync(5);
            _mockDatabaseService.Setup(x => x.GetRunsCountAsync()).ReturnsAsync(3);
            _mockDatabaseService.Setup(x => x.GetLastWeighInDateAsync()).ReturnsAsync(now);
            _mockDatabaseService.Setup(x => x.GetLastRunDateAsync()).ReturnsAsync(yesterday);

            // Act
            var result = await _sut.GetAboutInfoAsync();

            // Assert
            result.Should().NotBeNull();
            result.WeighInsCount.Should().Be(5);
            result.RunsCount.Should().Be(3);
            result.LastWeighInDate.Should().Be(now);
            result.LastRunDate.Should().Be(yesterday);

            // Verify all methods were called
            _mockDatabaseService.Verify(x => x.GetWeighInsCountAsync(), Times.Once);
            _mockDatabaseService.Verify(x => x.GetRunsCountAsync(), Times.Once);
            _mockDatabaseService.Verify(x => x.GetLastWeighInDateAsync(), Times.Once);
            _mockDatabaseService.Verify(x => x.GetLastRunDateAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAboutInfoAsync_ShouldHandleNullDates()
        {
            // Arrange
            _mockDatabaseService.Setup(x => x.GetWeighInsCountAsync()).ReturnsAsync(0);
            _mockDatabaseService.Setup(x => x.GetRunsCountAsync()).ReturnsAsync(0);
            _mockDatabaseService.Setup(x => x.GetLastWeighInDateAsync()).ReturnsAsync((DateTime?)null);
            _mockDatabaseService.Setup(x => x.GetLastRunDateAsync()).ReturnsAsync((DateTime?)null);

            // Act
            var result = await _sut.GetAboutInfoAsync();

            // Assert
            result.Should().NotBeNull();
            result.WeighInsCount.Should().Be(0);
            result.RunsCount.Should().Be(0);
            result.LastWeighInDate.Should().BeNull();
            result.LastRunDate.Should().BeNull();
        }
    }
}
