using Barks.Data;
using Barks.Models;
using Barks.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Barks.Controllers;

public class ProductsController : Controller
{
    private const long MaxImageSize = 5 * 1024 * 1024;
    private readonly IWebHostEnvironment _environment;
    private readonly InventoryDbContext _context;

    public ProductsController(InventoryDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
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
    [RequestSizeLimit(MaxImageSize + 64 * 1024)]
    public async Task<IActionResult> Create(
        [Bind("ItemCode,ProductName,Flavor,Size,Category,Unit,Price,Supplier,MinimumStock")] Product product,
        IFormFile? image)
    {
        NormalizeItemCode(product);
        if (await ItemCodeExistsAsync(product.ItemCode))
        {
            ModelState.AddModelError(nameof(product.ItemCode), "That item code is already in use.");
        }

        if (image is { Length: > 0 } && !await IsSupportedImageAsync(image))
        {
            ModelState.AddModelError("Image", "Upload a JPEG, PNG, or WebP image no larger than 5 MB.");
        }

        if (!ModelState.IsValid)
        {
            return View(product);
        }

        product.Quantity = 0;
        if (image is { Length: > 0 })
        {
            product.ImagePath = await SaveImageAsync(image);
        }

        _context.Add(product);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            DeleteImageFile(product.ImagePath);
            throw;
        }

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
    [RequestSizeLimit(MaxImageSize + 64 * 1024)]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("ProductId,ItemCode,ProductName,Flavor,Size,Category,Unit,Price,Supplier,MinimumStock")] Product product,
        IFormFile? image)
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

        if (image is { Length: > 0 } && !await IsSupportedImageAsync(image))
        {
            ModelState.AddModelError("Image", "Upload a JPEG, PNG, or WebP image no larger than 5 MB.");
        }

        if (!ModelState.IsValid)
        {
            product.ImagePath = await _context.Products
                .AsNoTracking()
                .Where(existing => existing.ProductId == id)
                .Select(existing => existing.ImagePath)
                .SingleOrDefaultAsync();
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
        var oldImagePath = existingProduct.ImagePath;
        if (image is { Length: > 0 })
        {
            existingProduct.ImagePath = await SaveImageAsync(image);
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            if (image is { Length: > 0 })
            {
                DeleteImageFile(existingProduct.ImagePath);
            }

            throw;
        }

        if (image is { Length: > 0 })
        {
            DeleteImageFile(oldImagePath);
        }
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

    private static async Task<bool> IsSupportedImageAsync(IFormFile image)
    {
        if (image.Length > MaxImageSize)
        {
            return false;
        }

        var header = new byte[12];
        await using var stream = image.OpenReadStream();
        var bytesRead = await stream.ReadAtLeastAsync(header, header.Length, throwOnEndOfStream: false);
        return HasJpegSignature(header, bytesRead)
            || HasPngSignature(header, bytesRead)
            || HasWebpSignature(header, bytesRead);
    }

    private async Task<string> SaveImageAsync(IFormFile image)
    {
        var header = new byte[12];
        await using (var stream = image.OpenReadStream())
        {
            var bytesRead = await stream.ReadAtLeastAsync(header, header.Length, throwOnEndOfStream: false);
            stream.Position = 0;
            var extension = GetImageExtension(header, bytesRead)
                ?? throw new InvalidOperationException("The uploaded image format is not supported.");
            var fileName = $"{Guid.NewGuid():N}{extension}";
            var directory = Path.Combine(_environment.WebRootPath, "uploads", "products");
            Directory.CreateDirectory(directory);
            var filePath = Path.Combine(directory, fileName);
            var saved = false;
            try
            {
                await using (var output = System.IO.File.Create(filePath))
                {
                    await stream.CopyToAsync(output);
                }

                saved = true;
            }
            finally
            {
                if (!saved && System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            return $"/uploads/products/{fileName}";
        }
    }

    private static string? GetImageExtension(byte[] header, int bytesRead)
    {
        if (HasJpegSignature(header, bytesRead))
        {
            return ".jpg";
        }

        if (HasPngSignature(header, bytesRead))
        {
            return ".png";
        }

        return HasWebpSignature(header, bytesRead) ? ".webp" : null;
    }

    private static bool HasJpegSignature(byte[] header, int bytesRead) =>
        bytesRead >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF;

    private static bool HasPngSignature(byte[] header, int bytesRead) =>
        bytesRead >= 8
        && header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47
        && header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A;

    private static bool HasWebpSignature(byte[] header, int bytesRead) =>
        bytesRead >= 12
        && header[0] == (byte)'R' && header[1] == (byte)'I' && header[2] == (byte)'F' && header[3] == (byte)'F'
        && header[8] == (byte)'W' && header[9] == (byte)'E' && header[10] == (byte)'B' && header[11] == (byte)'P';

    private void DeleteImageFile(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath) || !imagePath.StartsWith("/uploads/products/", StringComparison.Ordinal))
        {
            return;
        }

        var fileName = Path.GetFileName(imagePath);
        var filePath = Path.Combine(_environment.WebRootPath, "uploads", "products", fileName);
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
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

            var imagePath = product.ImagePath;
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            DeleteImageFile(imagePath);
        }

        return RedirectToAction(nameof(Index));
    }

}
