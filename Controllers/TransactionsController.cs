using Barks.Data;
using Barks.Models;
using Barks.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Barks.Controllers;

public class TransactionsController : Controller
{
    private readonly InventoryDbContext _context;

    public TransactionsController(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var transactions = await _context.Transactions
            .AsNoTracking()
            .Include(transaction => transaction.Product)
            .OrderByDescending(transaction => transaction.Date)
            .ThenByDescending(transaction => transaction.TransactionId)
            .ToListAsync();

        return View(transactions);
    }

    public async Task<IActionResult> Create(int? productId, string? transactionType)
    {
        await PopulateProductsAsync(productId);
        return View(new StockAdjustmentViewModel
        {
            ProductId = productId.GetValueOrDefault(),
            TransactionType = transactionType is "Stock In" or "Stock Out"
                ? transactionType
                : "Stock In"
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StockAdjustmentViewModel adjustment)
    {
        var product = adjustment.ProductId > 0
            ? await _context.Products.FindAsync(adjustment.ProductId)
            : null;

        if (product == null)
        {
            ModelState.AddModelError(nameof(adjustment.ProductId), "Select an existing product.");
        }
        else if (adjustment.TransactionType == "Stock Out" && adjustment.Quantity > product.Quantity)
        {
            ModelState.AddModelError(
                nameof(adjustment.Quantity),
                $"Only {product.Quantity} units of {product.ProductName} are currently in stock.");
        }
        else if (adjustment.TransactionType == "Stock In" &&
                 adjustment.Quantity > int.MaxValue - product.Quantity)
        {
            ModelState.AddModelError(nameof(adjustment.Quantity), "The adjusted quantity is too large.");
        }

        if (!ModelState.IsValid)
        {
            await PopulateProductsAsync(adjustment.ProductId);
            return View(adjustment);
        }

        if (adjustment.TransactionType == "Stock In")
        {
            product!.Quantity += adjustment.Quantity;
        }
        else
        {
            product!.Quantity -= adjustment.Quantity;
        }

        _context.Transactions.Add(new Transaction
        {
            ProductId = product.ProductId,
            TransactionType = adjustment.TransactionType,
            Quantity = adjustment.Quantity,
            Date = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        TempData["Success"] =
            $"{adjustment.TransactionType} recorded: {adjustment.Quantity} × {product.ProductName}.";

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateProductsAsync(int? selectedProductId)
    {
        var products = await _context.Products
            .AsNoTracking()
            .OrderBy(product => product.ProductName)
            .Select(product => new
            {
                product.ProductId,
                DisplayName = $"{product.ProductName} ({product.Quantity} in stock)"
            })
            .ToListAsync();

        ViewBag.HasProducts = products.Count > 0;
        ViewBag.Products = new SelectList(products, "ProductId", "DisplayName", selectedProductId);
    }
}
