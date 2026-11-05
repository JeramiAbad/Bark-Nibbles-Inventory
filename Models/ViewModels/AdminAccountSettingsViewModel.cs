using System.ComponentModel.DataAnnotations;

namespace Barks.Models.ViewModels;

public class AdminAccountSettingsViewModel : IValidatableObject
{
    [Required]
    [StringLength(32, MinimumLength = 3)]
    [Display(Name = "Admin username")]
    public string Username { get; set; } = "";

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Current password")]
    public string CurrentPassword { get; set; } = "";

    [StringLength(128)]
    [DataType(DataType.Password)]
    [Display(Name = "New password (optional)")]
    public string? NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm new password")]
    public string? ConfirmPassword { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrEmpty(NewPassword) && NewPassword.Length < 12)
        {
            yield return new ValidationResult(
                "Use at least 12 characters for a new password.",
                [nameof(NewPassword)]);
        }

        if (!string.Equals(NewPassword, ConfirmPassword, StringComparison.Ordinal))
        {
            yield return new ValidationResult(
                "The new passwords do not match.",
                [nameof(ConfirmPassword)]);
        }
    }
}
