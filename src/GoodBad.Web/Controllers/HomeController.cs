using System.Diagnostics;
using GoodBad.Web.Data;
using GoodBad.Web.Models;
using GoodBad.Web.Services;
using GoodBad.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodBad.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var model = new HomeViewModel
        {
            // The products we actively recommend: durable, repairable, long-lived.
            Recommended = await _db.Products
                .Where(p => p.IsRecommended)
                .OrderByDescending(p => p.Reviews.Count(r => r.Verdict == Verdict.Good))
                .ThenBy(p => p.Name)
                .Take(8)
                .ToCards()
                .ToListAsync(),

            // Products the community is warning against.
            Warnings = await _db.Products
                .Where(p => p.Reviews.Any(r => r.Verdict == Verdict.Bad))
                .OrderByDescending(p => p.Reviews.Count(r => r.Verdict == Verdict.Bad))
                .ThenBy(p => p.Name)
                .Take(4)
                .ToCards()
                .ToListAsync(),

            Lists = await _db.ProductLists
                .Include(l => l.Category)
                .Include(l => l.Items)
                .OrderByDescending(l => l.IsEditorial)
                .ThenBy(l => l.Name)
                .Take(6)
                .ToListAsync(),

            TopCategories = await _db.Categories
                .Where(c => c.ParentId == null)
                .OrderBy(c => c.SortOrder)
                .ToListAsync(),

            LatestReviews = await _db.Reviews
                .Include(r => r.Product)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .Take(6)
                .ToListAsync(),

            ProductCount = await _db.Products.CountAsync(),
            ReviewCount = await _db.Reviews.CountAsync(),
            BrandCount = await _db.Brands.CountAsync(),
            ListCount = await _db.ProductLists.CountAsync()
        };

        return View(model);
    }

    public IActionResult About() => View();

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
