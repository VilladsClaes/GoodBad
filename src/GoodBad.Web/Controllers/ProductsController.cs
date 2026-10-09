using GoodBad.Web.Data;
using GoodBad.Web.Models;
using GoodBad.Web.Services;
using GoodBad.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodBad.Web.Controllers;

public class ProductsController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProductsController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? q, int? categoryId, string? sort)
    {
        var query = _db.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(p =>
                p.Name.Contains(q) ||
                (p.Brand != null && p.Brand.Name.Contains(q)) ||
                (p.Description != null && p.Description.Contains(q)));
        }

        if (categoryId.HasValue)
        {
            var childIds = await _db.Categories
                .Where(c => c.ParentId == categoryId)
                .Select(c => c.Id)
                .ToListAsync();
            var ids = childIds.Append(categoryId.Value).ToList();
            query = query.Where(p => ids.Contains(p.CategoryId));
        }

        query = sort switch
        {
            "warnings" => query.OrderByDescending(p => p.Reviews.Count(r => r.Verdict == Verdict.Bad)),
            "recommended" => query.OrderByDescending(p => p.IsRecommended).ThenByDescending(p => p.Reviews.Count),
            _ => query.OrderByDescending(p => p.Reviews.Count)
        };

        var model = new ProductBrowseViewModel
        {
            Products = await query.Take(120).ToCards().ToListAsync(),
            Query = q,
            CategoryId = categoryId,
            Sort = sort,
            Categories = await _db.Categories.Where(c => c.ParentId == null).OrderBy(c => c.SortOrder).ToListAsync()
        };

        return View(model);
    }

    public async Task<IActionResult> Details(string slug)
    {
        var product = await _db.Products
            .Include(p => p.Brand)
            .Include(p => p.Category).ThenInclude(c => c.Parent)
            .Include(p => p.Reviews).ThenInclude(r => r.User)
            .Include(p => p.Reviews).ThenInclude(r => r.Aspects).ThenInclude(a => a.Aspect)
            .Include(p => p.Reviews).ThenInclude(r => r.Aspects).ThenInclude(a => a.Votes)
            .Include(p => p.Reviews).ThenInclude(r => r.Votes)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Slug == slug);

        if (product is null)
        {
            return NotFound();
        }

        var aspectIds = product.Reviews.SelectMany(r => r.Aspects).Select(a => a.Id).ToList();
        var aspectVotes = await _db.ReviewAspectVotes
            .Where(v => aspectIds.Contains(v.ReviewAspectId))
            .Select(v => v.ReviewAspectId)
            .ToListAsync();

        var allAspects = product.Reviews.SelectMany(r => r.Aspects).ToList();

        var model = new ProductDetailsViewModel
        {
            Product = product,
            GoodReviews = product.Reviews.Where(r => r.Verdict == Verdict.Good).OrderByDescending(r => r.Votes.Count(v => v.IsAgree)).ThenByDescending(r => r.CreatedAt).ToList(),
            BadReviews = product.Reviews.Where(r => r.Verdict == Verdict.Bad).OrderByDescending(r => r.Votes.Count(v => v.IsAgree)).ThenByDescending(r => r.CreatedAt).ToList(),
            GoodCount = product.Reviews.Count(r => r.Verdict == Verdict.Good),
            BadCount = product.Reviews.Count(r => r.Verdict == Verdict.Bad),

            TopProblems = allAspects
                .Where(a => a.Aspect.Kind == AspectKind.Problem)
                .GroupBy(a => a.Aspect)
                .Select(g => new AspectTally(g.Key, g.Count(), g.Sum(a => aspectVotes.Count(id => id == a.Id))))
                .OrderByDescending(t => t.AgreeCount)
                .ThenByDescending(t => t.ReviewAspectCount)
                .ToList(),

            TopPraises = allAspects
                .Where(a => a.Aspect.Kind == AspectKind.Praise)
                .GroupBy(a => a.Aspect)
                .Select(g => new AspectTally(g.Key, g.Count(), g.Sum(a => aspectVotes.Count(id => id == a.Id))))
                .OrderByDescending(t => t.AgreeCount)
                .ThenByDescending(t => t.ReviewAspectCount)
                .ToList(),

            BuyThisInstead = await _db.ProductRecommendations
                .Include(r => r.AlternativeProduct).ThenInclude(p => p.Brand)
                .Include(r => r.AlternativeProduct).ThenInclude(p => p.Category)
                .Where(r => r.SourceProductId == product.Id)
                .ToListAsync(),

            BetterChoiceThan = await _db.ProductRecommendations
                .Include(r => r.SourceProduct).ThenInclude(p => p.Brand)
                .Include(r => r.SourceProduct).ThenInclude(p => p.Category)
                .Where(r => r.AlternativeProductId == product.Id)
                .ToListAsync(),

            AppearsInLists = await _db.ProductLists
                .Include(l => l.Category)
                .Where(l => l.Items.Any(i => i.ProductId == product.Id))
                .ToListAsync(),

            Fixes = await _db.Fixes
                .Include(f => f.User)
                .Include(f => f.Votes)
                .Include(f => f.Review)
                .Where(f => f.ProductId == product.Id)
                .OrderByDescending(f => f.Votes.Count)
                .ThenByDescending(f => f.CreatedAt)
                .ToListAsync()
        };

        var aspects = await _db.Aspects
            .Include(a => a.Categories)
            .Where(a => !a.Categories.Any() || a.Categories.Any(ca => ca.CategoryId == product.CategoryId))
            .OrderBy(a => a.Kind)
            .ThenBy(a => a.Text)
            .ToListAsync();

        var userId = _userManager.GetUserId(User);
        model.UserHasReviewed = userId is not null && product.Reviews.Any(r => r.UserId == userId);
        if (userId is not null)
        {
            model.MyLists = await _db.ProductLists
                .Where(l => l.OwnerUserId == userId)
                .OrderBy(l => l.Name)
                .ToListAsync();

            var reviewIds = product.Reviews.Select(r => r.Id).ToList();
            model.UserReviewVotes = await _db.ReviewVotes
                .Where(v => v.UserId == userId && reviewIds.Contains(v.ReviewId))
                .ToDictionaryAsync(v => v.ReviewId, v => v.IsAgree);

            model.UserAspectVotes = (await _db.ReviewAspectVotes
                .Where(v => v.UserId == userId && aspectIds.Contains(v.ReviewAspectId))
                .Select(v => v.ReviewAspectId)
                .ToListAsync()).ToHashSet();
        }
        model.NewReview = new ReviewCreateViewModel
        {
            ProductId = product.Id,
            ProductName = product.Name,
            Verdict = Verdict.Bad,
            AvailableAspects = aspects.Select(a => new AspectOption
            {
                Id = a.Id,
                Text = a.Text,
                Kind = a.Kind,
                IsCommon = !a.Categories.Any()
            }).ToList()
        };

        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> Create()
    {
        await PopulateSelectListsAsync();
        return View(new ProductCreateViewModel());
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductCreateViewModel model)
    {
        if (!await _db.Categories.AnyAsync(c => c.Id == model.CategoryId))
        {
            ModelState.AddModelError(nameof(model.CategoryId), "Vælg en gyldig kategori.");
        }

        if (!ModelState.IsValid)
        {
            await PopulateSelectListsAsync();
            return View(model);
        }

        var product = new Product
        {
            Name = model.Name.Trim(),
            Description = model.Description,
            ImageUrl = model.ImageUrl,
            ModelNumber = model.ModelNumber,
            CategoryId = model.CategoryId,
            BrandId = model.BrandId,
            CreatedByUserId = _userManager.GetUserId(User),
            Slug = SlugHelper.Slugify(model.Name)
        };

        var baseSlug = product.Slug;
        var suffix = 2;
        while (await _db.Products.AnyAsync(p => p.Slug == product.Slug))
        {
            product.Slug = $"{baseSlug}-{suffix++}";
        }

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        TempData["Success"] = $"\"{product.Name}\" er tilføjet. Skriv gerne den første anmeldelse.";
        return RedirectToAction(nameof(Details), new { slug = product.Slug });
    }

    private async Task PopulateSelectListsAsync()
    {
        var all = await _db.Categories.Include(c => c.Parent).ToListAsync();
        var picker = all
            .OrderBy(c => c.Parent is null ? c.Name : c.Parent.Name)
            .ThenBy(c => c.Parent is null ? 0 : 1)
            .ThenBy(c => c.Name)
            .Select(c => new CategoryPickerItem(c.Id, c.Parent is null ? c.Name : $"{c.Parent.Name} › {c.Name}"))
            .ToList();

        ViewBag.Categories = picker;
        ViewBag.Brands = await _db.Brands.OrderBy(b => b.Name).ToListAsync();
    }
}
