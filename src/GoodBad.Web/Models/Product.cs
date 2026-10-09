using System.ComponentModel.DataAnnotations;

namespace GoodBad.Web.Models;

/// <summary>A physical, human-made product that can be reviewed.</summary>
public class Product
{
    public int Id { get; set; }

    [Required, MaxLength(160)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(180)]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string? Description { get; set; }

    [MaxLength(600)]
    public string? ImageUrl { get; set; }

    [MaxLength(80)]
    public string? ModelNumber { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int? BrandId { get; set; }
    public Brand? Brand { get; set; }

    [MaxLength(450)]
    public string? CreatedByUserId { get; set; }
    public ApplicationUser? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Set by GoodBad editors to showcase truly durable products on the front page.</summary>
    public bool IsRecommended { get; set; }

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Fix> Fixes { get; set; } = new List<Fix>();
    public ICollection<ProductListItem> ListItems { get; set; } = new List<ProductListItem>();
    public ICollection<ProductRecommendation> RecommendationsAsSource { get; set; } = new List<ProductRecommendation>();
    public ICollection<ProductRecommendation> RecommendationsAsAlternative { get; set; } = new List<ProductRecommendation>();
}
