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
public class ProductsController : Controller
{
    private const int PageSize = 25;

    private readonly AppDbContext _db;
    private readonly ImageStorageService _images;

    public ProductsController(AppDbContext db, ImageStorageService images)
    {
        _db = db;
        _images = images;
    }

    public async Task<IActionResult> Index(string? q, string? filter, int page = 1)
    {
        var query = _db.Products.Include(p => p.Brand).Include(p => p.Category).Include(p => p.Reviews).AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(p => p.Name.Contains(q) || (p.Brand != null && p.Brand.Name.Contains(q)));
        }

        query = filter switch
        {
            "hidden" => query.Where(p => p.IsHidden),
            "recommended" => query.Where(p => p.IsRecommended),
            "noimage" => query.Where(p => p.ImageUrl == null),
            _ => query
        };

        var total = await query.CountAsync();
        page = Math.Max(1, page);

        ViewBag.Query = q;
        ViewBag.Filter = filter;
        ViewBag.Page = page;
        ViewBag.TotalPages = (int)Math.Ceiling(total / (double)PageSize);
        ViewBag.Total = total;

        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        return View(products);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product is null)
        {
            return NotFound();
        }

        await PopulateAsync();
        ViewBag.Slug = product.Slug;
        return View(new ProductEditViewModel
        {
            Id = product.Id,
            Name = product.Name,
            CategoryId = product.CategoryId,
            BrandId = product.BrandId,
            ModelNumber = product.ModelNumber,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            IsRecommended = product.IsRecommended,
            IsHidden = product.IsHidden
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductEditViewModel model)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == model.Id);
        if (product is null)
        {
            return NotFound();
        }

        if (!await _db.Categories.AnyAsync(c => c.Id == model.CategoryId))
        {
            ModelState.AddModelError(nameof(model.CategoryId), "Vælg en gyldig kategori.");
        }

        var (imageUrl, error) = await _images.SaveAsync(model.ImageFile, "products");
        if (error is not null)
        {
            ModelState.AddModelError(nameof(model.ImageFile), error);
        }

        if (!ModelState.IsValid)
        {
            await PopulateAsync();
            ViewBag.Slug = product.Slug;
            return View(model);
        }

        if (!string.Equals(product.Name, model.Name.Trim(), StringComparison.Ordinal))
        {
            product.Slug = await UniqueSlugAsync(model.Name, product.Id);
        }

        product.Name = model.Name.Trim();
        product.CategoryId = model.CategoryId;
        product.BrandId = model.BrandId;
        product.ModelNumber = model.ModelNumber;
        product.Description = model.Description;
        product.IsRecommended = model.IsRecommended;
        product.IsHidden = model.IsHidden;
        product.UpdatedAt = DateTime.UtcNow;

        if (imageUrl is not null)
        {
            _images.Delete(product.ImageUrl);
            product.ImageUrl = imageUrl;
        }
        else if (!string.IsNullOrWhiteSpace(model.ImageUrl) && model.ImageUrl != product.ImageUrl)
        {
            _images.Delete(product.ImageUrl);
            product.ImageUrl = model.ImageUrl;
        }
        else if (string.IsNullOrWhiteSpace(model.ImageUrl) && model.ImageFile is null && !string.IsNullOrWhiteSpace(product.ImageUrl))
        {
            // The URL field was cleared manually.
            if (!ImageStorageService.IsRemoteUrl(product.ImageUrl))
            {
                _images.Delete(product.ImageUrl);
            }

            product.ImageUrl = null;
        }

        await _db.SaveChangesAsync();
        TempData["Success"] = "Produktet er opdateret.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetRecommended(int id, bool value, string? returnUrl)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product is null)
        {
            return NotFound();
        }

        product.IsRecommended = value;
        await _db.SaveChangesAsync();
        TempData["Success"] = value
            ? $"\"{product.Name}\" vises nu blandt GoodBad-anbefalingerne."
            : $"\"{product.Name}\" er fjernet fra anbefalingerne.";
        return SafeRedirect(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetHidden(int id, bool value, string? returnUrl)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product is null)
        {
            return NotFound();
        }

        product.IsHidden = value;
        await _db.SaveChangesAsync();
        TempData["Success"] = value ? $"\"{product.Name}\" er skjult." : $"\"{product.Name}\" er synligt igen.";
        return SafeRedirect(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product is null)
        {
            return NotFound();
        }

        _images.Delete(product.ImageUrl);
        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Produktet og dets anmeldelser er slettet.";
        return RedirectToAction(nameof(Index));
    }

    private IActionResult SafeRedirect(string? returnUrl)
        => !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction(nameof(Index));

    private async Task PopulateAsync()
    {
        ViewBag.Categories = await AdminSelectLists.CategoriesAsync(_db);
        ViewBag.Brands = await AdminSelectLists.BrandsAsync(_db);
    }

    private async Task<string> UniqueSlugAsync(string name, int? ignoreId)
    {
        var baseSlug = SlugHelper.Slugify(name);
        var slug = baseSlug;
        var suffix = 2;

        while (true)
        {
            var query = _db.Products.Where(p => p.Slug == slug);
            if (ignoreId.HasValue)
            {
                query = query.Where(p => p.Id != ignoreId.Value);
            }

            if (!await query.AnyAsync())
            {
                return slug;
            }

            slug = $"{baseSlug}-{suffix++}";
        }
    }
}
