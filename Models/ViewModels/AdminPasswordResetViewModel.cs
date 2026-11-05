using System.ComponentModel.DataAnnotations;

namespace Barks.Models.ViewModels;

public class AdminPasswordResetViewModel
{
    [Required]
    [StringLength(32)]
    public string Username { get; set; } = "";

    [Required]
    [StringLength(128, MinimumLength = 12, ErrorMessage = "Use at least 12 characters.")]
    [DataType(DataType.Password)]
    [Display(Name = "New password")]
    public string Password { get; set; } = "";

    [Required]
    [Compare(nameof(Password), ErrorMessage = "The passwords do not match.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm new password")]
    public string ConfirmPassword { get; set; } = "";
}
