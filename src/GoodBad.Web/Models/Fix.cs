using System.ComponentModel.DataAnnotations;

namespace GoodBad.Web.Models;

public enum FixSource
{
    /// <summary>Proposed by a community member.</summary>
    User = 0,
    /// <summary>Suggested by the system from keywords in the review.</summary>
    System = 1
}

public enum FixSourceKind
{
    YouTube = 0,
    Reddit = 1,
    Guide = 2,
    Search = 3
}

/// <summary>A community or system proposed fix / workaround for a problem.</summary>
public class Fix
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>The review (and therefore the problem) this fix addresses.</summary>
    public int? ReviewId { get; set; }
    public Review? Review { get; set; }

    [MaxLength(450)]
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string Body { get; set; } = string.Empty;

    [MaxLength(600)]
    public string? SourceUrl { get; set; }

    public FixSource Source { get; set; }

    public FixSourceKind? SourceKind { get; set; }

    public int? AspectId { get; set; }
    public Aspect? Aspect { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<FixVote> Votes { get; set; } = new List<FixVote>();
}

/// <summary>"Det fix virker" upvote.</summary>
public class FixVote
{
    public int Id { get; set; }

    public int FixId { get; set; }
    public Fix Fix { get; set; } = null!;

    [MaxLength(450)]
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Keyword rule used by the fix-suggestion engine: when a review mentions any of
/// <see cref="Keywords"/>, the linked YouTube/Reddit/guide resource is suggested.
/// </summary>
public class FixSuggestionRule
{
    public int Id { get; set; }

    /// <summary>Comma separated Danish keywords, matched case-insensitively.</summary>
    [Required, MaxLength(300)]
    public string Keywords { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(600)]
    public string? Url { get; set; }

    public FixSourceKind Kind { get; set; }

    public int? AspectId { get; set; }
    public Aspect? Aspect { get; set; }
}
