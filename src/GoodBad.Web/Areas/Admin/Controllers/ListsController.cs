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
public class ListsController : Controller
{
    private readonly AppDbContext _db;

    public ListsController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _db.ProductLists
            .Include(l => l.Category)
            .Include(l => l.Owner)
            .Include(l => l.Items)
            .OrderByDescending(l => l.IsEditorial)
            .ThenBy(l => l.Name)
            .ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        await PopulateAsync();
        return View("Edit", new ListEditViewModel { IsEditorial = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ListEditViewModel model)
    {
        await PopulateAsync();
        if (!ModelState.IsValid)
        {
            return View("Edit", model);
        }

        var list = new ProductList
        {
            Name = model.Name.Trim(),
            Slug = await UniqueSlugAsync(model.Name, null),
            Description = model.Description,
            CategoryId = model.CategoryId,
            IsEditorial = model.IsEditorial,
            IsHidden = model.IsHidden
        };

        _db.ProductLists.Add(list);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Listen \"{list.Name}\" er oprettet.";
        return RedirectToAction(nameof(Edit), new { id = list.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var list = await _db.ProductLists
            .Include(l => l.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Brand)
            .FirstOrDefaultAsync(l => l.Id == id);
        if (list is null)
        {
            return NotFound();
        }

        await PopulateAsync();
        ViewBag.List = list;
        return View(new ListEditViewModel
        {
            Id = list.Id,
            Name = list.Name,
            Description = list.Description,
            CategoryId = list.CategoryId,
            IsEditorial = list.IsEditorial,
            IsHidden = list.IsHidden
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ListEditViewModel model)
    {
        var list = await _db.ProductLists.FirstOrDefaultAsync(l => l.Id == model.Id);
        if (list is null)
        {
            return NotFound();
        }

        await PopulateAsync();
        if (!ModelState.IsValid)
        {
            ViewBag.List = await _db.ProductLists
                .Include(l => l.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Brand)
                .FirstAsync(l => l.Id == model.Id);
            return View(model);
        }

        if (!string.Equals(list.Name, model.Name.Trim(), StringComparison.Ordinal))
        {
            list.Slug = await UniqueSlugAsync(model.Name, list.Id);
        }

        list.Name = model.Name.Trim();
        list.Description = model.Description;
        list.CategoryId = model.CategoryId;
        list.IsEditorial = model.IsEditorial;
        list.IsHidden = model.IsHidden;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Listen er opdateret.";
        return RedirectToAction(nameof(Edit), new { id = list.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddItem(int id, int productId, string? note)
    {
        var list = await _db.ProductLists.FindAsync(id);
        if (list is null)
        {
            return NotFound();
        }

        if (!await _db.ProductListItems.AnyAsync(i => i.ProductListId == id && i.ProductId == productId))
        {
            var sortOrder = (await _db.ProductListItems.Where(i => i.ProductListId == id).Select(i => (int?)i.SortOrder).MaxAsync() ?? 0) + 1;
            _db.ProductListItems.Add(new ProductListItem
            {
                ProductListId = id,
                ProductId = productId,
                Note = note,
                SortOrder = sortOrder
            });
            await _db.SaveChangesAsync();
            TempData["Success"] = "Produktet er tilføjet listen.";
        }
        else
        {
            TempData["Error"] = "Produktet står allerede på listen.";
        }

        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveItem(int id, int itemId)
    {
        var item = await _db.ProductListItems.FirstOrDefaultAsync(i => i.Id == itemId && i.ProductListId == id);
        if (item is not null)
        {
            _db.ProductListItems.Remove(item);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Produktet er fjernet fra listen.";
        }

        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetFeatured(int id, bool value)
    {
        var list = await _db.ProductLists.FindAsync(id);
        if (list is null)
        {
            return NotFound();
        }

        list.IsEditorial = value;
        await _db.SaveChangesAsync();
        TempData["Success"] = value ? "Listen er nu redaktionel (GoodBad-anbefaling)." : "Listen er nu en fællesliste.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var list = await _db.ProductLists.FirstOrDefaultAsync(l => l.Id == id);
        if (list is null)
        {
            return NotFound();
        }

        _db.ProductLists.Remove(list);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Listen er slettet.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateAsync()
    {
        ViewBag.Categories = await AdminSelectLists.CategoriesAsync(_db);
        ViewBag.Products = await AdminSelectLists.ProductsAsync(_db);
    }

    private async Task<string> UniqueSlugAsync(string name, int? ignoreId)
    {
        var baseSlug = SlugHelper.Slugify(name);
        var slug = baseSlug;
        var suffix = 2;

        while (true)
        {
            var query = _db.ProductLists.Where(l => l.Slug == slug);
            if (ignoreId.HasValue)
            {
                query = query.Where(l => l.Id != ignoreId.Value);
            }

            if (!await query.AnyAsync())
            {
                return slug;
            }

            slug = $"{baseSlug}-{suffix++}";
        }
    }
}
