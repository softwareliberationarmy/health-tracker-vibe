using System.Net.Http.Json;
using HealthTracker.Shared.Models;

namespace HealthTracker.Cli.Services;

/// <summary>
/// Implementation of the API client using HttpClient
/// </summary>
public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc/>
    public async Task<AboutResponse?> GetAboutAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<AboutResponse>("/about");
        }
        catch (Exception)
        {
            // Return null when API is unreachable or any other error occurs
            return null;
        }
    }
}
