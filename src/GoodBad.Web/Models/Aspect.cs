using System.ComponentModel.DataAnnotations;

namespace GoodBad.Web.Models;

public enum AspectKind
{
    /// <summary>Something wrong with the product.</summary>
    Problem = 0,
    /// <summary>Something that is genuinely good about the product.</summary>
    Praise = 1
}

/// <summary>
/// A reusable defect/praise ("går op i sømmene", "plastikken er af lav kvalitet",
/// "smagen er dårlig"). Aspects are tied to the categories where they make sense.
/// </summary>
public class Aspect
{
    public int Id { get; set; }

    [Required, MaxLength(160)]
    public string Text { get; set; } = string.Empty;

    [Required, MaxLength(180)]
    public string Slug { get; set; } = string.Empty;

    public AspectKind Kind { get; set; }

    [MaxLength(600)]
    public string? Description { get; set; }

    public ICollection<CategoryAspect> Categories { get; set; } = new List<CategoryAspect>();
    public ICollection<ReviewAspect> Reviews { get; set; } = new List<ReviewAspect>();
}

/// <summary>Which aspects are meaningful for a given category.</summary>
public class CategoryAspect
{
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int AspectId { get; set; }
    public Aspect Aspect { get; set; } = null!;
}
