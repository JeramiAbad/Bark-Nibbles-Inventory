using System.ComponentModel.DataAnnotations;

namespace Barks.Models;

public class AdminAccount
{
    public int AdminAccountId { get; set; } = 1;

    [Required]
    [MaxLength(32)]
    public string Username { get; set; } = "";

    [Required]
    public string PasswordHash { get; set; } = "";
}
