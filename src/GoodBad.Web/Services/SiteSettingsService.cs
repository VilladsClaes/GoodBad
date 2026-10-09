using GoodBad.Web.Data;
using GoodBad.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GoodBad.Web.Services;

/// <summary>
/// Reads and writes the editable site settings. Values are cached briefly so
/// every request does not hit the database, and <see cref="Invalidate"/> is
/// called whenever an administrator saves a change.
/// </summary>
public class SiteSettingsService : ISiteSettings
{
    private const string CacheKey = "goodbad.site-settings";

    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;

    public SiteSettingsService(AppDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<Dictionary<string, string?>> AllAsync()
    {
        if (_cache.TryGetValue(CacheKey, out Dictionary<string, string?>? cached) && cached is not null)
        {
            return cached;
        }

        var values = await _db.SiteSettings
            .AsNoTracking()
            .ToDictionaryAsync(s => s.Key, s => s.Value);

        _cache.Set(CacheKey, values, TimeSpan.FromSeconds(30));
        return values;
    }

    public async Task<string?> GetAsync(string key)
    {
        var all = await AllAsync();
        return all.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value) ? value : null;
    }

    public async Task SetAsync(string key, string? value, string? description = null, string? group = null)
    {
        var setting = await _db.SiteSettings.FirstOrDefaultAsync(s => s.Key == key);
        if (setting is null)
        {
            setting = new SiteSetting { Key = key };
            _db.SiteSettings.Add(setting);
        }

        setting.Value = value;
        setting.UpdatedAt = DateTime.UtcNow;
        if (description is not null)
        {
            setting.Description = description;
        }

        if (group is not null)
        {
            setting.Group = group;
        }

        await _db.SaveChangesAsync();
        Invalidate();
    }

    public void Invalidate() => _cache.Remove(CacheKey);
}

/// <summary>Setting keys used across the application, with sensible defaults.</summary>
public static class SettingKeys
{
    public const string SiteTitle = "Site.Title";
    public const string SiteTagline = "Site.Tagline";
    public const string HomeWarningNote = "Home.WarningNote";

    public const string YouTubeApiKey = "Integrations.YouTube.ApiKey";
    public const string RedditUserAgent = "Integrations.Reddit.UserAgent";
    public const string RedditClientId = "Integrations.Reddit.ClientId";
    public const string RedditClientSecret = "Integrations.Reddit.ClientSecret";

    public const string MaxImageBytes = "Storage.MaxImageBytes";

    public const string AutoHideReportThreshold = "Moderation.AutoHideReportThreshold";

    /// <summary>Keys shown (and editable) on /Admin/Settings.</summary>
    public static readonly (string Key, string Label, string Group, string? Default)[] Editable =
    {
        (SiteTitle, "Sidens titel", "Generelt", "GoodBad"),
        (SiteTagline, "Tagline", "Generelt", "Anmeld produkter – godt eller skidt – og fortæl hvorfor."),
        (HomeWarningNote, "Advarselstekst på forsiden", "Generelt",
            "Produkter her har fået gentagne anmeldelser om dårlig kvalitet. Køb dem ikke."),
        (YouTubeApiKey, "YouTube Data API-nøgle", "Integrationer", null),
        (RedditUserAgent, "Reddit User-Agent", "Integrationer",
            "GoodBad/1.0 (product-review platform; by /u/goodbad)"),
        (RedditClientId, "Reddit Client ID (gør Reddit-søgning pålidelig)", "Integrationer", null),
        (RedditClientSecret, "Reddit Client Secret", "Integrationer", null),
        (MaxImageBytes, "Maks. billedstørrelse i bytes", "Upload", "8388608"),
        (AutoHideReportThreshold, "Antal rapporter før indhold skjules", "Moderation", "5")
    };
}
