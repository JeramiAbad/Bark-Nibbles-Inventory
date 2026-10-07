using System.Net;
using System.Security.Claims;
using Barks.Data;
using Barks.Models;
using Barks.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace Barks.Controllers;

public class AccountController : Controller
{
    private readonly InventoryDbContext _context;
    private readonly IPasswordHasher<AdminAccount> _passwordHasher;

    public AccountController(
        InventoryDbContext context,
        IPasswordHasher<AdminAccount> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Setup()
    {
        if (await AdminAccountExistsAsync())
        {
            return RedirectToAction(nameof(Login));
        }

        if (!IsLocalRequest())
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        return View(new AdminSetupViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Setup(AdminSetupViewModel model)
    {
        if (await AdminAccountExistsAsync())
        {
            return RedirectToAction(nameof(Login));
        }

        if (!IsLocalRequest())
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        model.Username = model.Username?.Trim() ?? string.Empty;
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var admin = new AdminAccount
        {
            AdminAccountId = 1,
            Username = model.Username
        };
        admin.PasswordHash = _passwordHasher.HashPassword(admin, model.Password);
        _context.AdminAccounts.Add(admin);
        await _context.SaveChangesAsync();

        await SignInAdminAsync(admin);
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public async Task<IActionResult> Login(string? returnUrl = null)
    {
        if (!await AdminAccountExistsAsync())
        {
            return RedirectToAction(nameof(Setup));
        }

        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new AdminLoginViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("admin-login")]
    public async Task<IActionResult> Login(AdminLoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var username = model.Username.Trim();
        var admin = await _context.AdminAccounts
            .SingleOrDefaultAsync(account => account.Username == username);
        if (admin == null)
        {
            ModelState.AddModelError(string.Empty, "The username or password is incorrect.");
            return View(model);
        }

        var verificationResult =
            _passwordHasher.VerifyHashedPassword(admin, admin.PasswordHash, model.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "The username or password is incorrect.");
            return View(model);
        }

        if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            admin.PasswordHash = _passwordHasher.HashPassword(admin, model.Password);
            await _context.SaveChangesAsync();
        }

        await SignInAdminAsync(admin);
        if (Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl!);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    private Task<bool> AdminAccountExistsAsync() =>
        _context.AdminAccounts.AnyAsync(account => account.AdminAccountId == 1);

    private bool IsLocalRequest()
    {
        var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
        if (remoteIpAddress?.IsIPv4MappedToIPv6 == true)
        {
            remoteIpAddress = remoteIpAddress.MapToIPv4();
        }

        return remoteIpAddress != null && IPAddress.IsLoopback(remoteIpAddress);
    }

    private async Task SignInAdminAsync(AdminAccount admin)
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, admin.Username),
            new Claim(ClaimTypes.Role, "Admin")
        ],
        CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true
            });
    }
}
