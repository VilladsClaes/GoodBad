using GoodBad.Web.Models;

namespace GoodBad.Web.ViewModels;

public class HomeViewModel
{
    /// <summary>Truly durable products we actively recommend.</summary>
    public List<ProductCard> Recommended { get; set; } = new();

    /// <summary>Products the community is warning against.</summary>
    public List<ProductCard> Warnings { get; set; } = new();

    /// <summary>Curated "must have" lists.</summary>
    public List<ProductList> Lists { get; set; } = new();

    public List<Category> TopCategories { get; set; } = new();

    public List<Review> LatestReviews { get; set; } = new();

    public int ProductCount { get; set; }
    public int ReviewCount { get; set; }
    public int BrandCount { get; set; }
    public int ListCount { get; set; }
}
