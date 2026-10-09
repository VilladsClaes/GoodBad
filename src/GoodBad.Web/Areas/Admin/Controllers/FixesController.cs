using GoodBad.Web.Data;
using GoodBad.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodBad.Web.Areas.Admin.Controllers;

/// <summary>Moderation of community fixes and system suggestions.</summary>
[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
public class FixesController : Controller
{
    private const int PageSize = 30;

    private readonly AppDbContext _db;

    public FixesController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string? q, string? filter, int page = 1)
    {
        var query = _db.Fixes
            .Include(f => f.Product)
            .Include(f => f.User)
            .Include(f => f.Votes)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(f => f.Title.Contains(q) || f.Body.Contains(q) || f.Product.Name.Contains(q));
        }

        if (filter == "hidden")
        {
            query = query.Where(f => f.IsHidden);
        }
        else if (filter == "system")
        {
            query = query.Where(f => f.Source == Models.FixSource.System);
        }
        else if (filter == "user")
        {
            query = query.Where(f => f.Source == Models.FixSource.User);
        }

        var total = await query.CountAsync();
        page = Math.Max(1, page);

        ViewBag.Query = q;
        ViewBag.Filter = filter;
        ViewBag.Page = page;
        ViewBag.Total = total;
        ViewBag.TotalPages = (int)Math.Ceiling(total / (double)PageSize);

        return View(await query
            .OrderByDescending(f => f.CreatedAt)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetHidden(int id, bool value, string? returnUrl)
    {
        var fix = await _db.Fixes.FindAsync(id);
        if (fix is null)
        {
            return NotFound();
        }

        fix.IsHidden = value;
        await _db.SaveChangesAsync();
        TempData["Success"] = value ? "Forslaget er skjult." : "Forslaget er synligt igen.";
        return SafeRedirect(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var fix = await _db.Fixes.FindAsync(id);
        if (fix is null)
        {
            return NotFound();
        }

        _db.Fixes.Remove(fix);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Forslaget er slettet.";
        return RedirectToAction(nameof(Index));
    }

    private IActionResult SafeRedirect(string? returnUrl)
        => !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction(nameof(Index));
}
