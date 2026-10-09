using System.ComponentModel.DataAnnotations;

namespace GoodBad.Web.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Skriv dit navn.")]
    [MaxLength(80)]
    [Display(Name = "Navn")]
    public string DisplayName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Skriv din e-mail.")]
    [EmailAddress]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vælg en adgangskode.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Adgangskoden skal være mindst 8 tegn.")]
    [DataType(DataType.Password)]
    [Display(Name = "Adgangskode")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Adgangskoderne er ikke ens.")]
    [Display(Name = "Gentag adgangskode")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginViewModel
{
    [Required(ErrorMessage = "Skriv din e-mail.")]
    [EmailAddress]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Skriv din adgangskode.")]
    [DataType(DataType.Password)]
    [Display(Name = "Adgangskode")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Husk mig")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
