using GoodBad.Web.Data;
using GoodBad.Web.Models;
using GoodBad.Web.Services;
using GoodBad.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodBad.Web.Areas.Admin.Controllers;

/// <summary>Maintains the "du skulle have købt dette i stedet" pairings.</summary>
[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
public class RecommendationsController : Controller
{
    private readonly AppDbContext _db;

    public RecommendationsController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        await PopulateAsync();
        return View(await _db.ProductRecommendations
            .Include(r => r.SourceProduct).ThenInclude(p => p.Brand)
            .Include(r => r.AlternativeProduct).ThenInclude(p => p.Brand)
            .OrderBy(r => r.SourceProduct.Name)
            .ToListAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RecommendationEditViewModel model)
    {
        if (model.SourceProductId == model.AlternativeProductId)
        {
            TempData["Error"] = "Produktet kan ikke være sit eget alternativ.";
            return RedirectToAction(nameof(Index));
        }

        var exists = await _db.ProductRecommendations.AnyAsync(r =>
            r.SourceProductId == model.SourceProductId && r.AlternativeProductId == model.AlternativeProductId);

        if (exists)
        {
            TempData["Error"] = "Den anbefaling findes allerede.";
            return RedirectToAction(nameof(Index));
        }

        _db.ProductRecommendations.Add(new ProductRecommendation
        {
            SourceProductId = model.SourceProductId,
            AlternativeProductId = model.AlternativeProductId,
            Reason = model.Reason
        });

        await _db.SaveChangesAsync();
        TempData["Success"] = "Anbefalingen er oprettet.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var recommendation = await _db.ProductRecommendations.FindAsync(id);
        if (recommendation is null)
        {
            return NotFound();
        }

        _db.ProductRecommendations.Remove(recommendation);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Anbefalingen er slettet.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateAsync()
        => ViewBag.Products = await AdminSelectLists.ProductsAsync(_db);
}
