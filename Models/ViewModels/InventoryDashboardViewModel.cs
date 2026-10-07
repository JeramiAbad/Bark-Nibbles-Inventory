using Barks.Models;

namespace Barks.Models.ViewModels;

public class InventoryDashboardViewModel
{
    public int ProductCount { get; init; }

    public int UnitsOnHand { get; init; }

    public decimal InventoryValue { get; init; }

    public int LowStockCount { get; init; }

    public IReadOnlyList<Product> LowStockProducts { get; init; } = [];

    public IReadOnlyList<Transaction> RecentTransactions { get; init; } = [];
}
