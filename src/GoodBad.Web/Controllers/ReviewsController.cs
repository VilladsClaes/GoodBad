using GoodBad.Web.Data;
using GoodBad.Web.Models;
using GoodBad.Web.Services;
using GoodBad.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodBad.Web.Controllers;

public class ReviewsController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly FixSuggestionService _suggestions;
    private readonly ImageStorageService _images;

    public ReviewsController(
        AppDbContext db,
        UserManager<ApplicationUser> userManager,
        FixSuggestionService suggestions,
        ImageStorageService images)
    {
        _db = db;
        _userManager = userManager;
        _suggestions = suggestions;
        _images = images;
    }

    /// <summary>Creates a review and lets the system look for fixes in the keywords used.</summary>
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReviewCreateViewModel model)
    {
        var product = await _db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == model.ProductId);
        if (product is null)
        {
            return NotFound();
        }

        if (model.Verdict is null || string.IsNullOrWhiteSpace(model.Title) || string.IsNullOrWhiteSpace(model.Body))
        {
            TempData["Error"] = "Udfyld venligst om du kan lide produktet, en overskrift og en forklaring.";
            return RedirectToProduct(product.Slug);
        }

        // Optional photo of the product: an upload from the camera / camera
        // roll / Google Photos, or a plain image link.
        var (uploadedImage, imageError) = await _images.SaveAsync(model.ImageFile, "reviews");
        if (imageError is not null)
        {
            TempData["Error"] = imageError;
            return RedirectToProduct(product.Slug);
        }

        var review = new Review
        {
            ProductId = product.Id,
            UserId = _userManager.GetUserId(User),
            Verdict = model.Verdict.Value,
            Title = model.Title.Trim(),
            Body = model.Body.Trim(),
            OwnershipDuration = model.OwnershipDuration,
            ImageUrl = uploadedImage ?? Truncate(model.ImageUrl, 600)
        };

        if (model.SelectedAspectIds.Count > 0)
        {
            var aspectIds = await _db.Aspects
                .Where(a => model.SelectedAspectIds.Contains(a.Id))
                .Select(a => a.Id)
                .ToListAsync();
            foreach (var id in aspectIds)
            {
                review.Aspects.Add(new ReviewAspect { AspectId = id });
            }
        }

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        // The system scans what the user wrote for keywords and proposes
        // YouTube tutorials / Reddit threads that might fix the problem.
        var suggestions = await _suggestions.SuggestAsync($"{review.Title} {review.Body}", 6, product.Category.Name);
        if (suggestions.Count > 0)
        {
            foreach (var suggestion in suggestions)
            {
                _db.Fixes.Add(new Fix
                {
                    ProductId = product.Id,
                    ReviewId = review.Id,
                    Source = FixSource.System,
                    SourceKind = suggestion.Kind,
                    Title = suggestion.Title,
                    Body = suggestion.Body ?? $"Systemforslag fundet ud fra dine ord om \u201c{suggestion.MatchedKeyword}\u201d.",
                    SourceUrl = suggestion.Url
                });
            }

            await _db.SaveChangesAsync();
        }

        TempData["Success"] = "Tak for din anmeldelse! Se om nogen har et fix til problemet nedenfor.";
        return RedirectToProduct(product.Slug);
    }

    /// <summary>"Jeg er enig i at produktet er godt/dårligt".</summary>
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Vote(int id, bool agree = true)
    {
        var review = await _db.Reviews.Include(r => r.Product).ThenInclude(p => p.Category).FirstOrDefaultAsync(r => r.Id == id);
        if (review is null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User)!;
        var existing = await _db.ReviewVotes.FirstOrDefaultAsync(v => v.ReviewId == id && v.UserId == userId);

        if (existing is null)
        {
            _db.ReviewVotes.Add(new ReviewVote { ReviewId = id, UserId = userId, IsAgree = agree });
        }
        else if (existing.IsAgree == agree)
        {
            // Clicking the same button again removes the vote.
            _db.ReviewVotes.Remove(existing);
        }
        else
        {
            existing.IsAgree = agree;
        }

        await _db.SaveChangesAsync();
        return RedirectToProduct(review.Product.Slug);
    }

    /// <summary>"Det problem har jeg også".</summary>
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VoteAspect(int id)
    {
        var reviewAspect = await _db.ReviewAspects
            .Include(ra => ra.Review).ThenInclude(r => r.Product)
            .FirstOrDefaultAsync(ra => ra.Id == id);
        if (reviewAspect is null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User)!;
        var existing = await _db.ReviewAspectVotes
            .FirstOrDefaultAsync(v => v.ReviewAspectId == id && v.UserId == userId);

        if (existing is null)
        {
            _db.ReviewAspectVotes.Add(new ReviewAspectVote { ReviewAspectId = id, UserId = userId });
        }
        else
        {
            _db.ReviewAspectVotes.Remove(existing);
        }

        await _db.SaveChangesAsync();
        return RedirectToProduct(reviewAspect.Review.Product.Slug);
    }

    /// <summary>Regenerates system fixes for an existing review.</summary>
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RefreshSuggestions(int id)
    {
        var review = await _db.Reviews.Include(r => r.Product).ThenInclude(p => p.Category).FirstOrDefaultAsync(r => r.Id == id);
        if (review is null)
        {
            return NotFound();
        }

        var existingUrls = await _db.Fixes
            .Where(f => f.ReviewId == id && f.Source == FixSource.System)
            .Select(f => f.SourceUrl)
            .ToListAsync();

        var suggestions = await _suggestions.SuggestAsync($"{review.Title} {review.Body}", 6, review.Product.Category.Name);
        var added = 0;
        foreach (var suggestion in suggestions)
        {
            if (suggestion.Url is not null && existingUrls.Contains(suggestion.Url))
            {
                continue;
            }

            _db.Fixes.Add(new Fix
            {
                ProductId = review.ProductId,
                ReviewId = review.Id,
                Source = FixSource.System,
                SourceKind = suggestion.Kind,
                Title = suggestion.Title,
                Body = suggestion.Body ?? $"Systemforslag fundet ud fra dine ord om \u201c{suggestion.MatchedKeyword}\u201d.",
                SourceUrl = suggestion.Url
            });
            added++;
        }

        await _db.SaveChangesAsync();
        TempData["Success"] = added > 0
            ? $"{added} nye forslag blev fundet."
            : "Der blev ikke fundet nye forslag.";
        return RedirectToProduct(review.Product.Slug);
    }

    private static string? Truncate(string? value, int max)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var trimmed = value.Trim();
        return trimmed.Length <= max ? trimmed : trimmed[..max];
    }

    private IActionResult RedirectToProduct(string slug)
        => Redirect($"{Url.Action(nameof(ProductsController.Details), "Products", new { slug })}#anmeldelser");
}
