using System.ComponentModel.DataAnnotations;

namespace Barks.Models.ViewModels;

public class AdminSetupViewModel
{
    [Required]
    [StringLength(32, MinimumLength = 3)]
    public string Username { get; set; } = "";

    [Required]
    [StringLength(128, MinimumLength = 12, ErrorMessage = "Use at least 12 characters.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required]
    [Compare(nameof(Password), ErrorMessage = "The passwords do not match.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = "";
}
