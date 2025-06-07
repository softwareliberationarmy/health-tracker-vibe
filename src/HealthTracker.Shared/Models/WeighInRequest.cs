using System.ComponentModel.DataAnnotations;

namespace HealthTracker.Shared.Models;

public class WeighInRequest
{
    [Required(ErrorMessage = "Weight is required")]
    [Range(0.1, 9999.9, ErrorMessage = "Weight must be greater than 0")]
    public decimal Weight { get; set; }
    [Required(ErrorMessage = "Date is required")]
    [DataType(DataType.Date)]
    [ValidateNotFutureDate(ErrorMessage = "Date cannot be in the future")]
    public DateTime? Date { get; set; }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class ValidateNotFutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is DateTime date)
        {
            return date.Date <= DateTime.Today;
        }
        return true;
    }
}
