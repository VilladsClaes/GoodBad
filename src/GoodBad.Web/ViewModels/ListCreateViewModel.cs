using System.ComponentModel.DataAnnotations;

namespace GoodBad.Web.ViewModels;

public class ListCreateViewModel
{
    [Required(ErrorMessage = "Listen skal have et navn.")]
    [MaxLength(160)]
    [Display(Name = "Listens navn")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    [Display(Name = "Beskrivelse")]
    public string? Description { get; set; }

    [Display(Name = "Kategori")]
    public int? CategoryId { get; set; }
}
