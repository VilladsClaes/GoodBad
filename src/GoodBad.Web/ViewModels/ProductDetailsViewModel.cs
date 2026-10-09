using GoodBad.Web.Models;

namespace GoodBad.Web.ViewModels;

public class ProductDetailsViewModel
{
    public Product Product { get; set; } = null!;

    public List<Review> GoodReviews { get; set; } = new();
    public List<Review> BadReviews { get; set; } = new();

    public int GoodCount { get; set; }
    public int BadCount { get; set; }

    /// <summary>Pairings where THIS product is the bad one: "du skulle have købt dette i stedet".</summary>
    public List<ProductRecommendation> BuyThisInstead { get; set; } = new();

    /// <summary>Pairings where THIS product is the good alternative: "godt at du ikke købte dette".</summary>
    public List<ProductRecommendation> BetterChoiceThan { get; set; } = new();

    public List<ProductList> AppearsInLists { get; set; } = new();

    public List<Fix> Fixes { get; set; } = new();

    /// <summary>Most reported defects for this product, with "det problem har jeg også" counts.</summary>
    public List<AspectTally> TopProblems { get; set; } = new();
    public List<AspectTally> TopPraises { get; set; } = new();

    public ReviewCreateViewModel NewReview { get; set; } = new();

    public bool UserHasReviewed { get; set; }

    /// <summary>The signed-in user's own lists, so a product can be added to them.</summary>
    public List<ProductList> MyLists { get; set; } = new();

    /// <summary>ReviewId -> whether the current user agrees with the verdict.</summary>
    public Dictionary<int, bool> UserReviewVotes { get; set; } = new();

    /// <summary>ReviewAspect ids the current user has confirmed with "det problem har jeg også".</summary>
    public HashSet<int> UserAspectVotes { get; set; } = new();

    public double Score => (GoodCount + BadCount) == 0
        ? 0
        : Math.Round(100.0 * (GoodCount + 3) / (GoodCount + BadCount + 6), 1);
}

public record AspectTally(Aspect Aspect, int ReviewAspectCount, int AgreeCount);
