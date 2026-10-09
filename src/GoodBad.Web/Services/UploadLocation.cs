using Microsoft.AspNetCore.Hosting;

namespace GoodBad.Web.Services;

/// <summary>
/// Where uploaded photos are stored. Default is <c>wwwroot/uploads</c>, which is
/// served by the normal static file middleware and works on Simply.com shared
/// hosting. If that folder cannot be written to (some hosts lock the web root),
/// the app automatically falls back to <c>App_Data/uploads</c>, which is served
/// through a separate static file provider under the same URL.
///
/// Both can be overridden in configuration:
///   Storage:UploadWebPath  – public URL prefix, default "/uploads"
///   Storage:UploadFolder   – folder, relative to the content root or absolute
/// </summary>
public sealed record UploadLocation(string RootDirectory, string WebPath, bool IsOutsideWebRoot)
{
    public static UploadLocation Resolve(IConfiguration configuration, IWebHostEnvironment environment, ILogger logger)
    {
        var webPath = NormalizeWebPath(configuration["Storage:UploadWebPath"]);
        var webRoot = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");

        var candidates = new List<string>();
        var configured = configuration["Storage:UploadFolder"];
        if (!string.IsNullOrWhiteSpace(configured))
        {
            candidates.Add(Path.IsPathRooted(configured)
                ? configured
                : Path.Combine(environment.ContentRootPath, configured));
        }
        else
        {
            candidates.Add(Path.Combine(webRoot, webPath.TrimStart('/')));
        }

        // Fallback used when the web root is read-only on the web hotel.
        candidates.Add(Path.Combine(environment.ContentRootPath, "App_Data", "uploads"));

        Exception? lastError = null;
        for (var i = 0; i < candidates.Count; i++)
        {
            var candidate = Path.GetFullPath(candidates[i]);
            try
            {
                Directory.CreateDirectory(candidate);
                var probe = Path.Combine(candidate, ".write-test");
                File.WriteAllText(probe, "ok");
                File.Delete(probe);

                var outsideWebRoot = !candidate.StartsWith(Path.GetFullPath(webRoot) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
                if (i > 0)
                {
                    logger.LogWarning(
                        "Kunne ikke skrive til den ønskede upload-mappe. Bruger i stedet {Folder}.",
                        candidate);
                }

                return new UploadLocation(candidate, webPath, outsideWebRoot);
            }
            catch (Exception ex)
            {
                lastError = ex;
                logger.LogWarning(ex, "Upload-mappen {Folder} kan ikke bruges.", candidate);
            }
        }

        throw new InvalidOperationException(
            "Ingen skrivbar mappe til billedupload blev fundet. Sæt \"Storage:UploadFolder\" til en mappe appen må skrive i.",
            lastError);
    }

    internal static string NormalizeWebPath(string? configured)
    {
        var value = string.IsNullOrWhiteSpace(configured) ? "/uploads" : configured.Trim();
        if (!value.StartsWith('/'))
        {
            value = "/" + value;
        }

        return value.TrimEnd('/');
    }
}
