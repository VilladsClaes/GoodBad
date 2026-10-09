using System.ComponentModel.DataAnnotations;

namespace GoodBad.Web.Models;

public enum Verdict
{
    /// <summary>User disliked the product.</summary>
    Bad = 0,
    /// <summary>User liked the product.</summary>
    Good = 1
}

/// <summary>
/// A review of one product. The verdict is the one-tap "like / dislike";
/// the body plus aspects explain *why*.
/// </summary>
public class Review
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    [MaxLength(450)]
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public Verdict Verdict { get; set; }

    [Required, MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(6000)]
    public string Body { get; set; } = string.Empty;

    /// <summary>How long the product has been in use, e.g. "6 måneder".</summary>
    [MaxLength(120)]
    public string? OwnershipDuration { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<ReviewAspect> Aspects { get; set; } = new List<ReviewAspect>();
    public ICollection<ReviewVote> Votes { get; set; } = new List<ReviewVote>();
    public ICollection<Fix> Fixes { get; set; } = new List<Fix>();
}

/// <summary>An aspect the reviewer selected, with an optional personal note.</summary>
public class ReviewAspect
{
    public int Id { get; set; }

    public int ReviewId { get; set; }
    public Review Review { get; set; } = null!;

    public int AspectId { get; set; }
    public Aspect Aspect { get; set; } = null!;

    [MaxLength(1000)]
    public string? Note { get; set; }

    /// <summary>"Det problem har jeg også" votes.</summary>
    public ICollection<ReviewAspectVote> Votes { get; set; } = new List<ReviewAspectVote>();
}

/// <summary>"Det problem har jeg også" – another user confirms the same defect.</summary>
public class ReviewAspectVote
{
    public int Id { get; set; }

    public int ReviewAspectId { get; set; }
    public ReviewAspect ReviewAspect { get; set; } = null!;

    [MaxLength(450)]
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>"Jeg er enig i at produktet er godt/dårligt" – vote on a review verdict.</summary>
public class ReviewVote
{
    public int Id { get; set; }

    public int ReviewId { get; set; }
    public Review Review { get; set; } = null!;

    [MaxLength(450)]
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public bool IsAgree { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
