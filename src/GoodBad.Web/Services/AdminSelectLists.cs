using GoodBad.Web.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GoodBad.Web.Services;

/// <summary>Shared dropdown data for the admin area.</summary>
public static class AdminSelectLists
{
    public static async Task<List<SelectListItem>> CategoriesAsync(AppDbContext db)
        => (await db.Categories.Include(c => c.Parent).ToListAsync())
            .OrderBy(c => c.Parent is null ? c.Name : c.Parent.Name)
            .ThenBy(c => c.Parent is null ? 0 : 1)
            .ThenBy(c => c.Name)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Parent is null ? c.Name : $"{c.Parent.Name} › {c.Name}"
            })
            .ToList();

    public static async Task<List<SelectListItem>> BrandsAsync(AppDbContext db)
        => (await db.Brands.OrderBy(b => b.Name).ToListAsync())
            .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name })
            .ToList();

    public static async Task<List<SelectListItem>> ProductsAsync(AppDbContext db)
        => (await db.Products.Include(p => p.Brand).OrderBy(p => p.Name).ToListAsync())
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Brand is null ? p.Name : $"{p.Name} ({p.Brand.Name})"
            })
            .ToList();

    public static async Task<List<SelectListItem>> AspectsAsync(AppDbContext db)
        => (await db.Aspects.OrderBy(a => a.Kind).ThenBy(a => a.Text).ToListAsync())
            .Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Kind == Models.AspectKind.Problem ? $"Problem: {a.Text}" : $"Styrke: {a.Text}"
            })
            .ToList();
}
