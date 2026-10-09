using GoodBad.Web.Data;
using GoodBad.Web.Models;
using GoodBad.Web.Services;
using GoodBad.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodBad.Web.Controllers;

public class BrandsController : Controller
{
    private readonly AppDbContext _db;

    public BrandsController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var brands = await _db.Brands
            .Include(b => b.Products)
            .OrderBy(b => b.Name)
            .ToListAsync();
        return View(brands);
    }

    public async Task<IActionResult> Details(string slug)
    {
        var brand = await _db.Brands.FirstOrDefaultAsync(b => b.Slug == slug);
        if (brand is null)
        {
            return NotFound();
        }

        var products = await _db.Products.Where(p => p.BrandId == brand.Id).ToCards().ToListAsync();

        var model = new BrandDetailsViewModel
        {
            Brand = brand,
            Products = products,
            GoodCount = products.Sum(p => p.GoodCount),
            BadCount = products.Sum(p => p.BadCount),
            AverageScore = products.Count == 0 ? 0 : Math.Round(products.Average(p => p.Score), 1)
        };

        return View(model);
    }
}
