using System.ComponentModel.DataAnnotations;

namespace Barks.Models.ViewModels;

public class AdminLoginViewModel
{
    [Required]
    [StringLength(32)]
    public string Username { get; set; } = "";

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";
}
