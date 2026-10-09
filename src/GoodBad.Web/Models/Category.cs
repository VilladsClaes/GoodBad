using System.ComponentModel.DataAnnotations;

namespace GoodBad.Web.Models;

/// <summary>A product category. Categories form a tree (Parent/Children).</summary>
public class Category
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(140)]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(600)]
    public string? Description { get; set; }

    /// <summary>Optional short emoji/icon shown in navigation.</summary>
    [MaxLength(24)]
    public string? Icon { get; set; }

    public int? ParentId { get; set; }
    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; } = new List<Category>();

    public int SortOrder { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<CategoryAspect> CategoryAspects { get; set; } = new List<CategoryAspect>();
    public ICollection<ProductList> ProductLists { get; set; } = new List<ProductList>();
}
