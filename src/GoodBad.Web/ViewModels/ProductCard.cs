namespace GoodBad.Web.ViewModels;

/// <summary>
/// Lightweight projection of a product for list/grid pages, including the
/// aggregate review signal used to rank the platform.
/// </summary>
public record ProductCard(
    int Id,
    string Name,
    string Slug,
    string? ImageUrl,
    string? BrandName,
    string? BrandSlug,
    string? BrandLogoUrl,
    string? BrandWebsiteUrl,
    string CategoryName,
    string CategorySlug,
    bool IsRecommended,
    int ReviewCount,
    int GoodCount,
    int BadCount)
{
    /// <summary>
    /// Bayesian ("true") score from 0-100. A product with a few good reviews is
    /// not automatically better than one with many, which is what we want for a
    /// platform that rewards long-term durability.
    /// </summary>
    public double Score => ReviewCount == 0
        ? 0
        : Math.Round(100.0 * (GoodCount + 3) / (ReviewCount + 6), 1);

    public int BadPercent => ReviewCount == 0 ? 0 : (int)Math.Round(100.0 * BadCount / ReviewCount);

    public string ScoreCssClass => Score switch
    {
        >= 80 => "score-great",
        >= 60 => "score-ok",
        >= 40 => "score-poor",
        _ => "score-bad"
    };
}
