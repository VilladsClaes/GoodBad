using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GoodBad.Web.ViewModels;

public class ProductCreateViewModel
{
    [Required(ErrorMessage = "Produktet skal have et navn.")]
    [MaxLength(160)]
    [Display(Name = "Produktnavn")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vælg en kategori.")]
    [Display(Name = "Produktkategori")]
    public int CategoryId { get; set; }

    [Display(Name = "Brand / producent")]
    public int? BrandId { get; set; }

    [MaxLength(80)]
    [Display(Name = "Modelnummer")]
    public string? ModelNumber { get; set; }

    [MaxLength(600)]
    [Display(Name = "Billede-URL")]
    public string? ImageUrl { get; set; }

    /// <summary>Photo taken with the camera or picked from the gallery.</summary>
    [Display(Name = "Billede")]
    public IFormFile? ImageFile { get; set; }

    [MaxLength(4000)]
    [Display(Name = "Beskrivelse")]
    public string? Description { get; set; }
}
