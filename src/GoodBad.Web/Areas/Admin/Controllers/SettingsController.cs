using GoodBad.Web.Services;
using GoodBad.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodBad.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
public class SettingsController : Controller
{
    private readonly SiteSettingsService _settings;

    public SettingsController(SiteSettingsService settings)
    {
        _settings = settings;
    }

    public async Task<IActionResult> Index()
    {
        var stored = await _settings.AllAsync();

        var model = new SettingsViewModel
        {
            Rows = SettingKeys.Editable.Select(def => new SettingRow
            {
                Key = def.Key,
                Label = def.Label,
                Group = def.Group,
                Value = stored.TryGetValue(def.Key, out var value) && !string.IsNullOrWhiteSpace(value) ? value : def.Default
            }).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(IFormCollection form)
    {
        var saved = 0;
        for (var i = 0; i < SettingKeys.Editable.Length; i++)
        {
            var def = SettingKeys.Editable[i];
            var value = form[$"values[{i}]"].ToString();

            var normalized = string.Equals(value, def.Default, StringComparison.Ordinal) ? string.Empty : value;
            await _settings.SetAsync(def.Key, normalized, def.Label, def.Group);
            saved++;
        }

        TempData["Success"] = $"{saved} indstillinger gemt.";
        return RedirectToAction(nameof(Index));
    }
}
