using HealthTracker.Shared.Models;
using FluentAssertions;

namespace HealthTracker.Tests.Unit.Models;

public class AboutResponseTests
{
    [Fact]
    public void AboutResponse_WhenCreated_ShouldHaveAllProperties()
    {
        // Given
        var apiVersion = "1.0.0";
        var weighInsCount = 10;
        var runsCount = 5;
        var lastWeighInDate = DateTime.Today.AddDays(-1);
        var lastRunDate = DateTime.Today.AddDays(-2);

        // When
        var response = new AboutResponse
        {
            ApiVersion = apiVersion,
            WeighInsCount = weighInsCount,
            RunsCount = runsCount,
            LastWeighInDate = lastWeighInDate,
            LastRunDate = lastRunDate
        };

        // Then
        response.ApiVersion.Should().Be(apiVersion);
        response.WeighInsCount.Should().Be(weighInsCount);
        response.RunsCount.Should().Be(runsCount);
        response.LastWeighInDate.Should().Be(lastWeighInDate);
        response.LastRunDate.Should().Be(lastRunDate);
    }

    [Fact]
    public void AboutResponse_WhenCreatedWithNullDates_ShouldAllowNullValues()
    {
        // Given
        var response = new AboutResponse
        {
            ApiVersion = "1.0.0",
            WeighInsCount = 0,
            RunsCount = 0,
            LastWeighInDate = null,
            LastRunDate = null
        };

        // When & Then
        response.LastWeighInDate.Should().BeNull();
        response.LastRunDate.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    public void AboutResponse_WeighInsCount_ShouldAcceptNonNegativeValues(int count)
    {
        // Given
        var response = new AboutResponse();

        // When
        response.WeighInsCount = count;

        // Then
        response.WeighInsCount.Should().Be(count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    public void AboutResponse_RunsCount_ShouldAcceptNonNegativeValues(int count)
    {
        // Given
        var response = new AboutResponse();

        // When
        response.RunsCount = count;

        // Then
        response.RunsCount.Should().Be(count);
    }
}
