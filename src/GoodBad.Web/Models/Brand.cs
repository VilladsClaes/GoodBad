using System.ComponentModel.DataAnnotations;

namespace GoodBad.Web.Models;

/// <summary>The brand / company / manufacturer behind a product.</summary>
public class Brand
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(140)]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(600)]
    public string? LogoUrl { get; set; }

    [MaxLength(600)]
    public string? WebsiteUrl { get; set; }

    [MaxLength(80)]
    public string? Country { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
