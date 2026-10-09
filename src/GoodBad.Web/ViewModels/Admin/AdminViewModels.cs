using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using GoodBad.Web.Models;

namespace GoodBad.Web.ViewModels.Admin;

public class AdminDashboardViewModel
{
    public int ProductCount { get; set; }
    public int HiddenProductCount { get; set; }
    public int ReviewCount { get; set; }
    public int HiddenReviewCount { get; set; }
    public int FixCount { get; set; }
    public int HiddenFixCount { get; set; }
    public int BrandCount { get; set; }
    public int CategoryCount { get; set; }
    public int AspectCount { get; set; }
    public int RuleCount { get; set; }
    public int RecommendationCount { get; set; }
    public int ListCount { get; set; }
    public int UserCount { get; set; }
    public int AdminCount { get; set; }
    public bool YouTubeConfigured { get; set; }
    public bool RedditConfigured { get; set; }
    public List<Review> LatestReviews { get; set; } = new();
    public List<ApplicationUser> LatestUsers { get; set; } = new();
}

public class CategoryEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Kategorien skal have et navn."), MaxLength(120)]
    [Display(Name = "Navn")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(600)]
    [Display(Name = "Beskrivelse")]
    public string? Description { get; set; }

    [MaxLength(24)]
    [Display(Name = "Ikon (emoji)")]
    public string? Icon { get; set; }

    [Display(Name = "Overkategori")]
    public int? ParentId { get; set; }

    [Display(Name = "Sortering")]
    public int SortOrder { get; set; }
}

public class AspectEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Skriv en kort tekst."), MaxLength(160)]
    [Display(Name = "Tekst")]
    public string Text { get; set; } = string.Empty;

    [Display(Name = "Type")]
    public AspectKind Kind { get; set; }

    [MaxLength(600)]
    [Display(Name = "Beskrivelse")]
    public string? Description { get; set; }

    [Display(Name = "Gælder for kategorier (tom = alle)")]
    public List<int> CategoryIds { get; set; } = new();
}

public class BrandEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Brandet skal have et navn."), MaxLength(120)]
    [Display(Name = "Navn")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(600)]
    [Display(Name = "Hjemmeside")]
    public string? WebsiteUrl { get; set; }

    [MaxLength(80)]
    [Display(Name = "Land")]
    public string? Country { get; set; }

    [MaxLength(2000)]
    [Display(Name = "Beskrivelse")]
    public string? Description { get; set; }

    [Display(Name = "Logo (fil)")]
    public IFormFile? LogoFile { get; set; }

    [MaxLength(600)]
    [Display(Name = "Logo-URL")]
    public string? LogoUrl { get; set; }
}

public class ProductEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Produktet skal have et navn."), MaxLength(160)]
    [Display(Name = "Produktnavn")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Kategori")]
    public int CategoryId { get; set; }

    [Display(Name = "Brand / producent")]
    public int? BrandId { get; set; }

    [MaxLength(80)]
    [Display(Name = "Modelnummer")]
    public string? ModelNumber { get; set; }

    [MaxLength(4000)]
    [Display(Name = "Beskrivelse")]
    public string? Description { get; set; }

    [Display(Name = "Billede (fil)")]
    public IFormFile? ImageFile { get; set; }

    [MaxLength(600)]
    [Display(Name = "Billede-URL")]
    public string? ImageUrl { get; set; }

    [Display(Name = "GoodBad anbefaler (vises på forsiden)")]
    public bool IsRecommended { get; set; }

    [Display(Name = "Skjult for offentligheden")]
    public bool IsHidden { get; set; }
}

public class FixRuleEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Angiv mindst ét nøgleord."), MaxLength(300)]
    [Display(Name = "Nøgleord (kommasepareret)")]
    public string Keywords { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    [Display(Name = "Titel")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(600)]
    [Display(Name = "Link")]
    public string? Url { get; set; }

    [Display(Name = "Type")]
    public FixSourceKind Kind { get; set; }

    [Display(Name = "Knyttet problem (aspekt)")]
    public int? AspectId { get; set; }
}

public class RecommendationEditViewModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Dårligt produkt")]
    public int SourceProductId { get; set; }

    [Required]
    [Display(Name = "Bedre alternativ")]
    public int AlternativeProductId { get; set; }

    [MaxLength(600)]
    [Display(Name = "Begrundelse")]
    public string? Reason { get; set; }
}

public class ListEditViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(160)]
    [Display(Name = "Navn")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    [Display(Name = "Beskrivelse")]
    public string? Description { get; set; }

    [Display(Name = "Kategori")]
    public int? CategoryId { get; set; }

    [Display(Name = "Redaktionel liste (GoodBad-anbefaling)")]
    public bool IsEditorial { get; set; }

    [Display(Name = "Skjult")]
    public bool IsHidden { get; set; }
}

public class AdminUserRow
{
    public string Id { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public bool IsAdmin { get; set; }
    public int ReviewCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SettingsViewModel
{
    public List<SettingRow> Rows { get; set; } = new();
}

public class SettingRow
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public string? Value { get; set; }
}
