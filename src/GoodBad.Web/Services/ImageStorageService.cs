using Microsoft.AspNetCore.Http;

namespace GoodBad.Web.Services;

/// <summary>
/// Stores uploaded product photos and brand logos on disk under
/// wwwroot/uploads. On Simply.com the web root is writable, so files are
/// served straight from the web hotel – no external storage needed.
/// </summary>
public class ImageStorageService
{
    private static readonly string[] DefaultExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".bmp" };

    private readonly UploadLocation _location;
    private readonly IConfiguration _configuration;
    private readonly ISiteSettings _settings;
    private readonly ILogger<ImageStorageService> _logger;

    public ImageStorageService(
        UploadLocation location,
        IConfiguration configuration,
        ISiteSettings settings,
        ILogger<ImageStorageService> logger)
    {
        _location = location;
        _configuration = configuration;
        _settings = settings;
        _logger = logger;
    }

    /// <summary>Absolute path of the folder the photos are written to.</summary>
    public string UploadRoot => _location.RootDirectory;

    /// <summary>Public URL prefix for uploaded files, normally "/uploads".</summary>
    public string UploadWebPath => _location.WebPath;

    public async Task<long> GetMaxBytesAsync()
    {
        var fromSettings = await _settings.GetAsync(SettingKeys.MaxImageBytes);
        if (long.TryParse(fromSettings, out var fromDb) && fromDb > 0)
        {
            return fromDb;
        }

        return _configuration.GetValue("Storage:MaxImageBytes", 8 * 1024 * 1024);
    }

    public bool IsAllowedExtension(string fileName)
    {
        var allowed = (_configuration["Storage:AllowedExtensions"] ?? string.Join(',', DefaultExtensions))
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var extension = Path.GetExtension(fileName);
        return !string.IsNullOrEmpty(extension) && allowed.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>Validates an uploaded file and, if valid, stores it. Returns a web path like /uploads/products/ab12.jpg.</summary>
    public async Task<(string? Url, string? Error)> SaveAsync(IFormFile? file, string folder, CancellationToken cancellationToken = default)
    {
        if (file is null || file.Length == 0)
        {
            return (null, null);
        }

        if (!IsAllowedExtension(file.FileName))
        {
            return (null, "Billedet skal være en .jpg, .jpeg, .png, .webp, .gif eller .bmp fil.");
        }

        var maxBytes = await GetMaxBytesAsync();
        if (file.Length > maxBytes)
        {
            return (null, $"Billedet er for stort (maks. {maxBytes / (1024 * 1024)} MB).");
        }

        if (!string.IsNullOrEmpty(file.ContentType) && !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            return (null, "Filen ser ikke ud til at være et billede.");
        }

        var folderName = string.Join('/', folder.Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Select(SlugHelper.Slugify));

        var targetDirectory = Path.Combine(UploadRoot, folderName);
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(targetDirectory, fileName);

        try
        {
            Directory.CreateDirectory(targetDirectory);
            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream, cancellationToken);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            _logger.LogError(ex, "Kunne ikke gemme billedet {Path}.", fullPath);
            return (null, "Billedet kunne ikke gemmes. Kontrollér at appen har skriverettigheder til upload-mappen.");
        }

        _logger.LogInformation("Gemte billede {Path} ({Bytes} bytes).", fullPath, file.Length);
        return ($"{UploadWebPath}/{folderName}/{fileName}", null);
    }

    /// <summary>Deletes a previously uploaded file. Remote URLs (http/https) are ignored.</summary>
    public void Delete(string? url)
    {
        var prefix = UploadWebPath + "/";
        if (string.IsNullOrWhiteSpace(url) || !url.StartsWith(prefix, StringComparison.Ordinal))
        {
            return;
        }

        var relative = url[prefix.Length..].Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.GetFullPath(Path.Combine(UploadRoot, relative));
        var root = Path.GetFullPath(UploadRoot);

        // Never delete anything outside the upload folder.
        if (!fullPath.StartsWith(root, StringComparison.OrdinalIgnoreCase) || !File.Exists(fullPath))
        {
            return;
        }

        try
        {
            File.Delete(fullPath);
        }
        catch (IOException ex)
        {
            _logger.LogWarning(ex, "Kunne ikke slette {Path}.", fullPath);
        }
    }

    public static bool IsRemoteUrl(string? url)
        => !string.IsNullOrWhiteSpace(url)
           && (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase));
}
