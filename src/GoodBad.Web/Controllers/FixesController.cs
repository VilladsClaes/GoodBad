using GoodBad.Web.Data;
using GoodBad.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodBad.Web.Controllers;

public class FixesController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public FixesController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    /// <summary>A community member proposes a fix for a problem.</summary>
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int productId, int? reviewId, string title, string body, string? sourceUrl)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == productId);
        if (product is null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(body))
        {
            TempData["Error"] = "Et fix skal have både en titel og en forklaring.";
            return RedirectToProduct(product.Slug);
        }

        if (reviewId.HasValue && !await _db.Reviews.AnyAsync(r => r.Id == reviewId && r.ProductId == productId))
        {
            reviewId = null;
        }

        _db.Fixes.Add(new Fix
        {
            ProductId = productId,
            ReviewId = reviewId,
            UserId = _userManager.GetUserId(User),
            Source = FixSource.User,
            Title = title.Trim(),
            Body = body.Trim(),
            SourceUrl = string.IsNullOrWhiteSpace(sourceUrl) ? null : sourceUrl.Trim()
        });

        await _db.SaveChangesAsync();
        TempData["Success"] = "Tak! Dit fix er delt med fællesskabet.";
        return RedirectToProduct(product.Slug);
    }

    /// <summary>"Det fix virker".</summary>
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Vote(int id)
    {
        var fix = await _db.Fixes.Include(f => f.Product).FirstOrDefaultAsync(f => f.Id == id);
        if (fix is null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User)!;
        var existing = await _db.FixVotes.FirstOrDefaultAsync(v => v.FixId == id && v.UserId == userId);
        if (existing is null)
        {
            _db.FixVotes.Add(new FixVote { FixId = id, UserId = userId });
        }
        else
        {
            _db.FixVotes.Remove(existing);
        }

        await _db.SaveChangesAsync();
        return RedirectToProduct(fix.Product.Slug);
    }

    private IActionResult RedirectToProduct(string slug)
        => Redirect($"{Url.Action(nameof(ProductsController.Details), "Products", new { slug })}#fixes");
}
