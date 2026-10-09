using GoodBad.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace GoodBad.Web.Tests;

public class ImageStorageServiceTests : IDisposable
{
    private readonly string _webRoot;

    public ImageStorageServiceTests()
    {
        _webRoot = Path.Combine(Path.GetTempPath(), "goodbad-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_webRoot);
    }

    private ImageStorageService CreateService()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Storage:MaxImageBytes"] = "1024",
                ["Storage:AllowedExtensions"] = ".jpg,.jpeg,.png,.webp"
            })
            .Build();

        var location = UploadLocation.Resolve(
            configuration,
            new FakeWebHostEnvironment(_webRoot),
            NullLogger<UploadLocation>.Instance);

        return new ImageStorageService(
            location,
            configuration,
            new FakeSettings(),
            NullLogger<ImageStorageService>.Instance);
    }

    private static IFormFile Photo(string fileName, int size = 10, string contentType = "image/png")
    {
        var bytes = new byte[size];
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "ImageFile", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    [Fact]
    public async Task Saves_a_photo_and_returns_a_web_path()
    {
        var service = CreateService();

        var (url, error) = await service.SaveAsync(Photo("billede.png"), "reviews");

        Assert.Null(error);
        Assert.NotNull(url);
        Assert.StartsWith("/uploads/reviews/", url);
        Assert.True(File.Exists(Path.Combine(_webRoot, url!.TrimStart('/').Replace('/', Path.DirectorySeparatorChar))));
    }

    [Fact]
    public async Task Rejects_an_unsupported_file_type()
    {
        var service = CreateService();

        var (url, error) = await service.SaveAsync(Photo("ondt.exe", contentType: "application/octet-stream"), "reviews");

        Assert.Null(url);
        Assert.NotNull(error);
    }

    [Fact]
    public async Task Rejects_a_photo_that_is_too_large()
    {
        var service = CreateService();

        var (url, error) = await service.SaveAsync(Photo("stort.png", size: 4096), "reviews");

        Assert.Null(url);
        Assert.Contains("for stort", error);
    }

    [Fact]
    public async Task Does_nothing_when_no_file_is_posted()
    {
        var service = CreateService();

        var (url, error) = await service.SaveAsync(null, "reviews");

        Assert.Null(url);
        Assert.Null(error);
    }

    [Fact]
    public void Delete_ignores_external_links_and_paths_outside_the_upload_folder()
    {
        var service = CreateService();
        var outside = Path.Combine(_webRoot, "appsettings.json");
        File.WriteAllText(outside, "hemmelig");

        service.Delete("https://eksempel.dk/billede.png");
        service.Delete("/uploads/../appsettings.json");
        service.Delete("/uploads/reviews/findes-ikke.png");

        Assert.True(File.Exists(outside));
    }

    [Fact]
    public async Task Delete_removes_an_uploaded_photo()
    {
        var service = CreateService();
        var (url, _) = await service.SaveAsync(Photo("billede.png"), "products");
        var fullPath = Path.Combine(_webRoot, url!.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        Assert.True(File.Exists(fullPath));

        service.Delete(url);

        Assert.False(File.Exists(fullPath));
    }

    public void Dispose()
    {
        try { Directory.Delete(_webRoot, recursive: true); } catch { /* best effort */ }
    }
}
