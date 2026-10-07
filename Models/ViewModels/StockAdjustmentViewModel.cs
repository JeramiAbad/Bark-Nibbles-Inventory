using System.ComponentModel.DataAnnotations;

namespace Barks.Models.ViewModels;

public class StockAdjustmentViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Choose a product.")]
    [Display(Name = "Product")]
    public int ProductId { get; set; }

    [Required]
    [RegularExpression("^(Stock In|Stock Out)$", ErrorMessage = "Choose Stock In or Stock Out.")]
    [Display(Name = "Adjustment type")]
    public string TransactionType { get; set; } = "Stock In";

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }
}
