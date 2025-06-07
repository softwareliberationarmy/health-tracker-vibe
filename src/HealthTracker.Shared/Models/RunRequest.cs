using System.ComponentModel.DataAnnotations;

namespace HealthTracker.Shared.Models;

public class RunRequest
{
    [Required(ErrorMessage = "Distance is required")]
    [Range(0.1, 9999.9, ErrorMessage = "Distance must be greater than 0")]
    public decimal Distance { get; set; }

    [Required(ErrorMessage = "Duration is required")]
    [ValidatePositiveTimeSpan(ErrorMessage = "Duration must be greater than zero")]
    public TimeSpan Duration { get; set; }
    [Required(ErrorMessage = "Date is required")]
    [DataType(DataType.Date)]
    [ValidateNotFutureDate(ErrorMessage = "Date cannot be in the future")]
    public DateTime? Date { get; set; }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class ValidatePositiveTimeSpanAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is TimeSpan timeSpan)
        {
            return timeSpan > TimeSpan.Zero;
        }
        return true;
    }
}
