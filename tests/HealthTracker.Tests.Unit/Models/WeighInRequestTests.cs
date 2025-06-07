using HealthTracker.Shared.Models;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace HealthTracker.Tests.Unit.Models;

public class WeighInRequestTests
{
    [Fact]
    public void WeighInRequest_WhenCreatedWithValidData_ShouldHaveAllProperties()
    {
        // Given
        var weight = 75.5m;
        var date = DateTime.Today;

        // When
        var request = new WeighInRequest
        {
            Weight = weight,
            Date = date
        };

        // Then
        request.Weight.Should().Be(weight);
        request.Date.Should().Be(date);
    }

    [Theory]
    [InlineData(0.1)]
    [InlineData(50.5)]
    [InlineData(200.0)]
    [InlineData(999.9)]
    public void WeighInRequest_Weight_ShouldAcceptValidWeights(decimal weight)
    {
        // Given
        var request = new WeighInRequest { Date = DateTime.Today };

        // When
        request.Weight = weight;

        // Then
        request.Weight.Should().Be(weight);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-50.5)]
    public void WeighInRequest_Weight_ShouldFailValidationForInvalidWeights(decimal weight)
    {
        // Given
        var request = new WeighInRequest
        {
            Weight = weight,
            Date = DateTime.Today
        };

        // When
        var validationResults = ValidateModel(request);

        // Then
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(vr => vr.MemberNames.Contains(nameof(WeighInRequest.Weight)));
    }

    [Fact]
    public void WeighInRequest_Date_ShouldNotAcceptFutureDate()
    {
        // Given
        var request = new WeighInRequest
        {
            Weight = 75.0m,
            Date = DateTime.Today.AddDays(1)
        };

        // When
        var validationResults = ValidateModel(request);

        // Then
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(vr => vr.MemberNames.Contains(nameof(WeighInRequest.Date)));
    }

    [Fact]
    public void WeighInRequest_Date_ShouldAcceptTodayAndPastDates()
    {
        // Given
        var requests = new[]
        {
            new WeighInRequest { Weight = 75.0m, Date = DateTime.Today },
            new WeighInRequest { Weight = 75.0m, Date = DateTime.Today.AddDays(-1) },
            new WeighInRequest { Weight = 75.0m, Date = DateTime.Today.AddYears(-1) }
        };

        // When & Then
        foreach (var request in requests)
        {
            var validationResults = ValidateModel(request);
            validationResults.Where(vr => vr.MemberNames.Contains(nameof(WeighInRequest.Date)))
                .Should().BeEmpty($"Date {request.Date} should be valid");
        }
    }

    [Fact]
    public void WeighInRequest_RequiredFields_ShouldFailValidationWhenMissing()
    {
        // Given
        var request = new WeighInRequest();

        // When
        var validationResults = ValidateModel(request);

        // Then
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(vr => vr.MemberNames.Contains(nameof(WeighInRequest.Weight)));
        validationResults.Should().Contain(vr => vr.MemberNames.Contains(nameof(WeighInRequest.Date)));
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}
