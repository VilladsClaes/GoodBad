using System.ComponentModel.DataAnnotations;

namespace GoodBad.Web.Models;

/// <summary>
/// A curated, peer-reviewed shopping list such as "Værktøj til hobbyværkstedet"
/// or "Regntøj til børn".
/// </summary>
public class ProductList
{
    public int Id { get; set; }

    [Required, MaxLength(160)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(180)]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    [MaxLength(450)]
    public string? OwnerUserId { get; set; }
    public ApplicationUser? Owner { get; set; }

    /// <summary>True for GoodBad editorial lists, false for community lists.</summary>
    public bool IsEditorial { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ProductListItem> Items { get; set; } = new List<ProductListItem>();
}

public class ProductListItem
{
    public int Id { get; set; }

    public int ProductListId { get; set; }
    public ProductList ProductList { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    [MaxLength(600)]
    public string? Note { get; set; }

    public int SortOrder { get; set; }
}

/// <summary>
/// "Du skulle have købt dette i stedet" pairing between a bad product and a
/// durable alternative (and the mirror image on good products).
/// </summary>
public class ProductRecommendation
{
    public int Id { get; set; }

    public int SourceProductId { get; set; }
    public Product SourceProduct { get; set; } = null!;

    public int AlternativeProductId { get; set; }
    public Product AlternativeProduct { get; set; } = null!;

    [MaxLength(600)]
    public string? Reason { get; set; }
}
