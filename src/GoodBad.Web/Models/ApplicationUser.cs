using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GoodBad.Web.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(80)]
    public string? DisplayName { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Fix> Fixes { get; set; } = new List<Fix>();
    public ICollection<ProductList> Lists { get; set; } = new List<ProductList>();
}
