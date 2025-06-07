using System.Reflection;
using HealthTracker.Shared.Models;

namespace HealthTracker.Api.Services;

public class HealthService : IHealthService
{
    private readonly IDatabaseService _databaseService;

    public HealthService(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    /// <summary>
    /// Gets information about the API and health statistics
    /// </summary>
    /// <returns>About information including version and statistics</returns>
    public async Task<AboutResponse> GetAboutInfoAsync()
    {
        var apiVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";

        var response = new AboutResponse
        {
            ApiVersion = apiVersion,
            WeighInsCount = await _databaseService.GetWeighInsCountAsync(),
            RunsCount = await _databaseService.GetRunsCountAsync(),
            LastWeighInDate = await _databaseService.GetLastWeighInDateAsync(),
            LastRunDate = await _databaseService.GetLastRunDateAsync()
        };

        return response;
    }
}
