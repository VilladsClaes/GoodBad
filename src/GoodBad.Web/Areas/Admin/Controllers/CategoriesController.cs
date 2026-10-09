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
public class CategoriesController : Controller
{
    private readonly AppDbContext _db;

    public CategoriesController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.ProductCounts = await _db.Products
            .GroupBy(p => p.CategoryId)
            .Select(g => new { CategoryId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.CategoryId, x => x.Count);

        var categories = await _db.Categories
            .Include(c => c.Parent)
            .OrderBy(c => c.ParentId == null ? c.SortOrder : 999)
            .ThenBy(c => c.Parent!.SortOrder)
            .ThenBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();

        return View(categories);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateAsync();
        return View("Edit", new CategoryEditViewModel { SortOrder = await NextSortOrderAsync(null) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryEditViewModel model)
    {
        await PopulateAsync();
        if (!ModelState.IsValid)
        {
            return View("Edit", model);
        }

        var category = new Category
        {
            Name = model.Name.Trim(),
            Slug = await UniqueSlugAsync(model.Name, null),
            Description = model.Description,
            Icon = model.Icon,
            ParentId = model.ParentId,
            SortOrder = model.SortOrder
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Kategorien \"{category.Name}\" er oprettet.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category is null)
        {
            return NotFound();
        }

        await PopulateAsync(category.Id);
        return View(new CategoryEditViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Icon = category.Icon,
            ParentId = category.ParentId,
            SortOrder = category.SortOrder
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CategoryEditViewModel model)
    {
        var category = await _db.Categories.FindAsync(model.Id);
        if (category is null)
        {
            return NotFound();
        }

        if (model.ParentId == model.Id)
        {
            ModelState.AddModelError(nameof(model.ParentId), "En kategori kan ikke være sin egen overkategori.");
        }

        await PopulateAsync(category.Id);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!string.Equals(category.Name, model.Name.Trim(), StringComparison.Ordinal))
        {
            category.Slug = await UniqueSlugAsync(model.Name, category.Id);
        }

        category.Name = model.Name.Trim();
        category.Description = model.Description;
        category.Icon = model.Icon;
        category.ParentId = model.ParentId;
        category.SortOrder = model.SortOrder;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Kategorien er opdateret.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category is null)
        {
            return NotFound();
        }

        if (await _db.Categories.AnyAsync(c => c.ParentId == id))
        {
            TempData["Error"] = "Kategorien har underkategorier. Flyt eller slet dem først.";
            return RedirectToAction(nameof(Index));
        }

        if (await _db.Products.AnyAsync(p => p.CategoryId == id))
        {
            TempData["Error"] = "Kategorien indeholder produkter. Flyt dem til en anden kategori først.";
            return RedirectToAction(nameof(Index));
        }

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Kategorien er slettet.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateAsync(int? excludeId = null)
    {
        var query = _db.Categories.Include(c => c.Parent).AsQueryable();
        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        var parents = await query
            .Where(c => c.ParentId == null)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();

        ViewBag.ParentCategories = parents;
    }

    private async Task<int> NextSortOrderAsync(int? parentId)
        => (await _db.Categories.Where(c => c.ParentId == parentId).Select(c => (int?)c.SortOrder).MaxAsync() ?? 0) + 1;

    private async Task<string> UniqueSlugAsync(string name, int? ignoreId)
    {
        var baseSlug = SlugHelper.Slugify(name);
        var slug = baseSlug;
        var suffix = 2;

        while (true)
        {
            var query = _db.Categories.Where(c => c.Slug == slug);
            if (ignoreId.HasValue)
            {
                query = query.Where(c => c.Id != ignoreId.Value);
            }

            if (!await query.AnyAsync())
            {
                return slug;
            }

            slug = $"{baseSlug}-{suffix++}";
        }
    }
}
