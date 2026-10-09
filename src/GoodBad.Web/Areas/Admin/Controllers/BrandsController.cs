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
public class BrandsController : Controller
{
    private readonly AppDbContext _db;
    private readonly ImageStorageService _images;

    public BrandsController(AppDbContext db, ImageStorageService images)
    {
        _db = db;
        _images = images;
    }

    public async Task<IActionResult> Index(string? q)
    {
        var query = _db.Brands.Include(b => b.Products).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(b => b.Name.Contains(q));
        }

        ViewBag.Query = q;
        return View(await query.OrderBy(b => b.Name).ToListAsync());
    }

    public IActionResult Create() => View("Edit", new BrandEditViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BrandEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Edit", model);
        }

        var brand = new Brand
        {
            Name = model.Name.Trim(),
            Slug = await UniqueSlugAsync(model.Name, null),
            WebsiteUrl = model.WebsiteUrl,
            Country = model.Country,
            Description = model.Description
        };

        var (logoUrl, error) = await _images.SaveAsync(model.LogoFile, "brands");
        if (error is not null)
        {
            ModelState.AddModelError(nameof(model.LogoFile), error);
            return View("Edit", model);
        }

        brand.LogoUrl = logoUrl ?? model.LogoUrl;

        _db.Brands.Add(brand);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Brandet \"{brand.Name}\" er oprettet.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var brand = await _db.Brands.FindAsync(id);
        if (brand is null)
        {
            return NotFound();
        }

        return View(new BrandEditViewModel
        {
            Id = brand.Id,
            Name = brand.Name,
            WebsiteUrl = brand.WebsiteUrl,
            Country = brand.Country,
            Description = brand.Description,
            LogoUrl = brand.LogoUrl
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BrandEditViewModel model)
    {
        var brand = await _db.Brands.FindAsync(model.Id);
        if (brand is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var (logoUrl, error) = await _images.SaveAsync(model.LogoFile, "brands");
        if (error is not null)
        {
            ModelState.AddModelError(nameof(model.LogoFile), error);
            return View(model);
        }

        if (!string.Equals(brand.Name, model.Name.Trim(), StringComparison.Ordinal))
        {
            brand.Slug = await UniqueSlugAsync(model.Name, brand.Id);
        }

        brand.Name = model.Name.Trim();
        brand.WebsiteUrl = model.WebsiteUrl;
        brand.Country = model.Country;
        brand.Description = model.Description;

        if (logoUrl is not null)
        {
            _images.Delete(brand.LogoUrl);
            brand.LogoUrl = logoUrl;
        }
        else if (!string.IsNullOrWhiteSpace(model.LogoUrl) && model.LogoUrl != brand.LogoUrl)
        {
            _images.Delete(brand.LogoUrl);
            brand.LogoUrl = model.LogoUrl;
        }

        await _db.SaveChangesAsync();
        TempData["Success"] = "Brandet er opdateret.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var brand = await _db.Brands.FirstOrDefaultAsync(b => b.Id == id);
        if (brand is null)
        {
            return NotFound();
        }

        _images.Delete(brand.LogoUrl);
        _db.Brands.Remove(brand);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Brandet er slettet. Produkterne findes stadig, men uden brand.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<string> UniqueSlugAsync(string name, int? ignoreId)
    {
        var baseSlug = SlugHelper.Slugify(name);
        var slug = baseSlug;
        var suffix = 2;

        while (true)
        {
            var query = _db.Brands.Where(b => b.Slug == slug);
            if (ignoreId.HasValue)
            {
                query = query.Where(b => b.Id != ignoreId.Value);
            }

            if (!await query.AnyAsync())
            {
                return slug;
            }

            slug = $"{baseSlug}-{suffix++}";
        }
    }
}
