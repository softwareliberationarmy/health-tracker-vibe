using System.Reflection;
using HealthTracker.Shared.Models;

namespace HealthTracker.Api.Services;

public class HealthService : IHealthService
{
    /// <summary>
    /// Gets information about the API and health statistics
    /// </summary>
    /// <returns>About information including version and statistics</returns>
    public Task<AboutResponse> GetAboutInfoAsync()
    {
        // For now, return mock data; this will be updated when we add database functionality
        var apiVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";

        var response = new AboutResponse
        {
            ApiVersion = apiVersion,
            WeighInsCount = 0,
            RunsCount = 0,
            LastWeighInDate = null,
            LastRunDate = null
        };

        return Task.FromResult(response);
    }
}
