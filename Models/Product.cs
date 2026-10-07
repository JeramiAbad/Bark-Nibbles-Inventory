using System.ComponentModel.DataAnnotations;

namespace Barks.Models;

public class Product
{
    public int ProductId { get; set; }

    [MaxLength(32)]
    public string? ItemCode { get; set; }

    [Required]
    public string ProductName { get; set; } = "";

    [MaxLength(120)]
    public string? Flavor { get; set; }

    [MaxLength(64)]
    public string? Size { get; set; }

    public string Category { get; set; } = "";

    [MaxLength(32)]
    public string Unit { get; set; } = "pack";

    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
    public int Quantity { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Minimum stock cannot be negative.")]
    public int MinimumStock { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Price cannot be negative.")]
    public decimal Price { get; set; }

    public string Supplier { get; set; } = "";
}
