using GoodBad.Web.Data;
using GoodBad.Web.Models;
using GoodBad.Web.Services;
using GoodBad.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodBad.Web.Controllers;

public class CategoriesController : Controller
{
    private readonly AppDbContext _db;

    public CategoriesController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _db.Categories
            .Include(c => c.Children)
            .Where(c => c.ParentId == null)
            .OrderBy(c => c.SortOrder)
            .ToListAsync();
        return View(categories);
    }

    public async Task<IActionResult> Details(string slug)
    {
        var category = await _db.Categories
            .Include(c => c.Parent)
            .Include(c => c.Children)
            .FirstOrDefaultAsync(c => c.Slug == slug);

        if (category is null)
        {
            return NotFound();
        }

        var ids = category.Children.Select(c => c.Id).Append(category.Id).ToList();

        var model = new CategoryDetailsViewModel
        {
            Category = category,
            Products = await _db.Products
                .Where(p => ids.Contains(p.CategoryId))
                .OrderByDescending(p => p.IsRecommended)
                .ThenByDescending(p => p.Reviews.Count)
                .ThenBy(p => p.Name)
                .Take(60)
                .ToCards()
                .ToListAsync(),
            Problems = await _db.Aspects
                .Where(a => a.Kind == AspectKind.Problem &&
                            (a.Categories.Any(ca => ca.CategoryId == category.Id) || !a.Categories.Any()))
                .OrderByDescending(a => a.Reviews.Count)
                .ThenBy(a => a.Text)
                .Take(24)
                .ToListAsync(),
            Praises = await _db.Aspects
                .Where(a => a.Kind == AspectKind.Praise &&
                            (a.Categories.Any(ca => ca.CategoryId == category.Id) || !a.Categories.Any()))
                .OrderByDescending(a => a.Reviews.Count)
                .ThenBy(a => a.Text)
                .Take(24)
                .ToListAsync(),
            Lists = await _db.ProductLists
                .Include(l => l.Items)
                .Where(l => l.CategoryId == category.Id ||
                            (l.Category != null && l.Category.ParentId == category.Id))
                .OrderBy(l => l.Name)
                .ToListAsync()
        };

        return View(model);
    }
}
