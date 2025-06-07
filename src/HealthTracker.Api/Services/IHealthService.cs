using HealthTracker.Shared.Models;

namespace HealthTracker.Api.Services;

public interface IHealthService
{
    /// <summary>
    /// Gets information about the API and health statistics
    /// </summary>
    /// <returns>About information including version and statistics</returns>
    Task<AboutResponse> GetAboutInfoAsync();
}
