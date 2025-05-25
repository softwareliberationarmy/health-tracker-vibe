namespace HealthTracker.Core.Models;

/// <summary>
/// Represents a weigh-in record with body composition data
/// </summary>
public class WeighIn
{
    /// <summary>
    /// Unique identifier for the weigh-in record
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Date of the weigh-in
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Body weight in pounds
    /// </summary>
    public double Weight { get; set; }

    /// <summary>
    /// Body Mass Index
    /// </summary>
    public double Bmi { get; set; }

    /// <summary>
    /// Body fat percentage (0-100)
    /// </summary>
    public double Fat { get; set; }

    /// <summary>
    /// Muscle mass percentage (0-100)
    /// </summary>
    public double Muscle { get; set; }

    /// <summary>
    /// Resting metabolic rate in kcal
    /// </summary>
    public int RestingMetab { get; set; }

    /// <summary>
    /// Visceral fat level (10-30)
    /// </summary>
    public int VisceralFat { get; set; }
}
