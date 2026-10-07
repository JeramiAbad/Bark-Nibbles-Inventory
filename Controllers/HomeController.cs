using System.Diagnostics;
using Barks.Data;
using Barks.Models;
using Barks.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Barks.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly InventoryDbContext _context;

    public HomeController(ILogger<HomeController> logger, InventoryDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.AsNoTracking().ToListAsync();
        var recentTransactions = await _context.Transactions
            .AsNoTracking()
            .Include(transaction => transaction.Product)
            .OrderByDescending(transaction => transaction.Date)
            .ThenByDescending(transaction => transaction.TransactionId)
            .Take(6)
            .ToListAsync();
        var lowStockProducts = products
            .Where(product => product.MinimumStock > 0 && product.Quantity <= product.MinimumStock)
            .OrderBy(product => product.Quantity)
            .ThenBy(product => product.ProductName)
            .Take(6)
            .ToList();

        return View(new InventoryDashboardViewModel
        {
            ProductCount = products.Count,
            UnitsOnHand = products.Sum(product => product.Quantity),
            InventoryValue = products.Sum(product => product.Quantity * product.Price),
            LowStockCount = products.Count(product =>
                product.MinimumStock > 0 && product.Quantity <= product.MinimumStock),
            LowStockProducts = lowStockProducts,
            RecentTransactions = recentTransactions
        });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
