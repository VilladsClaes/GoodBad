using GoodBad.Web.Data;
using GoodBad.Web.Models;
using GoodBad.Web.Services;
using GoodBad.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodBad.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
public class UsersController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? q)
    {
        var query = _db.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(u => (u.Email != null && u.Email.Contains(q)) || (u.DisplayName != null && u.DisplayName.Contains(q)));
        }

        var users = await query.OrderByDescending(u => u.CreatedAt).Take(200).ToListAsync();
        var admins = (await _userManager.GetUsersInRoleAsync(Roles.Admin)).Select(u => u.Id).ToHashSet();

        var reviewCounts = await _db.Reviews
            .Where(r => r.UserId != null)
            .GroupBy(r => r.UserId!)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.UserId, x => x.Count);

        ViewBag.Query = q;

        return View(users.Select(u => new AdminUserRow
        {
            Id = u.Id,
            DisplayName = u.DisplayName,
            Email = u.Email,
            IsAdmin = admins.Contains(u.Id),
            ReviewCount = reviewCounts.TryGetValue(u.Id, out var count) ? count : 0,
            CreatedAt = u.CreatedAt
        }).ToList());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAdmin(string id, string? returnUrl)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        var isAdmin = await _userManager.IsInRoleAsync(user, Roles.Admin);
        if (isAdmin)
        {
            var admins = await _userManager.GetUsersInRoleAsync(Roles.Admin);
            if (admins.Count <= 1)
            {
                TempData["Error"] = "Der skal være mindst én administrator.";
                return SafeRedirect(returnUrl);
            }

            await _userManager.RemoveFromRoleAsync(user, Roles.Admin);
            TempData["Success"] = $"{user.Email} er ikke længere administrator.";
        }
        else
        {
            await _userManager.AddToRoleAsync(user, Roles.Admin);
            TempData["Success"] = $"{user.Email} er nu administrator.";
        }

        return SafeRedirect(returnUrl);
    }

    private IActionResult SafeRedirect(string? returnUrl)
        => !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction(nameof(Index));
}
