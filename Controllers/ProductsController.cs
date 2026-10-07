using Barks.Data;
using Barks.Models;
using Barks.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Barks.Controllers;

public class ProductsController : Controller
{
    private readonly InventoryDbContext _context;

    public ProductsController(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? search, bool lowStock = false)
    {
        var query = _context.Products.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(product =>
                (product.ItemCode != null && product.ItemCode.Contains(search)) ||
                product.ProductName.Contains(search) ||
                (product.Flavor != null && product.Flavor.Contains(search)) ||
                (product.Size != null && product.Size.Contains(search)) ||
                product.Category.Contains(search) ||
                product.Supplier.Contains(search));
        }

        if (lowStock)
        {
            query = query.Where(product =>
                product.MinimumStock > 0 && product.Quantity <= product.MinimumStock);
        }

        return View(new ProductListViewModel
        {
            Products = await query
                .OrderBy(product => product.ProductName)
                .ToListAsync(),
            Search = search?.Trim() ?? "",
            LowStockOnly = lowStock
        });
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(product => product.ProductId == id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("ItemCode,ProductName,Flavor,Size,Category,Unit,Price,Supplier,MinimumStock")] Product product)
    {
        NormalizeItemCode(product);
        if (await ItemCodeExistsAsync(product.ItemCode))
        {
            ModelState.AddModelError(nameof(product.ItemCode), "That item code is already in use.");
        }

        if (!ModelState.IsValid)
        {
            return View(product);
        }

        product.Quantity = 0;
        _context.Add(product);
        await _context.SaveChangesAsync();
        TempData["Success"] = $"{product.ProductName} was added. Record a stock adjustment to set its quantity.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id, [Bind("ProductId,ItemCode,ProductName,Flavor,Size,Category,Unit,Price,Supplier,MinimumStock")] Product product)
    {
        if (id != product.ProductId)
        {
            return NotFound();
        }

        NormalizeItemCode(product);
        if (await ItemCodeExistsAsync(product.ItemCode, id))
        {
            ModelState.AddModelError(nameof(product.ItemCode), "That item code is already in use.");
        }

        if (!ModelState.IsValid)
        {
            return View(product);
        }

        var existingProduct = await _context.Products.FindAsync(id);
        if (existingProduct == null)
        {
            return NotFound();
        }

        existingProduct.ProductName = product.ProductName;
        existingProduct.ItemCode = product.ItemCode;
        existingProduct.Flavor = product.Flavor;
        existingProduct.Size = product.Size;
        existingProduct.Category = product.Category;
        existingProduct.Unit = product.Unit;
        existingProduct.Price = product.Price;
        existingProduct.Supplier = product.Supplier;
        existingProduct.MinimumStock = product.MinimumStock;
        await _context.SaveChangesAsync();
        TempData["Success"] = $"{existingProduct.ProductName} was updated.";

        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> ItemCodeExistsAsync(string? itemCode, int? excludingProductId = null)
    {
        if (string.IsNullOrWhiteSpace(itemCode))
        {
            return false;
        }

        return await _context.Products.AnyAsync(product =>
            product.ItemCode == itemCode &&
            (!excludingProductId.HasValue || product.ProductId != excludingProductId.Value));
    }

    private static void NormalizeItemCode(Product product)
    {
        product.ItemCode = string.IsNullOrWhiteSpace(product.ItemCode)
            ? null
            : product.ItemCode.Trim();
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(product => product.ProductId == id);
        if (product == null)
        {
            return NotFound();
        }

        ViewBag.HasStockHistory = await _context.Transactions
            .AnyAsync(transaction => transaction.ProductId == product.ProductId);
        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            if (await _context.Transactions.AnyAsync(transaction => transaction.ProductId == product.ProductId))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "This product has stock activity and cannot be deleted without removing its audit history.");
                ViewBag.HasStockHistory = true;
                return View(nameof(Delete), product);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

}
