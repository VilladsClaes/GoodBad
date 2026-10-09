using GoodBad.Web.Data;
using GoodBad.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodBad.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
public class ReviewsController : Controller
{
    private const int PageSize = 30;

    private readonly AppDbContext _db;
    private readonly ImageStorageService _images;

    public ReviewsController(AppDbContext db, ImageStorageService images)
    {
        _db = db;
        _images = images;
    }

    public async Task<IActionResult> Index(string? q, string? filter, int page = 1)
    {
        var query = _db.Reviews.Include(r => r.Product).Include(r => r.User).AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(r => r.Title.Contains(q) || r.Body.Contains(q) || r.Product.Name.Contains(q));
        }

        query = filter switch
        {
            "hidden" => query.Where(r => r.IsHidden),
            "bad" => query.Where(r => r.Verdict == Models.Verdict.Bad),
            "good" => query.Where(r => r.Verdict == Models.Verdict.Good),
            _ => query
        };

        var total = await query.CountAsync();
        page = Math.Max(1, page);

        ViewBag.Query = q;
        ViewBag.Filter = filter;
        ViewBag.Page = page;
        ViewBag.Total = total;
        ViewBag.TotalPages = (int)Math.Ceiling(total / (double)PageSize);

        return View(await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetHidden(int id, bool value, string? returnUrl)
    {
        var review = await _db.Reviews.FindAsync(id);
        if (review is null)
        {
            return NotFound();
        }

        review.IsHidden = value;
        await _db.SaveChangesAsync();
        TempData["Success"] = value ? "Anmeldelsen er skjult." : "Anmeldelsen er synlig igen.";
        return SafeRedirect(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var review = await _db.Reviews.FindAsync(id);
        if (review is null)
        {
            return NotFound();
        }

        _db.Reviews.Remove(review);
        await _db.SaveChangesAsync();

        // Remove an uploaded review photo from disk (external links are ignored).
        _images.Delete(review.ImageUrl);
        TempData["Success"] = "Anmeldelsen er slettet.";
        return RedirectToAction(nameof(Index));
    }

    private IActionResult SafeRedirect(string? returnUrl)
        => !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction(nameof(Index));
}
