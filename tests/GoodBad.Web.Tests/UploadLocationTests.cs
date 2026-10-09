using GoodBad.Web.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace GoodBad.Web.Tests;

public class UploadLocationTests : IDisposable
{
    private readonly string _root;

    public UploadLocationTests()
    {
        _root = Path.Combine(Path.GetTempPath(), "goodbad-upload-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(_root, "wwwroot"));
    }

    private static IConfiguration Config(params (string Key, string Value)[] values)
        => new ConfigurationBuilder()
            .AddInMemoryCollection(values.ToDictionary(v => v.Key, v => (string?)v.Value))
            .Build();

    [Fact]
    public void Defaults_to_wwwroot_uploads()
    {
        var env = new FakeWebHostEnvironment(Path.Combine(_root, "wwwroot"));

        var location = UploadLocation.Resolve(Config(), env, NullLogger<UploadLocation>.Instance);

        Assert.Equal(Path.Combine(_root, "wwwroot", "uploads"), location.RootDirectory);
        Assert.Equal("/uploads", location.WebPath);
        Assert.False(location.IsOutsideWebRoot);
        Assert.True(Directory.Exists(location.RootDirectory));
    }

    [Fact]
    public void Honours_a_custom_web_path()
    {
        var env = new FakeWebHostEnvironment(Path.Combine(_root, "wwwroot"));

        var location = UploadLocation.Resolve(Config(("Storage:UploadWebPath", "/media")), env, NullLogger<UploadLocation>.Instance);

        Assert.Equal(Path.Combine(_root, "wwwroot", "media"), location.RootDirectory);
        Assert.Equal("/media", location.WebPath);
    }

    [Fact]
    public void Falls_back_to_App_Data_when_the_web_root_is_read_only()
    {
        var webRoot = Path.Combine(_root, "wwwroot");
        var contentRoot = Path.Combine(_root, "site");
        Directory.CreateDirectory(contentRoot);
        var env = new FakeWebHostEnvironment(webRoot, contentRoot);

        // Simulate a locked web root on the web hotel.
        var original = File.GetUnixFileMode(webRoot);
        File.SetUnixFileMode(webRoot, UnixFileMode.UserRead | UnixFileMode.UserExecute | UnixFileMode.GroupRead | UnixFileMode.GroupExecute | UnixFileMode.OtherRead | UnixFileMode.OtherExecute);
        try
        {
            var location = UploadLocation.Resolve(Config(), env, NullLogger<UploadLocation>.Instance);

            Assert.Equal(Path.Combine(contentRoot, "App_Data", "uploads"), location.RootDirectory);
            Assert.True(location.IsOutsideWebRoot);
            Assert.Equal("/uploads", location.WebPath);
        }
        finally
        {
            File.SetUnixFileMode(webRoot, original);
        }
    }

    public void Dispose()
    {
        try
        {
            foreach (var dir in Directory.GetDirectories(_root, "*", SearchOption.AllDirectories))
            {
                File.SetUnixFileMode(dir, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
            }

            Directory.Delete(_root, recursive: true);
        }
        catch { /* best effort */ }
    }
}
