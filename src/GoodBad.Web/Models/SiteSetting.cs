using System.ComponentModel.DataAnnotations;

namespace GoodBad.Web.Models;

/// <summary>
/// A single editable site setting (site title, integration keys, upload limits
/// ...). Everything that an administrator should be able to change without a
/// redeploy lives here and is edited in /Admin/Settings.
/// </summary>
public class SiteSetting
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Key { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string? Value { get; set; }

    [MaxLength(400)]
    public string? Description { get; set; }

    /// <summary>Group shown in the admin UI, e.g. "Generelt" or "Integrationer".</summary>
    [MaxLength(80)]
    public string? Group { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
