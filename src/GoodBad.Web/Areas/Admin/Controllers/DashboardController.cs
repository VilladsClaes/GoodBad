using GoodBad.Web.Data;
using GoodBad.Web.Models;
using GoodBad.Web.Services;
using GoodBad.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodBad.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
public class DashboardController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly YouTubeSearchClient _youTube;
    private readonly RedditSearchClient _reddit;

    public DashboardController(
        AppDbContext db,
        UserManager<ApplicationUser> userManager,
        YouTubeSearchClient youTube,
        RedditSearchClient reddit)
    {
        _db = db;
        _userManager = userManager;
        _youTube = youTube;
        _reddit = reddit;
    }

    public async Task<IActionResult> Index()
    {
        var model = new AdminDashboardViewModel
        {
            ProductCount = await _db.Products.CountAsync(),
            HiddenProductCount = await _db.Products.CountAsync(p => p.IsHidden),
            ReviewCount = await _db.Reviews.CountAsync(),
            HiddenReviewCount = await _db.Reviews.CountAsync(r => r.IsHidden),
            FixCount = await _db.Fixes.CountAsync(),
            HiddenFixCount = await _db.Fixes.CountAsync(f => f.IsHidden),
            BrandCount = await _db.Brands.CountAsync(),
            CategoryCount = await _db.Categories.CountAsync(),
            AspectCount = await _db.Aspects.CountAsync(),
            RuleCount = await _db.FixSuggestionRules.CountAsync(),
            RecommendationCount = await _db.ProductRecommendations.CountAsync(),
            ListCount = await _db.ProductLists.CountAsync(),
            UserCount = await _db.Users.CountAsync(),
            AdminCount = (await _userManager.GetUsersInRoleAsync(Roles.Admin)).Count,
            YouTubeConfigured = await _youTube.IsConfiguredAsync(),
            RedditConfigured = await _reddit.IsConfiguredAsync(),
            LatestReviews = await _db.Reviews
                .Include(r => r.Product)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .Take(8)
                .ToListAsync(),
            LatestUsers = await _db.Users
                .OrderByDescending(u => u.CreatedAt)
                .Take(8)
                .ToListAsync()
        };

        return View(model);
    }
}
