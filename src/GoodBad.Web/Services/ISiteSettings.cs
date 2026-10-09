namespace GoodBad.Web.Services;

/// <summary>
/// Read-only access to the editable settings stored in the database
/// (/Admin/Settings). An interface is used so the integrations (YouTube,
/// Reddit, image storage) can be unit tested without a database.
/// </summary>
public interface ISiteSettings
{
    /// <summary>All stored settings, keyed by <see cref="SettingKeys"/>.</summary>
    Task<Dictionary<string, string?>> AllAsync();

    /// <summary>One setting, or null when it is not set / empty.</summary>
    Task<string?> GetAsync(string key);
}
