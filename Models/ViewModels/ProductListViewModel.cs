using Barks.Models;

namespace Barks.Models.ViewModels;

public class ProductListViewModel
{
    public IReadOnlyList<Product> Products { get; init; } = [];

    public string Search { get; init; } = "";

    public bool LowStockOnly { get; init; }
}
