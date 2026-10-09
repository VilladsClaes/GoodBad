using GoodBad.Web.Data;
using GoodBad.Web.Models;
using GoodBad.Web.Services;
using GoodBad.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodBad.Web.Controllers;

public class ListsController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ListsController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var lists = await _db.ProductLists
            .Include(l => l.Category)
            .Include(l => l.Items)
            .Include(l => l.Owner)
            .OrderByDescending(l => l.IsEditorial)
            .ThenBy(l => l.Name)
            .ToListAsync();
        return View(lists);
    }

    public async Task<IActionResult> Details(string slug)
    {
        var list = await _db.ProductLists
            .Include(l => l.Category)
            .Include(l => l.Owner)
            .Include(l => l.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Brand)
            .Include(l => l.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(l => l.Slug == slug);

        if (list is null)
        {
            return NotFound();
        }

        var productIds = list.Items.Select(i => i.ProductId).ToList();

        var model = new ListDetailsViewModel
        {
            List = list,
            Items = list.Items.OrderBy(i => i.SortOrder).ToList(),
            Products = await _db.Products
                .Where(p => productIds.Contains(p.Id))
                .ToCards()
                .ToListAsync()
        };

        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _db.Categories
            .Where(c => c.ParentId == null)
            .OrderBy(c => c.SortOrder)
            .ToListAsync();
        return View(new ListCreateViewModel());
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ListCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _db.Categories.Where(c => c.ParentId == null).OrderBy(c => c.SortOrder).ToListAsync();
            return View(model);
        }

        var list = new ProductList
        {
            Name = model.Name.Trim(),
            Description = model.Description,
            CategoryId = model.CategoryId,
            OwnerUserId = _userManager.GetUserId(User),
            IsEditorial = false,
            Slug = SlugHelper.Slugify(model.Name)
        };

        var baseSlug = list.Slug;
        var suffix = 2;
        while (await _db.ProductLists.AnyAsync(l => l.Slug == list.Slug))
        {
            list.Slug = $"{baseSlug}-{suffix++}";
        }

        _db.ProductLists.Add(list);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Listen er oprettet. Tilføj produkter fra deres produktsider.";
        return RedirectToAction(nameof(Details), new { slug = list.Slug });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddItem(int listId, int productId, string? note)
    {
        var userId = _userManager.GetUserId(User);
        var list = await _db.ProductLists.FirstOrDefaultAsync(l => l.Id == listId);
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == productId);

        if (list is null || product is null)
        {
            return NotFound();
        }

        if (list.OwnerUserId != userId)
        {
            return Forbid();
        }

        if (!await _db.ProductListItems.AnyAsync(i => i.ProductListId == listId && i.ProductId == productId))
        {
            var next = await _db.ProductListItems.Where(i => i.ProductListId == listId).CountAsync();
            _db.ProductListItems.Add(new ProductListItem
            {
                ProductListId = listId,
                ProductId = productId,
                Note = note,
                SortOrder = next
            });
            await _db.SaveChangesAsync();
            TempData["Success"] = $"\"{product.Name}\" er tilføjet til listen \"{list.Name}\".";
        }
        else
        {
            TempData["Error"] = "Produktet er allerede på listen.";
        }

        return RedirectToAction(nameof(Details), new { slug = list.Slug });
    }

    [Authorize]
    public async Task<IActionResult> Mine()
    {
        var userId = _userManager.GetUserId(User);
        var lists = await _db.ProductLists
            .Include(l => l.Category)
            .Include(l => l.Items)
            .Where(l => l.OwnerUserId == userId)
            .OrderBy(l => l.Name)
            .ToListAsync();
        return View(lists);
    }
}
