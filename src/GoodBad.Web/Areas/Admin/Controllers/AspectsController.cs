using GoodBad.Web.Data;
using GoodBad.Web.Models;
using GoodBad.Web.Services;
using GoodBad.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodBad.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
public class AspectsController : Controller
{
    private readonly AppDbContext _db;

    public AspectsController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string? q, AspectKind? kind)
    {
        var query = _db.Aspects
            .Include(a => a.Categories).ThenInclude(ca => ca.Category)
            .Include(a => a.Reviews)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(a => a.Text.Contains(q));
        }

        if (kind.HasValue)
        {
            query = query.Where(a => a.Kind == kind.Value);
        }

        ViewBag.Query = q;
        ViewBag.Kind = kind;
        return View(await query.OrderBy(a => a.Kind).ThenBy(a => a.Text).ToListAsync());
    }

    public async Task<IActionResult> Create(AspectKind kind = AspectKind.Problem)
    {
        await PopulateAsync();
        return View("Edit", new AspectEditViewModel { Kind = kind });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AspectEditViewModel model)
    {
        await PopulateAsync();
        if (!ModelState.IsValid)
        {
            return View("Edit", model);
        }

        var aspect = new Aspect
        {
            Text = model.Text.Trim(),
            Slug = await UniqueSlugAsync(model.Text, null),
            Kind = model.Kind,
            Description = model.Description
        };

        _db.Aspects.Add(aspect);
        await _db.SaveChangesAsync();
        await SyncCategoriesAsync(aspect.Id, model.CategoryIds);
        TempData["Success"] = $"Aspektet \"{aspect.Text}\" er oprettet.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var aspect = await _db.Aspects.Include(a => a.Categories).FirstOrDefaultAsync(a => a.Id == id);
        if (aspect is null)
        {
            return NotFound();
        }

        await PopulateAsync();
        return View(new AspectEditViewModel
        {
            Id = aspect.Id,
            Text = aspect.Text,
            Kind = aspect.Kind,
            Description = aspect.Description,
            CategoryIds = aspect.Categories.Select(c => c.CategoryId).ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AspectEditViewModel model)
    {
        var aspect = await _db.Aspects.FirstOrDefaultAsync(a => a.Id == model.Id);
        if (aspect is null)
        {
            return NotFound();
        }

        await PopulateAsync();
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!string.Equals(aspect.Text, model.Text.Trim(), StringComparison.Ordinal))
        {
            aspect.Slug = await UniqueSlugAsync(model.Text, aspect.Id);
        }

        aspect.Text = model.Text.Trim();
        aspect.Kind = model.Kind;
        aspect.Description = model.Description;

        await _db.SaveChangesAsync();
        await SyncCategoriesAsync(aspect.Id, model.CategoryIds);
        TempData["Success"] = "Aspektet er opdateret.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var aspect = await _db.Aspects.FirstOrDefaultAsync(a => a.Id == id);
        if (aspect is null)
        {
            return NotFound();
        }

        _db.CategoryAspects.RemoveRange(_db.CategoryAspects.Where(ca => ca.AspectId == id));
        _db.Aspects.Remove(aspect);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Aspektet er slettet.";
        return RedirectToAction(nameof(Index));
    }

    private async Task SyncCategoriesAsync(int aspectId, List<int> categoryIds)
    {
        var existing = await _db.CategoryAspects.Where(ca => ca.AspectId == aspectId).ToListAsync();
        _db.CategoryAspects.RemoveRange(existing);

        foreach (var categoryId in categoryIds.Distinct())
        {
            _db.CategoryAspects.Add(new CategoryAspect { AspectId = aspectId, CategoryId = categoryId });
        }

        await _db.SaveChangesAsync();
    }

    private async Task PopulateAsync()
    {
        ViewBag.Categories = await AdminSelectLists.CategoriesAsync(_db);
    }

    private async Task<string> UniqueSlugAsync(string text, int? ignoreId)
    {
        var baseSlug = SlugHelper.Slugify(text);
        var slug = baseSlug;
        var suffix = 2;

        while (true)
        {
            var query = _db.Aspects.Where(a => a.Slug == slug);
            if (ignoreId.HasValue)
            {
                query = query.Where(a => a.Id != ignoreId.Value);
            }

            if (!await query.AnyAsync())
            {
                return slug;
            }

            slug = $"{baseSlug}-{suffix++}";
        }
    }
}
