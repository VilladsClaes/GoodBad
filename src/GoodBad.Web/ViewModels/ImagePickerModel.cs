namespace GoodBad.Web.ViewModels;

/// <summary>Small view model for the shared image picker (camera, gallery or link).</summary>
public class ImagePickerModel
{
    /// <summary>Name of the IFormFile property to bind to, e.g. "ImageFile".</summary>
    public string FileField { get; set; } = "ImageFile";

    /// <summary>Name of the string property that holds an external image URL.</summary>
    public string UrlField { get; set; } = "ImageUrl";

    public string? CurrentUrl { get; set; }

    public string Label { get; set; } = "Billede";

    public string? Hint { get; set; }

    public string AspectCss { get; set; } = "admin-image-preview";
}
