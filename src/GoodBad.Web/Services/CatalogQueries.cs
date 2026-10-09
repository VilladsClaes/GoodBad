using GoodBad.Web.Models;
using GoodBad.Web.ViewModels;

namespace GoodBad.Web.Services;

public static class CatalogQueries
{
    /// <summary>Projects products into the lightweight card projection used in lists.</summary>
    public static IQueryable<ProductCard> ToCards(this IQueryable<Product> query)
        => query.Select(p => new ProductCard(
            p.Id,
            p.Name,
            p.Slug,
            p.ImageUrl,
            p.Brand != null ? p.Brand.Name : null,
            p.Brand != null ? p.Brand.Slug : null,
            p.Brand != null ? p.Brand.LogoUrl : null,
            p.Brand != null ? p.Brand.WebsiteUrl : null,
            p.Category.Name,
            p.Category.Slug,
            p.IsRecommended,
            p.Reviews.Count,
            p.Reviews.Count(r => r.Verdict == Verdict.Good),
            p.Reviews.Count(r => r.Verdict == Verdict.Bad)));
}
