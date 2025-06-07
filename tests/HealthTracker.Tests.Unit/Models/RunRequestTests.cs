using HealthTracker.Shared.Models;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace HealthTracker.Tests.Unit.Models;

public class RunRequestTests
{
    [Fact]
    public void RunRequest_WhenCreatedWithValidData_ShouldHaveAllProperties()
    {
        // Given
        var distance = 5.0m;
        var duration = TimeSpan.FromMinutes(30);
        var date = DateTime.Today;

        // When
        var request = new RunRequest
        {
            Distance = distance,
            Duration = duration,
            Date = date
        };

        // Then
        request.Distance.Should().Be(distance);
        request.Duration.Should().Be(duration);
        request.Date.Should().Be(date);
    }

    [Theory]
    [InlineData(0.1)]
    [InlineData(5.0)]
    [InlineData(42.195)] // Marathon distance
    [InlineData(100.0)]
    public void RunRequest_Distance_ShouldAcceptValidDistances(decimal distance)
    {
        // Given
        var request = new RunRequest
        {
            Duration = TimeSpan.FromMinutes(30),
            Date = DateTime.Today
        };

        // When
        request.Distance = distance;

        // Then
        request.Distance.Should().Be(distance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5.0)]
    public void RunRequest_Distance_ShouldFailValidationForInvalidDistances(decimal distance)
    {
        // Given
        var request = new RunRequest
        {
            Distance = distance,
            Duration = TimeSpan.FromMinutes(30),
            Date = DateTime.Today
        };

        // When
        var validationResults = ValidateModel(request);

        // Then
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(vr => vr.MemberNames.Contains(nameof(RunRequest.Distance)));
    }

    [Theory]
    [InlineData(1)] // 1 minute
    [InlineData(30)] // 30 minutes
    [InlineData(120)] // 2 hours
    [InlineData(300)] // 5 hours
    public void RunRequest_Duration_ShouldAcceptValidDurations(int minutes)
    {
        // Given
        var request = new RunRequest
        {
            Distance = 5.0m,
            Date = DateTime.Today
        };
        var duration = TimeSpan.FromMinutes(minutes);

        // When
        request.Duration = duration;

        // Then
        request.Duration.Should().Be(duration);
    }

    [Theory]
    [InlineData(0)] // 0 minutes
    [InlineData(-1)] // Negative duration
    public void RunRequest_Duration_ShouldFailValidationForInvalidDurations(int minutes)
    {
        // Given
        var request = new RunRequest
        {
            Distance = 5.0m,
            Duration = TimeSpan.FromMinutes(minutes),
            Date = DateTime.Today
        };

        // When
        var validationResults = ValidateModel(request);

        // Then
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(vr => vr.MemberNames.Contains(nameof(RunRequest.Duration)));
    }

    [Fact]
    public void RunRequest_Date_ShouldNotAcceptFutureDate()
    {
        // Given
        var request = new RunRequest
        {
            Distance = 5.0m,
            Duration = TimeSpan.FromMinutes(30),
            Date = DateTime.Today.AddDays(1)
        };

        // When
        var validationResults = ValidateModel(request);

        // Then
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(vr => vr.MemberNames.Contains(nameof(RunRequest.Date)));
    }

    [Fact]
    public void RunRequest_Date_ShouldAcceptTodayAndPastDates()
    {
        // Given
        var requests = new[]
        {
            new RunRequest { Distance = 5.0m, Duration = TimeSpan.FromMinutes(30), Date = DateTime.Today },
            new RunRequest { Distance = 5.0m, Duration = TimeSpan.FromMinutes(30), Date = DateTime.Today.AddDays(-1) },
            new RunRequest { Distance = 5.0m, Duration = TimeSpan.FromMinutes(30), Date = DateTime.Today.AddYears(-1) }
        };

        // When & Then
        foreach (var request in requests)
        {
            var validationResults = ValidateModel(request);
            validationResults.Where(vr => vr.MemberNames.Contains(nameof(RunRequest.Date)))
                .Should().BeEmpty($"Date {request.Date} should be valid");
        }
    }

    [Fact]
    public void RunRequest_RequiredFields_ShouldFailValidationWhenMissing()
    {
        // Given
        var request = new RunRequest();

        // When
        var validationResults = ValidateModel(request);

        // Then
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(vr => vr.MemberNames.Contains(nameof(RunRequest.Distance)));
        validationResults.Should().Contain(vr => vr.MemberNames.Contains(nameof(RunRequest.Duration)));
        validationResults.Should().Contain(vr => vr.MemberNames.Contains(nameof(RunRequest.Date)));
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}
