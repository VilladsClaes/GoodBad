using System.ComponentModel.DataAnnotations;
using GoodBad.Web.Models;

namespace GoodBad.Web.ViewModels;

public class ReviewCreateViewModel
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vælg om du kan lide produktet eller ej.")]
    public Verdict? Verdict { get; set; }

    [Required(ErrorMessage = "Giv anmeldelsen en overskrift.")]
    [MaxLength(160)]
    [Display(Name = "Overskrift")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Fortæl hvad der er godt eller galt med produktet.")]
    [MaxLength(6000)]
    [Display(Name = "Hvad er der galt – eller hvorfor er det så godt?")]
    public string Body { get; set; } = string.Empty;

    [MaxLength(120)]
    [Display(Name = "Hvor længe har du haft produktet?")]
    public string? OwnershipDuration { get; set; }

    /// <summary>Selected aspect ids (defects for a bad verdict, strengths for a good one).</summary>
    public List<int> SelectedAspectIds { get; set; } = new();

    public List<AspectOption> AvailableAspects { get; set; } = new();
}

public class AspectOption
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public AspectKind Kind { get; set; }
    public bool IsCommon { get; set; }
}
