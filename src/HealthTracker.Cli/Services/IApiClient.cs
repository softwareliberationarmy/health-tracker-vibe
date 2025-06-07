using HealthTracker.Shared.Models;

namespace HealthTracker.Cli.Services;

/// <summary>
/// Interface for API client to interact with Health Tracker API
/// </summary>
public interface IApiClient
{
    /// <summary>
    /// Gets information about the API version and statistics
    /// </summary>
    /// <returns>AboutResponse with API info, or null if API is unreachable</returns>
    Task<AboutResponse?> GetAboutAsync();
}
