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
public class FixRulesController : Controller
{
    private readonly AppDbContext _db;

    public FixRulesController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _db.FixSuggestionRules
            .Include(r => r.Aspect)
            .OrderBy(r => r.Kind)
            .ThenBy(r => r.Keywords)
            .ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        await PopulateAsync();
        return View("Edit", new FixRuleEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FixRuleEditViewModel model)
    {
        await PopulateAsync();
        if (!ModelState.IsValid)
        {
            return View("Edit", model);
        }

        _db.FixSuggestionRules.Add(new FixSuggestionRule
        {
            Keywords = model.Keywords.Trim(),
            Title = model.Title.Trim(),
            Url = model.Url,
            Kind = model.Kind,
            AspectId = model.AspectId
        });

        await _db.SaveChangesAsync();
        TempData["Success"] = "Reglen er oprettet. Den bruges næste gang en anmeldelse matcher nøgleordene.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var rule = await _db.FixSuggestionRules.FindAsync(id);
        if (rule is null)
        {
            return NotFound();
        }

        await PopulateAsync();
        return View(new FixRuleEditViewModel
        {
            Id = rule.Id,
            Keywords = rule.Keywords,
            Title = rule.Title,
            Url = rule.Url,
            Kind = rule.Kind,
            AspectId = rule.AspectId
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(FixRuleEditViewModel model)
    {
        var rule = await _db.FixSuggestionRules.FindAsync(model.Id);
        if (rule is null)
        {
            return NotFound();
        }

        await PopulateAsync();
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        rule.Keywords = model.Keywords.Trim();
        rule.Title = model.Title.Trim();
        rule.Url = model.Url;
        rule.Kind = model.Kind;
        rule.AspectId = model.AspectId;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Reglen er opdateret.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var rule = await _db.FixSuggestionRules.FindAsync(id);
        if (rule is null)
        {
            return NotFound();
        }

        _db.FixSuggestionRules.Remove(rule);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Reglen er slettet.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateAsync()
        => ViewBag.Aspects = await AdminSelectLists.AspectsAsync(_db);
}
