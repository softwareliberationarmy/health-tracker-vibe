namespace HealthTracker.Core.Models;

/// <summary>
/// Represents a running activity record
/// </summary>
public class Run
{
    /// <summary>
    /// Unique identifier for the run record
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Date of the run
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Distance covered in the run
    /// </summary>
    public double Distance { get; set; }

    /// <summary>
    /// Unit of measurement for the distance (e.g., "mi", "km")
    /// </summary>
    public string DistanceUnit { get; set; } = string.Empty;

    /// <summary>
    /// Duration of the run in seconds
    /// </summary>
    public int TimeInSeconds { get; set; }
}
