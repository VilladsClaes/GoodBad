using GoodBad.Web.Models;

namespace GoodBad.Web.ViewModels;

public class CategoryDetailsViewModel
{
    public Category Category { get; set; } = null!;
    public List<ProductCard> Products { get; set; } = new();
    public List<Aspect> Problems { get; set; } = new();
    public List<Aspect> Praises { get; set; } = new();
    public List<ProductList> Lists { get; set; } = new();
}

public class ListDetailsViewModel
{
    public ProductList List { get; set; } = null!;
    public List<ProductCard> Products { get; set; } = new();
    public List<ProductListItem> Items { get; set; } = new();
}

public class BrandDetailsViewModel
{
    public Brand Brand { get; set; } = null!;
    public List<ProductCard> Products { get; set; } = new();
    public double AverageScore { get; set; }
    public int GoodCount { get; set; }
    public int BadCount { get; set; }
}

public class ProductBrowseViewModel
{
    public List<ProductCard> Products { get; set; } = new();
    public string? Query { get; set; }
    public int? CategoryId { get; set; }
    public string? Sort { get; set; }
    public List<Category> Categories { get; set; } = new();
}
