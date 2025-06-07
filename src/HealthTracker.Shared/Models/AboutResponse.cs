namespace HealthTracker.Shared.Models;

public class AboutResponse
{
    public string ApiVersion { get; set; } = string.Empty;
    public int WeighInsCount { get; set; }
    public int RunsCount { get; set; }
    public DateTime? LastWeighInDate { get; set; }
    public DateTime? LastRunDate { get; set; }
}
